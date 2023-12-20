using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    class StringLenToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return System.Windows.Visibility.Collapsed;
            if (!string.IsNullOrEmpty(value.ToString()))
                return System.Windows.Visibility.Visible;
            else
                return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;
            if ((System.Windows.Visibility)value == System.Windows.Visibility.Collapsed)
                return "";
            else
                return "ab";
        }

    }
}
