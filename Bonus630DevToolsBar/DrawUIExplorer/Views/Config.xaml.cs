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
using System.Windows.Shapes;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        ViewModels.ConfigViewModel dataContext;
        public Config()
        {
            InitializeComponent();
            dataContext = new ConfigViewModel();
            this.DataContext = dataContext;
            dataContext.CloseEvent += () => { this.Close(); };
        }
    }
}
