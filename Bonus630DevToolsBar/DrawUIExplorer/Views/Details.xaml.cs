using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {
        public StylesController StylesController { get; set; } 
        public Details(Core core)
        {
            InitializeComponent();
            this.DataContext = new DetailsViewModel(core);
            StylesController = new StylesController(this.Resources);
        }
        private void list_attributes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DataClass.Attribute li = (sender as ListView).SelectedItem as DataClass.Attribute;
            if (li != null)
                Clipboard.SetText(li.ToString());
        }
        private void lba_route_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string content = (sender as Label).Content.ToString();
            Clipboard.SetText(content);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = lba_route.Content.ToString();
            Clipboard.SetText(content);
        }
    }

}
