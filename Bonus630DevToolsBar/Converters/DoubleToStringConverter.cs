using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
   
   

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();  


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
               if (value == null)
                return value;
            char regionSymbol = (1.1).ToString()[1];
            char toReplaceSymbol = ',';
            if (regionSymbol == ',')
                toReplaceSymbol = '.';
            double val = 0;
            Double.TryParse(((string)value).Replace(toReplaceSymbol, regionSymbol), out val);
            return val;
        }

    }
}
