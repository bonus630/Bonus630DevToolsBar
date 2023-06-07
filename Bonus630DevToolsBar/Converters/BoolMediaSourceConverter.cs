using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    public class BoolMediaSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Properties.Resources.expandArrow.GetBitmapSource();
            if((bool)value)
                return Properties.Resources.expandArrow.GetBitmapSource();
            else
                return Properties.Resources.collapseArrow.GetBitmapSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
