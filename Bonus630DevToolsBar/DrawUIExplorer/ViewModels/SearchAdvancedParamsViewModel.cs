using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    public class SearchAdvancedParamsViewModel : ViewModelBase, System.Windows.Data.IValueConverter
    {
        private Action<IBasicData, string, bool> searchAction;

        public Action<IBasicData, string, bool> SearchAction
        {
            get { return searchAction; }
            set { searchAction = value; }
        }

        private IBasicData basicData;

        public IBasicData SearchBasicData
        {
            get { return basicData; }
            set { basicData = value; }
        }

        private string searchParam;

        public string SearchParam
        {
            get { return searchParam; }
            set { searchParam = value; }
        }
        private string condition;

        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }
        private bool unique;

        public bool IsUnique
        {
            get { return unique; }
            set { unique = value; }
        }
        private bool enable = true;

        

        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;
                OnPropertyChanged();
               // DisableButtonImage = null;
            }
        }

        public string DisableButtonImage
        {
            get
            {
                if (this.enable)
                    return "visibleEnable.bmp";
                else
                    return "visibleDisable.bmp";
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public SearchAdvancedParamsViewModel(Action<IBasicData, string, bool> action, IBasicData basic, string param, bool unique = false)
        {
            this.searchAction = action;
            this.basicData = basic;
            this.searchParam = param;
            this.unique = unique;
        }
        public SearchAdvancedParamsViewModel()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
     

      
    }
}
