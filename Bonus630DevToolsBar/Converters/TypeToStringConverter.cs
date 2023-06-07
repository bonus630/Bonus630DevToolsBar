using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using br.com.Bonus630DevToolsBar.RunCommandDocker;


namespace br.com.Bonus630DevToolsBar.Converters
{
    public class TypeToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            Type t = value.GetType();
            if (t.Equals(typeof(DBNull)))
                return "";
            if(t.Equals(typeof(string)))
                return value;
            if (t.IsValueType)
                return value.ToString();
            if (value is FuncToParam)
                return (value as FuncToParam).Name;
            return "NAN";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
