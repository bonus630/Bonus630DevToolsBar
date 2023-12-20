using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace br.com.Bonus630DevToolsBar.CustomControls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : TextBox
    {
        char regionSymbol;
        char toReplaceSymbol;
        Regex rg;
        public NumericTextBox()
        {
            InitializeComponent();
            regionSymbol = (1.1).ToString()[1];
            if (regionSymbol == ',')
                toReplaceSymbol = '.';
            else
                toReplaceSymbol = ',';

            rg = NumericTextBox.InstantiateRegex(this.DecimalPlaces, this.NumericType);
            //this.DataContext = this;
        }
        public event Action<double> ValueChangedEvent;

        #region DependencyProperties

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(NumericTextBox), new PropertyMetadata(null));
        [Browsable(true)]
        [Category("Extras")]
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

  

        public static readonly DependencyProperty NumericTypeProperty = DependencyProperty.Register("NumericType", typeof(ControlDownNumericType), typeof(NumericTextBox), new PropertyMetadata(ControlDownNumericType._Double));
        [Browsable(true)]
        [Category("Extras")]
        public ControlDownNumericType NumericType
        {
            get { return (ControlDownNumericType)GetValue(NumericTypeProperty); }
            set { SetValue(NumericTypeProperty, value); }
        }

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(double), typeof(NumericTextBox), new PropertyMetadata(null));
        [Browsable(true)]
        [Category("Extras")]
        public double DefaultValue
        {
            get { return (double)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericTextBox), new PropertyMetadata(2,new PropertyChangedCallback(ChangedDecimalPlaces)));
        [Browsable(true)]
        [Category("Extras")]
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }
        #endregion
        public string UserInputText
        {
            get
            {
                return this.Text.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                this.Text = value;
            }
        }
        public Visibility SetVisible { set { this.Visibility = value; } }



        private double prevValue = 0;
        private double _value = 0;
        public double Value
        {
            get
            {

                double.TryParse(Text.Replace(toReplaceSymbol, regionSymbol), out _value);
                return _value;
            }
            set
            {
                _value = value;
                Text = _value.ToString();
            }
        }

        //public event EventHandler<Double> ValueChanged;


        private void restoreDefault()
        {
            this.Text = "0";
        }
        private void checkTextFormat(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = true; não continua a adição de texto
            //return;

            string text = (e.Source as TextBox).Text;
            string prevText = e.Text;
            bool stop = false;
            stop = !(rg.IsMatch(text) & rg.IsMatch(prevText));
            if (prevText == regionSymbol.ToString() || prevText == toReplaceSymbol.ToString())
            {

                stop = (text.Contains(regionSymbol) || text.Contains(toReplaceSymbol));
                if (this.NumericType == ControlDownNumericType._Int)
                    stop = true;
                if (prevText == toReplaceSymbol.ToString() && !stop)
                {
                    TextBox tb = sender as TextBox;
                    if (tb.Text.Length == 0)
                        tb.AppendText(regionSymbol.ToString());
                    int caretIndex = tb.CaretIndex;
                    tb.Text = tb.Text.Insert(caretIndex, "0");

                    tb.CaretIndex = caretIndex + 1;
                    e.Handled = true;
                    return;
                }
                if (prevText == regionSymbol.ToString() && !stop)
                {
                    TextBox tb = sender as TextBox;
                    if (tb.Text.Length == 0)
                        tb.AppendText("0");
                    int caretIndex = tb.CaretIndex;
                    tb.Text = tb.Text.Insert(caretIndex, regionSymbol.ToString());

                    tb.CaretIndex = caretIndex + 1;
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = stop;
            Debug.WriteLine("TXT:" + (sender as TextBox).Text);
        }
      
        private static void ChangedDecimalPlaces(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            NumericTextBox ntx = d as NumericTextBox;
            ntx.rg = InstantiateRegex((int)e.NewValue, ntx.NumericType);
        }

        private static Regex InstantiateRegex(int decimalPlaces,ControlDownNumericType numericType)
        {
          
            string pattern = "";
            int d = 1;
            if (decimalPlaces > d)
                d = decimalPlaces - d;
            if (numericType == ControlDownNumericType._Double)
                pattern = @"^(-)?(\d+)?([\.|,]{1})?([\d]{0," + d + "})?$";
            if (numericType == ControlDownNumericType._Int)
                pattern = @"^(-)?(\d+)?$";
            Regex rg = new Regex(pattern);
            return rg;
        }

        #region overrides

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
        private void OnValueChanged()
        {
            if (prevValue == Value)
                return;
            else
                prevValue = Value;
            if (ValueChangedEvent != null)
                ValueChangedEvent(Value);
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.Text == "0")
                this.Text = "";
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (this.Text == "")
                this.Text = "0";

        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            checkTextFormat(this, e);
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            OnValueChanged();
        }
        #endregion

        #region Callbacks
        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {


        }
        #endregion

    }
    [Flags]
    public enum ControlDownNumericType
    {
        _Double,
        _Int
    }
}

