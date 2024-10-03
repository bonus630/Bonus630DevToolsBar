using br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.Models;
using br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.ViewModels;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using corel = Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon
{
    public partial class DockerUI : UserControl
    {
        private corel.Application corelApp;
        private StylesController stylesController;
        private DockerUIViewModel dataContext;
        
        //public static readonly DependencyProperty testandoProperty = DependencyProperty.Register("Testando", typeof(int), typeof(ControlsShorcutsCDRAddon.DockerUI), new FrameworkPropertyMetadata(100));

        //public delegate void InvokeEvent();
        //public InvokeEvent OnTeste;

        //public int Testando
        //{
        //    get { return (int)GetValue(testandoProperty); }
        //    set { SetValue(testandoProperty, value); }
        //}
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
        private void ShowMessage()
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }

        private void lv_shortcuts_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Obtenha o item selecionado no ListView
                var selectedItems = lv_shortcuts.SelectedItems;
                if(selectedItems.Count > 0)
                    this.dataContext.InvokeItem(selectedItems[0] as Shortcut);

            }
        }
    }
}
