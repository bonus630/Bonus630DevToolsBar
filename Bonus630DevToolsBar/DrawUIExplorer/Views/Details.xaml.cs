using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {
        public Details(Core core)
        {
            InitializeComponent();
            this.DataContext = new DetailsViewModel(core);
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
    }

}
