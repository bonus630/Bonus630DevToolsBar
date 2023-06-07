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

namespace br.com.Bonus630DevToolsBar.RunCommandDocker.MyPopup
{
    /// <summary>
    /// Interaction logic for PopupContentxaml.xaml
    /// </summary>
    public partial class PopupContent : UserControl
    {

        public event Action<Reflected> ReflectedIsExpandedEvent;

        public PopupContent()
        {
            InitializeComponent();
        }

        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            Reflected reflected = (e.OriginalSource as TreeViewItem).Header as Reflected ;
            if (ReflectedIsExpandedEvent != null)
                ReflectedIsExpandedEvent(reflected);
        }
    }
}
