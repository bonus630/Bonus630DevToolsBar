using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace br.com.Bonus630DevToolsBar.CustomControls
{
    /// <summary>
    /// Interaction logic for LockerCheckBox.xaml
    /// </summary>
    public partial class IconCheckBox : CheckBox
    {
        public static readonly DependencyProperty IconCheckedProperty = DependencyProperty.Register("IconChecked",typeof(string), typeof(IconCheckBox), null);
        public static readonly DependencyProperty IconUncheckedProperty = DependencyProperty.Register("IconUnchecked", typeof(string), typeof(IconCheckBox), null);

        [Browsable(true)]
        [Category("Icons")]
        public string IconChecked
        {
            get
            {
                return (string)GetValue(IconCheckedProperty);
            }
            set
            {
                SetValue(IconCheckedProperty, value);
            }
        }
        [Browsable(true)]
        [Category("Icons")]
        public string IconUnchecked
        {
            get
            {
                return (string)GetValue(IconUncheckedProperty);
            }
            set
            {
                SetValue(IconUncheckedProperty, value);
            }
        }


        public IconCheckBox()
        {
            InitializeComponent();
        }
    }
}
