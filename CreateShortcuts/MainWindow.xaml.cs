using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;

namespace CreateShortcuts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> shortcuts = new List<string>();
        string path = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
        }
        private void CreateShortcut(string targetPath)
        {
            string exeName = Path.GetFileNameWithoutExtension(targetPath);
           // string destFolder = Path.Combine(path,Path.GetDirectoryName(targetPath));


            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), exeName + ".lnk");

            var shortcut = new IWshRuntimeLibrary.WshShell().CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;

            shortcut.TargetPath = targetPath;

            shortcut.IconLocation = targetPath.Replace(".exe",".ico");
            shortcut.WorkingDirectory = path;
            // Salvar o atalho
            shortcut.Save();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ck_drawui.IsChecked)
                shortcuts.Add("DrawUIExplorer.exe");
            else
                shortcuts.Remove("DrawUIExplorer.exe");
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if ((bool)ck_drawui.IsChecked)
                shortcuts.Add("CDRCommandBarBuilder.exe");
            else
                shortcuts.Remove("CDRCommandBarBuilder.exe");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < shortcuts.Count; i++)
            {
                CreateShortcut(Path.Combine(path,shortcuts[i]));
            }
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
