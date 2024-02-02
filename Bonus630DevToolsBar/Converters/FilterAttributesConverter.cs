using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace br.com.Bonus630DevToolsBar.Converters
{
    [ValueConversion(typeof(List<DrawUIExplorer.DataClass.Attribute>), typeof(List<DrawUIExplorer.DataClass.Attribute>))]
    public class FilterAttributesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                return (value as List<DrawUIExplorer.DataClass.Attribute>).FindAll(att => att.Name != "captionRef" && att.Name != "guid" && att.Name != "guidRef" && att.IsGuid);
           
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
