using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace br.com.Bonus630DevToolsBar.CustomControls
{
  
    public partial class SVGButton : Button
    {
        List<path> paths  = new List<path>();
        public List<path> Paths { get { return paths; } set { paths = value; } }
        public SVGButton()
        {
            InitializeComponent();
        }
     
    }

    public class path 
    {
        //public static readonly DependencyProperty dProperty =
        //    DependencyProperty.Register("d", typeof(System.Windows.Media.Geometry), typeof(path), new FrameworkPropertyMetadata(null));

        //public Geometry d 
        //{
        //    get { return (Geometry)GetValue(dProperty); }
        //    set { SetValue(dProperty, value); }
        //}
        public Geometry d 
        {
            get ; 
            set ; 
        }
    }
}
