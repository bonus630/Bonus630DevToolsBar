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
        private string param = "";
        public string Param { get { return param; } }
        public StylesController StylesController { get; set; }
        public ParamBox()
        {
            InitializeComponent();
   
            
            StylesController = new StylesController(this.Resources);
            this.Loaded += ParamBox_Loaded;
        }

        private void ParamBox_Loaded(object sender, RoutedEventArgs e)
        {
            Point point = Mouse.GetPosition(this);
            Point screenPoint = PointToScreen(point);

            this.Top = screenPoint.Y - 60;
            this.Left = screenPoint.X - 60;
            txt_param.Focus();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void btn_clipboard_Click(object sender, RoutedEventArgs e)
        {
            param = Clipboard.GetText();
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

        private bool shift = false;
        private void txt_param_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftShift)
                shift = false;
            if(e.Key == Key.Enter && !shift)
            {
                this.DialogResult = true;
            }
            if (e.Key == Key.Enter && shift)
            {
                txt_param.Text += Environment.NewLine;
                txt_param.CaretIndex = txt_param.Text.Length;
            }
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
            param = txt_param.Text;
        }

        private void txt_param_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                shift = true;
        }
    }
}
