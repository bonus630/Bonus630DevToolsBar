using System;
using System.Globalization;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    public class IntAttVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return System.Windows.Visibility.Collapsed;
                if ((int)value[0] == 0)
                    return System.Windows.Visibility.Collapsed;
                if (value.Length < 2)
                    return System.Windows.Visibility.Collapsed;
                if ((value[1] as System.Windows.Controls.ItemCollection).Count == 0)
                    return System.Windows.Visibility.Collapsed;
                else
                {
                    foreach (var item in (value[1] as System.Windows.Controls.ItemCollection))
                    {
                        DrawUIExplorer.DataClass.Attribute att = (item as DrawUIExplorer.DataClass.Attribute);
                        if (att.Name != "guid" && att.IsGuid)
                            return System.Windows.Visibility.Visible;
                    }
                }
            }
            catch { }
            return System.Windows.Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return new object[] { 0, 0 };
        }
    
    
    }
}
