using br.com.Bonus630DevToolsBar.RunCommandDocker;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using form = System.Windows.Forms;

namespace br.com.Bonus630DevToolsBar.Folders
{
    /// <summary>
    /// Interaction logic for Folders.xaml
    /// </summary>
    public partial class Folders : UserControl
    {

        public BindingCommand<string> PastFolderCommand { get; set; }
        public BindingCommand<Folder> OpenFolderCommand { get; set; }
        public BindingCommand<Folder> PastIconeCommand { get; set; }
        public BindingCommand<Folder> RemoveCommand { get; set; }
        public ObservableCollection<Folder> _Folders { get; set; }

        private StylesController stylesController;


        public Folders(object app)
        {
            InitializeComponent();

            PastFolderCommand = new BindingCommand<string>(PastFolder);
            OpenFolderCommand = new BindingCommand<Folder>(OpenFolder);
            PastIconeCommand = new BindingCommand<Folder>(PastIcone);
            RemoveCommand = new BindingCommand<Folder>(Remove);


            _Folders = new ObservableCollection<Folder>();
            StartFolderShortcuts();
      
            DataContext = this;
            try
            {
                stylesController = new StylesController(this.Resources, app as Corel.Interop.VGCore.Application);
                stylesController.LoadThemeFromPreference();
            }
            catch { }
        }

        private void PastFolder(string folder)
        {
            folder = Clipboard.GetText();
        

            Folder f = new Folder(this);
            f.Index = Properties.Settings.Default.foldersPath.Count;
            f.Path = folder;
            
            _Folders.Add(f);
            SaveOrUpdate(f);

        }
        private void OpenFolder(Folder folder)
        {
            try
            {
                System.Diagnostics.Process.Start(folder.Path);
            }
            catch { }
        }
        private void PastIcone(Folder folder)
        {
            try
            {
                form.OpenFileDialog of = new form.OpenFileDialog();
                of.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;";
                of.Multiselect = false;
                of.DefaultExt = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //MessageBox.Show(of.ShowDialog().ToString());
                if (of.ShowDialog().Equals(form.DialogResult.OK))
                {
                    folder.SetIcone(of.FileName);
                    SaveOrUpdate(folder);
                }
            }
            catch
            {

            }
        }
        private void SaveOrUpdate(Folder folder)
        {
            
            if (folder.Index >= Properties.Settings.Default.foldersPath.Count)
            {
                Properties.Settings.Default.foldersPath.Add(folder.Path);
                Properties.Settings.Default.imagesPath.Add(folder.GetIcone());
            }
            else
            {
                Properties.Settings.Default.foldersPath[folder.Index] = folder.Path;
                Properties.Settings.Default.imagesPath[folder.Index] = folder.GetIcone();
            }
            Properties.Settings.Default.Save();
        }
        private void Remove(Folder folder)
        {
            _Folders.Remove(folder);
            Properties.Settings.Default.foldersPath.RemoveAt(folder.Index);
            Properties.Settings.Default.imagesPath.RemoveAt(folder.Index);
            Properties.Settings.Default.Save();
        }
        private void StartFolderShortcuts()
        {
          
            if (Properties.Settings.Default.foldersPath == null)
                Properties.Settings.Default.foldersPath = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.imagesPath == null)
                Properties.Settings.Default.imagesPath = new System.Collections.Specialized.StringCollection();
            

            for (int i = 0; i < Properties.Settings.Default.foldersPath.Count; i++)
            {
                Folder f = new Folder(this);
                f.Index = i;
                f.Path = Properties.Settings.Default.foldersPath[i];
                f.SetIcone(Properties.Settings.Default.imagesPath[i]);
                _Folders.Add(f);
            }
        }
    }
}
