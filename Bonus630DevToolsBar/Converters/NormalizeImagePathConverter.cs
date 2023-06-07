using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class NormalizeImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return string.Format("D:\\C# Ed26-08-22\\Bonus630DevToolsBar\\Bonus630DevToolsBar\\Images\\{0}", value);
            }
            else
            {
                string path = ControlUI.corelApp.AddonPath;
                return string.Format("{0}Bonus630DevToolsBar\\Images\\{1}", path, value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }


    }
}
