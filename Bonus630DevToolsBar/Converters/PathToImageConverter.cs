using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace br.com.Bonus630DevToolsBar.Converters
{
    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                try
                {
                    string path = value.ToString();
                    BitmapImage imagem = new BitmapImage();
                    imagem.BeginInit();
                    imagem.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    imagem.EndInit();
                    return imagem;
                }
                catch (Exception)
                {
                    // Tratar exceções, se necessário
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
