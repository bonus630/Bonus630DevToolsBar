using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ImagePathConverterD : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //return string.Format("D:\\C# Ed26-08-22\\ShapingDocker\\ShapingDocker\\bin\\Debug\\Img\\{0}.png", value);
            return string.Format("D:\\C# Ed26-08-22\\Bonus630DevToolsBar\\Bonus630DevToolsBar\\Images\\{0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }

    }
}
