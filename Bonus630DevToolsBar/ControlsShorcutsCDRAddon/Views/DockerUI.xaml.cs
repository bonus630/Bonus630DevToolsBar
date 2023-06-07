using br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.ViewModels;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon
{
    public partial class DockerUI : UserControl
    {
        private corel.Application corelApp;
        private StylesController stylesController;
        private DockerUIViewModel dataContext;
        public DockerUI(object app)
        {
            InitializeComponent();
            try
            {
                this.corelApp = app as corel.Application;
                stylesController = new StylesController(this.Resources, this.corelApp);
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
            dataContext = new DockerUIViewModel(this.corelApp);
            this.DataContext = dataContext;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            stylesController.LoadThemeFromPreference();
        }
    }
}
