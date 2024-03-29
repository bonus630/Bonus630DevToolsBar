using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for XSLTEster.xaml
    /// </summary>
    public partial class XSLTEster : UserControl
    {
        XSLTesterViewModel xSLTesterViewModel;
        public StylesController StylesController { get; set; }
        public XSLTEster(Core core)
        {
            InitializeComponent();
            xSLTesterViewModel = new XSLTesterViewModel(core);
            this.DataContext = xSLTesterViewModel;
            StylesController = new StylesController(this.Resources);
            xSLTesterViewModel.AddMoreText += XSLTesterViewModel_AddMoreText;
        }

        private void btn_format_Click(object sender, RoutedEventArgs e)
        {
            Format();
        }
        private void XSLTesterViewModel_AddMoreText(string obj)
        {
            string text = txt_xml.Text.Trim('\n','\r','\t');
            //int line = txt_xml.GetLineIndexFromCharacterIndex(txt_xml.CaretIndex);
            //int lineLength = txt_xml.GetLineLength(line);

            txt_xml.Text = xSLTesterViewModel.Core.FormatXml(text.Insert(txt_xml.CaretIndex, obj));

        }
        private void Format()
        {
            txt_xml.Text = xSLTesterViewModel.Core.FormatXml(txt_xml.Text);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(xSLTesterViewModel.xslFile))
            {
                //txt_xsl.Text = File.ReadAllText(xSLTesterViewModel.xslFile);
                xSLTesterViewModel.XslText = File.ReadAllText(xSLTesterViewModel.xslFile);
            }
            if (File.Exists(xSLTesterViewModel.xmlfile))
            {
                xSLTesterViewModel.XmlText = File.ReadAllText(xSLTesterViewModel.xmlfile);
                //txt_xml.Text = File.ReadAllText(xSLTesterViewModel.xmlfile);
            }
        }

        private void menuItem_Copy_Click(object sender, RoutedEventArgs e)
        {
            TextBox textbox = ((((sender as MenuItem).Parent as ContextMenu).Parent as System.Windows.Controls.Primitives.Popup).PlacementTarget as TextBox);
            Clipboard.SetText(textbox.SelectedText);
           
        }

        private void menuItem_Past_Click(object sender, RoutedEventArgs e)
        {
            TextBox textbox = ((((sender as MenuItem).Parent as ContextMenu).Parent as System.Windows.Controls.Primitives.Popup).PlacementTarget as TextBox);
             textbox.SelectedText = Clipboard.GetText();
        }

        private void menuItem_Guid_Click(object sender, RoutedEventArgs e)
        {
            TextBox textbox = ((((sender as MenuItem).Parent as ContextMenu).Parent as System.Windows.Controls.Primitives.Popup).PlacementTarget as TextBox);
            textbox.SelectedText = Guid.NewGuid().ToString();
            
        }

        private void menuItem_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ((((sender as MenuItem).Parent as ContextMenu).Parent as System.Windows.Controls.Primitives.Popup).PlacementTarget as TextBox).Clear();
          
        }
    }
}
