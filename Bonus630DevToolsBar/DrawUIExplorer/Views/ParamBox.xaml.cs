using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
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
    /// Interaction logic for ParamBox.xaml
    /// </summary>
    public partial class ParamBox : Window
    {
        public string Param { get { return txt_param.Text; } }
        public StylesController StylesController { get; set; }
        public ParamBox()
        {
            InitializeComponent();
            StylesController = new StylesController(this.Resources);
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public void ChangeTheme(string theme)
        {
            StylesController.LoadStyle(theme);
        }

        private void txt_param_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                this.DialogResult = true;
            }
            if(e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
        }
    }
}
