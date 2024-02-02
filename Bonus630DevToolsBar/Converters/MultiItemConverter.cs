using br.com.Bonus630DevToolsBar.DrawUIExplorer;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace br.com.Bonus630DevToolsBar.Converters
{
    public class MultiItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var staticItems = values[1] as IEnumerable;
            var dynamicItems = values[0] as IEnumerable;// List<DrawUIExplorer.DataClass.Attribute>;

            CompositeCollection compositeCollection = new CompositeCollection();

            if (staticItems != null)
                compositeCollection.Add(new CollectionContainer { Collection = staticItems });

            if (dynamicItems != null)
            {
                //for (int i = 0; i < dynamicItems.Count; i++)
                //{
                //    if (dynamicItems[i].Name != "captionRef" && dynamicItems[i].Name != "guid" && dynamicItems[i].Name != "guidRef" && dynamicItems[i].IsGuid)
                //    {
                //        MenuItem menuItem = new MenuItem();
                //        menuItem.Header = string.Format("Find {0}", dynamicItems[i].Name);
                //        menuItem.Tag = dynamicItems[i].Name;
                //        menuItem.CommandParameter = dynamicItems[i];
                //        menuItem.com
                //    }
                //}

                compositeCollection.Add(new CollectionContainer { Collection = dynamicItems });
            }

            return compositeCollection;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


