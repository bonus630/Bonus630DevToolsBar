using System.IO;
using System.Windows;
using System.Windows.Controls;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for XSLTEster.xaml
    /// </summary>
    public partial class XSLTEster : UserControl
    {
        XSLTesterViewModel xSLTesterViewModel;
        public StylesController StylesController { get; set; }
        public XSLTEster(Core core)
        {
            InitializeComponent();
            xSLTesterViewModel = new XSLTesterViewModel(core);
            this.DataContext = xSLTesterViewModel;
            StylesController = new StylesController(this.Resources);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(xSLTesterViewModel.xslFile))
            {
                //txt_xsl.Text = File.ReadAllText(xSLTesterViewModel.xslFile);
                xSLTesterViewModel.XslText = File.ReadAllText(xSLTesterViewModel.xslFile);
            }
            if (File.Exists(xSLTesterViewModel.xmlfile))
            {
                xSLTesterViewModel.XmlText = File.ReadAllText(xSLTesterViewModel.xmlfile);
                //txt_xml.Text = File.ReadAllText(xSLTesterViewModel.xmlfile);
            }
        }


    }
}
