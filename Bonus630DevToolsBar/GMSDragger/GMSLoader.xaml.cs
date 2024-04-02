using br.com.Bonus630DevToolsBar.RecentFiles;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using corel = Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    /// <summary>
    /// Interaction logic for CQLRunner.xaml
    /// </summary>
    public partial class GMSLoader : UserControl, INotifyPropertyChanged
    {

        private corel.Application corelApp;
        private StylesController stylesController;

        private ObservableCollection<MyFile> files;
        public ICommand UnloadGMSCommand { get; set; }
        public ICommand LoadGMSCommand { get; set; }
        public ICommand OpenGMSCommand { get; set; }


        public ObservableCollection<MyFile> Files
        {
            get { return files; }
            set { files = value; OnFilesChanged(); }
        }

        private void OnFilesChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Files"));
        }
        public GMSLoader(object app)
        {
            InitializeComponent();
            try
            {
                this.corelApp = app as corel.Application;
                stylesController = new StylesController(this.Resources, this.corelApp);
                UnloadGMSCommand = new br.com.Bonus630DevToolsBar.RunCommandDocker.BindingCommand<string>(UnloadGMS);
                LoadGMSCommand = new br.com.Bonus630DevToolsBar.RunCommandDocker.BindingCommand<string>(LoadGMS);
                OpenGMSCommand = new br.com.Bonus630DevToolsBar.RunCommandDocker.BindingCommand<string>(LoadVBA);
                this.Loaded += GMSLoader_Loaded;
            }
            catch
            {
                MessageBox.Show("VGCore Erro");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void UnloadGMS(string file)
        {
            LoadUnload(file, false);
        }
        private void LoadGMS(string file)
        {
            LoadUnload(file);
        }
        private void LoadVBA(string file)
        {
            LoadUnload(file);
            this.corelApp.FrameWork.Automation.InvokeItem("28e16db6-6339-440d-af0d-f58ac27c115d");
        }
        private void LoadUnload(string file, bool load = true)
        {

            Thread th = new Thread(() =>
            {

                try
                {
                    if (!this.corelApp.InitializeVBA())
                        return;
                    int index = Files.IndexOf(Files.FirstOrDefault(f => f.FullName == file));
                    GMSProjects gp = this.corelApp.GMSManager.Projects;
                    if (load && file.EndsWith(".bak"))
                    {

                        string n = file.Replace(".bak", "");
                        File.Move(file, n);
                        Files[index].FullName = n;

                        OnFilesChanged();
                        while (!File.Exists(n))
                            Thread.Sleep(1);

                        gp.Load(n, false, false);
                        
                        return;
                    }
                    if (!load && file.ToLower().EndsWith(".gms"))
                    {
                        for (int i = 1; i <= gp.Count; i++)
                        {
                            if (gp[i].FullFileName.ToLower() == file.ToLower())
                            {
                                gp[i].Unload();
                                break;
                            }
                        }
                        string n = file + ".bak";
                        File.Move(file, n);
                        Files[index].FullName = n;
                        OnFilesChanged();
                        return;
                    }
                }
                catch
                {

                }
            });
            th.IsBackground = true;
            th.Start();

            //
            //files = Directory.GetFiles(path);
            //for (int i = 0; i < files.Length; i++)
            //{
            //    try

            //    {
            //        if (files[i].ToLower().EndsWith(".gms"))

            //            gp.Load(files[i], false, false);
            //    }
            //    catch
            //    {

            //    }
            //}
        }
        private void GMSLoader_Loaded(object sender, RoutedEventArgs e)
        {
            stylesController.LoadThemeFromPreference();
            try
            {
                string path = ControlUI.corelApp.GMSManager.UserGMSPath;

                FileInfo[] f = (new DirectoryInfo(path)).GetFiles();
                Files = new ObservableCollection<MyFile>();
                for (int i = 0; i < f.Length; i++)
                {
                    if (f[i].Extension.ToLower() == ".gms" || f[i].Extension.ToLower() == ".bak")
                        Files.Add(new MyFile(f[i].FullName));
                }

                this.DataContext = this;
                OnFilesChanged();
            }
            catch { }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadVBA(((sender as ListViewItem).Content as MyFile).FullName);
        }
    }
    public class MyFile : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value;OnFilesChanged("Name"); }
        }

        private string extension;

        public string Extension
        {
            get { return extension; }
            set { extension = value; OnFilesChanged("Extension"); }
        }
        private string fullName;

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; SetNewName(); OnFilesChanged("FullName"); }
        }

        public MyFile(string fullname)
        {
         
            FullName = fullname;
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnFilesChanged(string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        private void SetNewName()
        {
            Name = fullName.Substring(fullName.LastIndexOf("\\")+1);
            Extension = Name.Substring(name.LastIndexOf("."));
        }
    }

}
