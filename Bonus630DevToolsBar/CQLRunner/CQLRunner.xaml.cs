using br.com.Bonus630DevToolsBar.RecentFiles;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
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
using corel = Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.CQLRunner
{
    /// <summary>
    /// Interaction logic for CQLRunner.xaml
    /// </summary>
    public partial class CQLRunner : UserControl
    {

        private corel.Application corelApp;
        private StylesController stylesController;
        public CQLRunner(object app)
        {
            InitializeComponent();
            try
            {

                this.corelApp = app as corel.Application;
                stylesController = new StylesController(this.Resources, this.corelApp);
                txt_cql.KeyUp += Txt_cql_KeyUp;
               
            }
            catch
            {
                MessageBox.Show("VGCore Erro");
            }
        }

        private void Txt_cql_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
                popup_button.IsOpen = false;
            if (e.Key.Equals(Key.Enter))
                RunCQL();
        }

        private int context = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popup_button.IsOpen = !popup_button.IsOpen;
            txt_cql.Focusable = true;
        }

        private void RunCQL()
        {
            string cql = txt_cql.Text;
            if (string.IsNullOrEmpty(cql))
                return;
            
            switch(context)
            {
                case 0:
                    lba_console.Content = this.corelApp.Evaluate(cql).ToString();
                    break; 
                case 1:
                    if (this.corelApp.ActivePage != null)
                        this.corelApp.ActivePage.Shapes.FindShapes(Query: cql).CreateSelection();
                    break; 
                case 2:
                    ShapeRange sr = this.corelApp.ActiveSelectionRange;
                    if (sr != null)
                        sr.Sort(cql);
                    break; 
                case 3:
                    if (this.corelApp.ActiveShape != null)
                        lba_console.Content = this.corelApp.ActiveShape.Evaluate(cql).ToString();
                    break;

            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            context = Int32.Parse((sender as RadioButton).Tag.ToString());
        }
    }

}
