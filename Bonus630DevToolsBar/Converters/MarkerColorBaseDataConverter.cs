using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace br.com.Bonus630DevToolsBar.Converters
{
    public class MarkerColorBaseDataConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<SolidColorBrush, IBasicData>((SolidColorBrush)values[0],(IBasicData) values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
