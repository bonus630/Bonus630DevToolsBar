using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace br.com.Bonus630DevToolsBar.CustomControls
{
    /// <summary>
    /// Interaction logic for IconButton.xaml
    /// </summary>
    public partial class IconButton : Button
    {
        

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(IconButton), null);
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
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(IconButton), null);
        [Browsable(true)]
        [Category("Appearence")]
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        public IconButton()
        {
            InitializeComponent();
        }
    }
}
