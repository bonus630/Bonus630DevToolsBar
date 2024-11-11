using br.com.Bonus630DevToolsBar.Converters;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using corel = Corel.Interop.VGCore;



namespace br.com.Bonus630DevToolsBar.RecentFiles
{
  
    public partial class RecentFilesView : UserControl
    { 
        private corel.Application corelApp;
        private StylesController stylesController;
        RecentFileModel recentFileModel;
        RecentFilesViewModel dataContext;
        int limit = 20;

        //public int Height
        //{
        //    get;
        //    set;
        //}

        public RecentFilesView(object app)
        {
        
            InitializeComponent();
            try
            {
                this.corelApp = app as corel.Application;
           
                stylesController = new StylesController(this.Resources, this.corelApp);
                recentFileModel = new RecentFileModel(this.corelApp.VersionMajor);
                dataContext = new RecentFilesViewModel();

                ck_autoLoad.IsChecked = Properties.Settings.Default.AutoLoad;
                this.corelApp.DocumentOpen += CorelApp_DocumentOpen;
                this.corelApp.DocumentClose += CorelApp_DocumentClose;
                this.corelApp.DocumentAfterSave += CorelApp_DocumentAfterSave;
                

            }
            catch
            {
                MessageBox.Show("VGCore Erro");
            }
            this.Loaded += RecentFilesView_Loaded;
            this.Unloaded += RecentFilesView_Unloaded;
        }

    

    

        private void CorelApp_DocumentAfterSave(Document Doc, bool SaveAs, string FileName)
        {
            string name = Doc.FileName;
            if (string.IsNullOrEmpty(name))
                name = FileName.Substring(FileName.LastIndexOf("\\") + 1);

            Save(name, FileName);
        }

        private void RecentFilesView_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void CorelApp_DocumentClose(Document Doc)
        {
            Update(Doc.FullFileName);
        }

        private void CorelApp_DocumentOpen(Document Doc, string FileName)
        {
            Save(Doc.FileName, FileName);
        }

        private void RecentFilesView_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = dataContext;
            stylesController.LoadThemeFromPreference();
            Load();
            

        }


        private void Save(string name, string fullName)
        {
            //System.Windows.Forms.MessageBox.Show(name);
            RecentFileViewModel file = dataContext[fullName];
            if (file == null && dataContext.Count < this.limit)
            {
                int index = dataContext.Count;
                file = recentFileModel.InsertData(index, name, fullName, false, 1, DateTime.Now.Ticks);

                dataContext.Add(file);
            }
            else
            {
                if (file == null)
                {
                    long minTime = dataContext.Files.Min(m => m.OpenedTime);
                    file = dataContext.Files.SingleOrDefault(r => r.OpenedTime == minTime);
                    file.Name = name;
                    file.FullName = fullName;
                    file.OpenTimes = 0;
                    file.AutoLoad = false;
                    file.OpenedTime = 0;
                }

                recentFileModel.UpdateFile(file.ID, file.Index, file.Name, file.FullName, file.OpenTimes++, file.OpenedTime, file.AutoLoad);
            }
            file.OpenDate = DateTime.Now;
            file.IsOpened = true;
        }
        private void Update(string path)
        {
            RecentFileViewModel file = dataContext[path];
            if (file != null)
            {
                recentFileModel.UpdateFile(file.ID, file.Index, file.Name, file.FullName, file.OpenTimes, DateTime.Now.Ticks, file.AutoLoad);
                file.IsOpened = false;
            }
        }
        private void Load()
        {
            SetMenu(Properties.Settings.Default.UseIndex);
            dataContext.Files = recentFileModel.Fill(limit);
            CheckFileExits();
            if ((bool)ck_autoLoad.IsChecked)
                OpenAutoFiles();
            Thread loadThumbThread = new Thread(new ThreadStart(LoadThumbs));
            loadThumbThread.IsBackground = true;
            loadThumbThread.Start();
        }

