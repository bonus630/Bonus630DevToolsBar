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
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = "D:\\C# Ed26-08-22\\Bonus630DevToolsBar\\";
            if(ControlUI.corelApp!=null)
                 path = ControlUI.corelApp.AddonPath;
           path = string.Format("{0}Bonus630DevToolsBar\\Images\\{1}", path, value);
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }

    }
}
