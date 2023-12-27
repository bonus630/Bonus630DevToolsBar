using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
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
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        public StylesController StylesController { get; set; }
        ViewModels.ConfigViewModel dataContext;
        public Config()
        {
            InitializeComponent();
            dataContext = new ConfigViewModel();
            this.DataContext = dataContext;
            StylesController = new StylesController(this.Resources);
            dataContext.CloseEvent += () => { this.Close(); };
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string editor =  ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            switch(editor)
            {
                case "VSCode":
                    this.dataContext.Editor = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"\\Programs\\Microsoft VS Code\\Code.exe");
                    this.dataContext.EditorArguments = "--goto {0}:{1}";
                    break;
                case "Notepad++":
                    this.dataContext.Editor = "C:\\Program Files(x86)\\Notepad++\\notepad++.exe";
                    this.dataContext.EditorArguments = "{0} -n{1}";
                    break;
            }
        }
    }
}
