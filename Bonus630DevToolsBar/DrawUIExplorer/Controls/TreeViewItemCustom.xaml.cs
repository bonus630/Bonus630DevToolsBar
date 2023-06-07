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

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Controls
{
    /// <summary>
    /// Interaction logic for TreeViewItemCustom.xaml
    /// </summary>
    public partial class TreeViewItemCustom : TreeViewItem
    {
        public TreeViewItemCustom()
        {
            InitializeComponent();
            //this.DataContext = this;
        }
        //private DataClass.IBasicData data;
        //public DataClass.IBasicData Data
        //{
        //    get { return this.data; }
        //    set
        //    {
        //        this.data = value;
        //        this.Header = value.TagName;
        //        this.data.SelectedEvent += Data_SelectedEvent;
        //    }
        //}
       
        //private void Data_SelectedEvent(bool isSelected, bool? isExpands, bool update)
        //{
        //    if (update)
        //    {
        //        this.IsSelected = isSelected;
        //        if (isExpands != null)
        //            this.IsExpanded = (bool)isExpands;
        //        this.data.SetSelected(false, isExpands, false);
        //    }
        //}
        //protected override void OnSelected(RoutedEventArgs e)
        //{
        //    base.OnSelected(e);
        //    this.data.SetSelected(true, this.IsExpanded, true);
        //}

    }
}
