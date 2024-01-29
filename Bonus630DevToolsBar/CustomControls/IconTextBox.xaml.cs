using Corel.Interop.VGCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace br.com.Bonus630DevToolsBar.CustomControls
{
    /// <summary>
    /// Interaction logic for IconTextBox.xaml
    /// </summary>
    public partial class IconTextBox : TextBox
    {

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(IconTextBox), null);
        [Browsable(true)]
        [Category("Common")]
        public string Icon
        {
            get
            {
                return (string)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }
        public IconTextBox()
        {
            InitializeComponent();
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(IconControl), new FrameworkPropertyMetadata(typeof(IconControl)));
        }
    }
}
