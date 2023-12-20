using System;
using System.Globalization;
using System.Windows.Data;
namespace br.com.Bonus630DevToolsBar.Converters
{
    public class TypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter is Type targetTypeParameter)
            {
                try
                {
                    // Tenta converter o valor para o tipo especificado
                    object convertedValue = System.Convert.ChangeType(value, targetTypeParameter);
                    return convertedValue;
                }
                catch (Exception ex)
                {
                    // Lida com exceções, por exemplo, valor não pode ser convertido para o tipo especificado
                    Console.WriteLine($"Erro na conversão: {ex.Message}");
                }
            }

            // Retorna um valor padrão se a conversão falhar
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
