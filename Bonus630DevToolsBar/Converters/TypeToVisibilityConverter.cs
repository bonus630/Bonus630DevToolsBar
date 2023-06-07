using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    public class TypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return System.Windows.Visibility.Collapsed;
            if(value.Equals(typeof(void)))
                return System.Windows.Visibility.Collapsed;
            else
                return System.Windows.Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return typeof(void);
        }
    }
}
