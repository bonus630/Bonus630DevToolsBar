﻿using br.com.Bonus630DevToolsBar.Converters;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using corel = Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.RecentFiles
{

    public partial class RecentFilesView : UserControl
    {
        public static int Test = 1;

        private corel.Application corelApp;
        private StylesController stylesController;
        RecentFileModel recentFileModel;
        RecentFilesViewModel dataContext;
        int limit = 60;
        private ushort CurrentPage = 0;
        private int TotalRows = 0;
        private ushort MaxPerPage = 10;
        private bool loaded = false;
        //private DispatcherTimer AjustMaxPerPageTimer;
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



            }
            catch
            {
                MessageBox.Show("VGCore Erro");
            }
            //AjustMaxPerPageTimer = new DispatcherTimer
            //{
            //    Interval = TimeSpan.FromMilliseconds(400)
            //};
            //AjustMaxPerPageTimer.Tick += AjustMaxPerPageTimer_Tick;
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
            this.corelApp.DocumentOpen -= CorelApp_DocumentOpen;
            this.corelApp.DocumentClose -= CorelApp_DocumentClose;
            this.corelApp.DocumentAfterSave -= CorelApp_DocumentAfterSave;
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
            stylesController.LoadThemeFromPreference();
            this.DataContext = dataContext;
            CurrentPage = Properties.Settings.Default.RecentFilesPosition;
            TotalRows = recentFileModel.GetTotalRows();
            this.loaded = true;
            this.corelApp.DocumentOpen += CorelApp_DocumentOpen;
            this.corelApp.DocumentClose += CorelApp_DocumentClose;
            this.corelApp.DocumentAfterSave += CorelApp_DocumentAfterSave;
            //    Load();

        }


        private void Save(string name, string fullName)
        {
            //System.Windows.Forms.MessageBox.Show(name);
            RecentFileViewModel file = dataContext[fullName];
            if (file == null && TotalRows < limit)
            {
                int index = dataContext.Count;
                file = recentFileModel.InsertData(index, name, fullName, false, 1, DateTime.Now.Ticks);
                file.SetAbsName();
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
                LoadThumbThread(dataContext.GetIndex(file.FullName));
            }
        }
        private void Load()
        {
            SetPageButtonsVisibility();
            SetMenu(Properties.Settings.Default.UseIndex);
            dataContext.Files = recentFileModel.Fill(MaxPerPage, CurrentPage * MaxPerPage);
            CheckFileExits();
            if ((bool)ck_autoLoad.IsChecked)
                OpenAutoFiles();
            Thread loadThumbThread = new Thread(new ParameterizedThreadStart(LoadThumbs));
            loadThumbThread.IsBackground = true;
            loadThumbThread.Start();

        }

        private void LoadThumbs(object file = null)
        {
            if (file == null)
            {
                for (int i = 0; i < dataContext.Count; i++)
                {
                    //LoadThumb(dataContext[i].ID);
                    LoadThumb(i);
                }
            }
            else
            {
                LoadThumb(dataContext.GetIndex((file as RecentFileViewModel).FullName));
            }
        }
        private void LoadThumb(int id)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    object[] r = recentFileModel.GetThumbAndVersion(dataContext[id].FullName);
                    Debug.WriteLine("Thumb - " + dataContext[id].Name);
                    dataContext[id].Thumb = (BitmapSource)r[0];
                    dataContext[id].Version = (int)r[1];
                }
                catch { }
            });
        }
        private void LoadThumbThread(int id)
        {
            var file = dataContext.GetItem(id);
            if (file != null && file.Thumb == null)
            {
                Thread loadThumbThread = new Thread(new ParameterizedThreadStart(LoadThumbs));
                loadThumbThread.IsBackground = true;
                loadThumbThread.Start(file);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string file = (sender as Button).Tag.ToString();
                if (!DocumentIsOpened(file))
                    this.corelApp.OpenDocument(file);
            }
            catch { }
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
            Debug.WriteLine((Test++));
            var s = this.corelApp.CreateStructOpenOptions();

            for (int i = 0; i < dataContext.Count; i++)
            {
                if (dataContext[i].AutoLoad)
                {
                    try
                    {
                        Debug.WriteLine("Open - " + dataContext[i].Name);
                        if (!DocumentIsOpened(dataContext[i].FullName))
                            this.corelApp.OpenDocumentEx(dataContext[i].FullName, s);
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show(e.Message);
                    }
                }
            }
        }
        private bool DocumentIsOpened(string FullName)
        {
            for (int i = 1; i <= corelApp.Documents.Count; i++)
            {
                if (corelApp.Documents[i].FullFileName == FullName)
                    return true;
            }
            return false;
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
        private void MenuItem_Click_Pin(object sender, RoutedEventArgs e)
        {
            UpdatePinned((int)(sender as MenuItem).Tag, true);
        }
        private void MenuItem_Click_UnPin(object sender, RoutedEventArgs e)
        {
            UpdatePinned((int)(sender as MenuItem).Tag, false);
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
        private void UpdateAutoLoad(int ID, bool autoLoad)
        {
            var r = dataContext.GetItem(ID);
            //var r = dataContext[index];
            if (r != null)
            {
                r.AutoLoad = autoLoad;
                recentFileModel.UpdateFile(r.ID, r.Index, r.Name, r.FullName, r.OpenTimes, r.OpenedTime, autoLoad, r.Pinned);
            }
        }
        private void UpdatePinned(int ID, bool isPinned)
        {
            var r = dataContext.GetItem(ID);
            //var r = dataContext[index];
            if (r != null)
            {
                int pinned = -1;
                if (isPinned)
                    pinned = recentFileModel.GetMaxPinned() + 1;
                r.IsPinned = isPinned;
                r.Pinned = pinned;
                recentFileModel.UpdateFile(r.ID, r.Index, r.Name, r.FullName, r.OpenTimes, r.OpenedTime, r.AutoLoad, pinned);
            }
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            RecentFileViewModel r = dataContext[(sender as Button).Tag.ToString()];
            if (r != null && r.Thumb == null)
                LoadThumbThread(r.ID);
            InfoPopup.DataContext = r;
            InfoPopup.IsOpen = true;
            //popupTimer.Start();

        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender as Button).IsMouseOver)
            {
                InfoPopup.IsOpen = false;
                Debug.WriteLine("a");
            }
        }
        //private void AjustMaxPerPageTimer_Tick(object sender, EventArgs e)
        //{

        //    AjustMaxPerPageTimer.Stop(); 
        //}
        private void MenuItem_Click_CopyPath(object sender, RoutedEventArgs e)
        {
            RecentFileViewModel r = dataContext.GetItem((int)(sender as MenuItem).Tag);
            System.Windows.Clipboard.SetText(r.FullName);
        }

        private void MenuItem_Click_OpenDirectory(object sender, RoutedEventArgs e)
        {
            try
            {
                RecentFileViewModel r = dataContext.GetItem((int)(sender as MenuItem).Tag);
                if (r != null)
                    System.Diagnostics.Process.Start(r.FullName.Substring(0, r.FullName.LastIndexOf("\\")));
            }
            catch (Exception ex)
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
            if (useIndex == -1)
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

        private void back_Click(object sender, RoutedEventArgs e)
        {

            ChangePage(false);
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(true);
        }

        private void ChangePage(bool increase)
        {
            if (CurrentPage > 0 && !increase)
            {
                CurrentPage -= 1;
            }
            if (CurrentPage * MaxPerPage < TotalRows && increase)
            {
                CurrentPage += 1;
            }
            SetPage(CurrentPage);
            Load();

        }
        private void SetPage(ushort page)
        {
            CurrentPage = page;
            Properties.Settings.Default.RecentFilesPosition = CurrentPage;
            Properties.Settings.Default.Save();

        }
        private void SetPageButtonsVisibility()
        {
            dataContext.CanDecrease = CurrentPage > 0;
            dataContext.CanIncrease = CurrentPage * MaxPerPage < (TotalRows- MaxPerPage) && MaxPerPage < TotalRows;

        }

        private void DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (!loaded || e.PreviousSize.Width == 0)
                return;
            ushort itens = (ushort)((e.NewSize.Width - 78) / 28);
            if (itens != 0 && itens != MaxPerPage && CurrentPage > 0)
            {
                int total = CurrentPage  * MaxPerPage;
                //if (itens > (CurrentPage + 1) * MaxPerPage)
                //    SetPage(CurrentPage--);
                //else
                //{
                    SetPage((ushort)(total / itens ));
                //}
                
                MaxPerPage = itens;
                Load();
            }
            if (itens > TotalRows)
            {
                MaxPerPage = (ushort)TotalRows;
                SetPage(0);
                Load();
            }
        }


    }

}
