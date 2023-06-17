using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using corel = Corel.Interop.VGCore;


namespace br.com.Bonus630DevToolsBar.RecentFiles
{
    /// <summary>
    /// Interaction logic for RecentFilesView.xaml
    /// </summary>
    public partial class RecentFilesView : UserControl
    {
        private corel.Application corelApp;
        private StylesController stylesController;
        RecentFileModel rfm;
        RecentFilesViewModel dataContext;
         int limit = 10;

        public RecentFilesView(object app)
        {
            InitializeComponent();
            try
            {

                this.corelApp = app as corel.Application;
                stylesController = new StylesController(this.Resources, this.corelApp);
                rfm = new RecentFileModel(this.corelApp.VersionMajor);
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
            Save(Doc.FileName, FileName);
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
                file = rfm.InsertData(index, name, fullName, false, 1, 0);

                dataContext.Add(file);
            }
            else
            {
                if(file == null)
                {
                    long minTime = dataContext.Files.Min(m => m.OpenedTime);
                    file = dataContext.Files.SingleOrDefault(r => r.OpenedTime == minTime);
                    file.Name = name;
                    file.FullName = fullName;
                    file.OpenTimes = 0;
                    file.AutoLoad = false;
                    file.OpenedTime = 0;
                }

                rfm.UpdateFile(file.ID, file.Index,file.Name,file.FullName, file.OpenTimes++, file.OpenedTime, file.AutoLoad);
            }
            file.OpenDate = DateTime.Now;
            file.IsOpened = true;
        }
        private void Update(string path)
        {
            RecentFileViewModel file = dataContext[path];
            if (file != null)
            {
                rfm.UpdateFile(file.ID, file.Index,file.Name,file.FullName, file.OpenTimes, (DateTime.Now - file.OpenDate).Ticks, file.AutoLoad);
                file.IsOpened = false;
            }
        }
        private void Load()
        {
            dataContext.Files = rfm.Fill(limit);

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
                LoadThumb(dataContext[i].ID);
            }
        }
        private void LoadThumb(int id)
        {

            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    dataContext[id].Thumb = rfm.GetThumb(dataContext[id].FullName);
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
                    corelApp.OpenDocument(dataContext[i].FullName);
            }
        }
        private void CheckFileExits()
        {
            int count = dataContext.Count;
            int current = 0;
            while(current < count)
            {
                if (!File.Exists(dataContext[current].FullName))
                {
                    rfm.DeleteFile(dataContext[current].ID);
                    dataContext.Remove(dataContext[current]);
                }
                else
                {
                    current++;
                }
                count = dataContext.Count;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Items.Width = 200;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            rfm.DeleteFile((int)(sender as MenuItem).Tag);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateAutoLoad((int)(sender as MenuItem).Tag, true);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            UpdateAutoLoad((int)(sender as MenuItem).Tag, false);
        }
        private void UpdateAutoLoad(int id, bool autoLoad)
        {
            var r = dataContext.GetItem(id);
            if (r != null)
            {
                r.AutoLoad = autoLoad;
                rfm.UpdateFile(r.ID, r.Index,r.Name,r.FullName, r.OpenTimes, r.OpenedTime, autoLoad);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            RecentFileViewModel r = dataContext[(sender as Button).Tag.ToString()];
            if (r != null && r.Thumb == null)
                LoadThumb(r.ID);

        }


      
    }

}
