using br.com.Bonus630DevToolsBar.RunCommandDocker;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace br.com.Bonus630DevToolsBar.CustomControls
{
    /// <summary>
    /// Interaction logic for ParamControl.xaml
    /// </summary>
    public partial class ParamControl : UserControl, INotifyPropertyChanged
    {
        //Temos um bug, ao teclar enter para executar a ação o valor nao foi atualizado ainda, será que foi corrigido!!!
        public static readonly DependencyProperty ParamValueProperty = DependencyProperty.Register("ParamValue", typeof(object), typeof(ParamControl), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnParamValueChanged));
        public object ParamValue
        {
            get { return GetValue(ParamValueProperty); }
            set{ SetValue(ParamValueProperty, value); }
        }
        public static void OnParamValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ParamControl)d).ParamValue = e.NewValue;
            ((ParamControl)d).ChangeParam();
        }
        public static readonly DependencyProperty ParamTypeProperty = DependencyProperty.Register("ParamType", typeof(Type), typeof(ParamControl), new FrameworkPropertyMetadata(default(Type), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnParamTypeChanged));
        public Type ParamType
        {
            get { return (Type)GetValue(ParamTypeProperty); }
            set { SetValue(ParamTypeProperty, value); }
        }  
        public static void OnParamTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ParamControl)d).ParamType = (Type)e.NewValue;
            ((ParamControl)d).ChangeParam();
        }
        public static readonly DependencyProperty ParamOptionsProperty = DependencyProperty.Register("ParamOptions", typeof(ObservableCollection<object>), typeof(ParamControl), new FrameworkPropertyMetadata(default(ObservableCollection<object>), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnParamOptionsChanged));
        public ObservableCollection<object> ParamOptions
        {
            get { return (ObservableCollection<object>)GetValue(ParamOptionsProperty); }
            set { SetValue(ParamOptionsProperty, value); }
        }
        public static void OnParamOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ParamControl)d).ParamOptions = (ObservableCollection<object>)e.NewValue;
            ((ParamControl)d).ChangeParam();
        }
        private bool isFuncParam = false;
        public bool IsFunc
        {
            get { return isFuncParam; }
            set { isFuncParam = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isBoolean;

        public bool IsBoolean
        {
            get { return isBoolean; }
            set { isBoolean = value; OnPropertyChanged(); }
        }
        private bool isVariant;

        public bool IsVariant
        {
            get { return isVariant; }
            set { isVariant = value; OnPropertyChanged(); }
        }
        private bool isInteger;

        public bool IsInteger
        {
            get { return isInteger; }
            set { isInteger = value; OnPropertyChanged(); }
        }
        private bool isDecimal;

        public bool IsDecimal
        {
            get { return isDecimal; }
            set { isDecimal = value; OnPropertyChanged(); }
        }  
        private bool isEnum;

        public bool IsEnum
        {
            get { return isEnum; }
            set { isEnum = value; OnPropertyChanged(); }
        }
        public ParamControl()
        {
            InitializeComponent();
            txt_paramValue.TextChanged += Txt_paramValue_TextChanged;
            numeric_paramValue.ValueChangedEvent += Numeric_paramValue_ValueChangedEvent;
            decimal_paramValue.ValueChangedEvent += Numeric_paramValue_ValueChangedEvent;
        }

        private void Numeric_paramValue_ValueChangedEvent(double obj)
        {
            if(IsDecimal || IsInteger)
                ParamValue = obj;
        }

        private void Txt_paramValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_paramValue.Text.Length == 0 && IsVariant)
            {
                this.ParamValue = null;
                ChangeParam();
                IsFunc = false;

            }
        }

        private void ChangeParam()
        {
            IsVariant = false;
            IsInteger = false;
            IsBoolean = false;
            IsDecimal = false;
            IsEnum = false;
            if (this.ParamValue is FuncToParam && !isFuncParam)
            {
                IsVariant = true;
                IsFunc = true;
                return;
            }

            if (this.ParamType == null || this.ParamType.Equals(typeof(DBNull)))
            {
                IsVariant = true;
                return;
            }
            if(this.ParamType.IsEnum)
            {
                IsEnum = true;
                return;
            }
            if (this.ParamType.Equals(typeof(string)))
            {
                IsVariant = true;
                return;
            }
            if (this.ParamType.Equals(typeof(bool)))
            {
                IsBoolean = true;
                return;
            }
            if (IsNumericType(this.ParamType))
            {
                IsInteger = true;
                return;
            }
            if (IsDecimalType(this.ParamType))
            {
                IsDecimal = true;

                return;
            }
            if (this.ParamType.IsValueType)
            {
                IsVariant = true;
                return;
            }



            IsVariant = true;
        }
        private void ChangeParam2()
        {
            if (this.ParamValue is FuncToParam && !isFuncParam)
            {
                txt_paramValue.Text = (this.ParamValue as FuncToParam).Name;
                txt_paramValue.Visibility = Visibility.Visible;
                numeric_paramValue.Visibility = Visibility.Collapsed;
                cb_paramValue.Visibility = Visibility.Collapsed;
                isFuncParam = true;
                return;
            }

            if (this.ParamType.Equals(typeof(DBNull)))
            {
                txt_paramValue.Text = "";
                return;
            }
            if (this.ParamType.Equals(typeof(string)))
            {
                txt_paramValue.Text = GetValue<string>();
                return;
            }
            if (this.ParamType.Equals(typeof(bool)))
            {
                cb_paramValue.IsChecked = GetValue<bool>();
                cb_paramValue.Visibility = Visibility.Visible;
                txt_paramValue.Visibility = Visibility.Collapsed;
                return;
            }
            if (IsNumericType(this.ParamType))
            {
                numeric_paramValue.Visibility = Visibility.Visible;
                txt_paramValue.Visibility = Visibility.Collapsed;
                numeric_paramValue.NumericType = ControlDownNumericType._Int;
                numeric_paramValue.Text = GetValue<string>();
                return;
            }
            if (IsDecimalType(this.ParamType))
            {
                numeric_paramValue.Visibility = Visibility.Visible;
                txt_paramValue.Visibility = Visibility.Collapsed;
                numeric_paramValue.DecimalPlaces = 9;
                numeric_paramValue.NumericType = ControlDownNumericType._Double;
                numeric_paramValue.Text = GetValue<string>();

                return;
            }
            if (this.ParamType.IsValueType)
            {
                txt_paramValue.Text = GetValue<string>();
                return;
            }



            txt_paramValue.Text = GetValue<string>();
        }
        private bool IsNumericType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }
        private bool IsDecimalType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }
        private T GetValue<T>()
        {
            return this.ParamValue == null ? default(T) : (T)this.ParamValue;
        }


    }
}