        private void LoadThumbs()
        {
            for (int i = 0; i < dataContext.Count; i++)
            {
                //LoadThumb(dataContext[i].ID);
                LoadThumb(i);
            }
        }
        private void LoadThumb(int id)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    
                    object[] r = recentFileModel.GetThumbAndVersion(dataContext[id].FullName);
                    dataContext[id].Thumb = (BitmapSource)r[0];
                    dataContext[id].Version = (int)r[1];
                }
                catch { }
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.corelApp.OpenDocument((sender as Button).Tag.ToString());
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var ck = (sender as CheckBox);
            Properties.Settings.Default.AutoLoad = (bool)ck.IsChecked;
            Properties.Settings.Default.Save();
            if ((bool)ck.IsChecked)
                OpenAutoFiles();
        }
        private void OpenAutoFiles()
        {
            for (int i = 0; i < dataContext.Count; i++)
            {
                if (dataContext[i].AutoLoad)
                {
                    try
                    {
                        corelApp.OpenDocument(dataContext[i].FullName);
                    }
                    catch(Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show(e.Message);
                    }
                }
            }
        }
        private void CheckFileExits()
        {
            int count = dataContext.Count;
            int current = 0;
            while (current < count)
            {
                if (!File.Exists(dataContext[current].FullName))
                {
                    recentFileModel.DeleteFileDataFromDB(dataContext[current].ID);
                    dataContext.Remove(dataContext[current]);
                }
                else
                {
                    dataContext[current].Index = current;
                    current++;
                }
                count = dataContext.Count;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Items.Width = 200;
        }
        private void MenuItem_Click_RemoveFileData(object sender, RoutedEventArgs e)
        {
            int ID = (int)(sender as MenuItem).Tag;
            recentFileModel.DeleteFileDataFromDB(ID);
            dataContext.Remove(ID);
        }
        private void MenuItem_Click_AddToAutoLoad(object sender, RoutedEventArgs e)
        {
            UpdateAutoLoad((int)(sender as MenuItem).Tag, true);
        }
        private void MenuItem_Click_RemoveFromAutoLoad(object sender, RoutedEventArgs e)
        {
            UpdateAutoLoad((int)(sender as MenuItem).Tag, false);
        }
        private void UpdateAutoLoad(int index, bool autoLoad)
        {
            //var r = dataContext.GetItem(id);
            var r = dataContext[index];
            if (r != null)
            {
                r.AutoLoad = autoLoad;
                
                recentFileModel.UpdateFile(r.ID, r.Index, r.Name, r.FullName, r.OpenTimes, r.OpenedTime, autoLoad);
            }
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            RecentFileViewModel r = dataContext[(sender as Button).Tag.ToString()];
            if (r != null && r.Thumb == null)
                LoadThumb(r.ID);

        }

        private void MenuItem_Click_CopyPath(object sender, RoutedEventArgs e)
        {
            RecentFileViewModel r = dataContext[(int)(sender as MenuItem).Tag];
            System.Windows.Clipboard.SetText(r.FullName);
        }

        private void MenuItem_Click_OpenDirectory(object sender, RoutedEventArgs e)
        {
            try
            {
                RecentFileViewModel r = dataContext[(int)(sender as MenuItem).Tag];
                System.Diagnostics.Process.Start(r.FullName.Substring(0, r.FullName.LastIndexOf("\\")));
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetMenu(short useIndex)
        {
            //index = 0,name = -1,thumb = 1
            menu_index.IsChecked = false;
            menu_name.IsChecked = false;
            menu_thumb.IsChecked = false;
            if (useIndex == 0)
                menu_index.IsChecked = true;
            if(useIndex == -1)
            menu_name.IsChecked = true;
            if (useIndex == 1)
                menu_thumb.IsChecked = true;
        }

        private void menu_index_Click(object sender, RoutedEventArgs e)
        {
            //index = 0,name = -1,thumb = 1
            if (menu_index.IsChecked)
               menu_name.IsChecked = false;
            if (menu_index.IsChecked)
                menu_thumb.IsChecked = false;
            if (!menu_index.IsChecked && !menu_name.IsChecked && !menu_thumb.IsChecked)
                menu_index.IsChecked = true;

            Properties.Settings.Default.UseIndex = 0;
            Properties.Settings.Default.Save();
            this.dataContext.ChangeAbsName();
        
        }  
        private void menu_name_Click(object sender, RoutedEventArgs e)
        {
            //index = 0,name = -1,thumb = 1
            if (menu_name.IsChecked)
                menu_index.IsChecked = false;
            if (menu_name.IsChecked)
                menu_thumb.IsChecked = false;
            if (!menu_index.IsChecked && !menu_name.IsChecked && !menu_thumb.IsChecked)
                menu_name.IsChecked = true;

            Properties.Settings.Default.UseIndex = -1;
            Properties.Settings.Default.Save();
            this.dataContext.ChangeAbsName();
        }

        private void menu_thumb_Click(object sender, RoutedEventArgs e)
        {
            //index = 0,name = -1,thumb = 1
            if (menu_thumb.IsChecked)
                menu_name.IsChecked = false;
            if (menu_thumb.IsChecked)
                menu_index.IsChecked = false;
            if (!menu_index.IsChecked && !menu_name.IsChecked && !menu_thumb.IsChecked)
                menu_thumb.IsChecked = true;

            Properties.Settings.Default.UseIndex = 1;
            Properties.Settings.Default.Save();
            this.dataContext.ChangeAbsName();
        }
    }

}
