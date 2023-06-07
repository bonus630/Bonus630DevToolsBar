using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Controls;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer
{
    public class TreeViewItemData : TreeViewItemCustom
    {
        private DataClass.IBasicData data;
        public DataClass.IBasicData Data { get { return this.data; } set
            {
                this.data = value;
                
                createUI();
                this.data.SelectedEvent += Data_SelectedEvent;
            } }
        Button buttonUI;
        private void Data_SelectedEvent(bool isSelected,bool? isExpands,bool update)
        {
            if (update)
            {
                this.IsSelected = isSelected;
                if(isExpands != null)
                    this.IsExpanded = (bool)isExpands;
                this.data.SetSelected(false,isExpands, false);
            }
         
        }
        public bool IsCreated { get;  set; }
        protected override void OnExpanded(RoutedEventArgs e)
        {
            string btnContent = "+";
            if (this.IsExpanded)
                btnContent = "-";
            if (buttonUI != null)
                buttonUI.Content = btnContent;
            //IsCreated = true;
            base.OnExpanded(e);
        }
        protected override void OnCollapsed(RoutedEventArgs e)
        {
            string btnContent = "+";
            if (this.IsExpanded)
                btnContent = "-";

            if (buttonUI != null)
                buttonUI.Content = btnContent;
            base.OnCollapsed(e);
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            OnSelected(e);
        }
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            this.Background = Brushes.SlateGray;
            InvalidateVisual();
        }
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            this.Background = Brushes.White;
            InvalidateVisual();
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Background = Brushes.LightYellow;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.IsSelected)
                this.Background = Brushes.SlateGray;
            else
                this.Background = Brushes.Transparent;
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.Right)
                this.data.SetSelected(true,true, true);
            //if (!XMLTagWindow.inCorel)
            //{
            //    e.Handled = true;
            //    return;
            //}
            //if (e.Key == Key.Left)
            //{
            //    if(this.IsExpanded)
            //        this.data.SetSelected(true, false, true);
            //    else {
            //        if (this.data.Parent != null)
            //            this.data.Parent.SetSelected(true, false, true);
            //    }
            //}
            //if(e.Key ==  Key.Down)
            //{
            //    if(this.IsExpanded)
            //    {
            //        if (this.data.Childrens.Count > 0)
            //            this.data.Childrens[0].SetSelected(true, null, true);
            //    }
            //    else
            //    {
            //        if (this.data.XmlChildreID < this.data.Parent.Childrens.Count)
            //            this.data.Parent.Childrens[this.data.XmlChildreID].SetSelected(true, null, true);
            //        else
            //        {
            //            if (this.data.Parent.Parent.Childrens.Count > this.data.Parent.XmlChildreID-1)
            //                this.data.Parent.Parent.Childrens[this.data.Parent.XmlChildreID].SetSelected(true, null, true);
            //        }
            //    }
            //}
            //if (e.Key == Key.Up)
            //{

            //    if (this.data.XmlChildreID > 1)
            //    {
            //        this.data.Parent.Childrens[this.data.XmlChildreID - 2].SetSelected(true, null, true);


            //    }
            //    else
            //    {
            //        if (this.data.Parent != null)
            //        {

            //            TreeViewItemData treeViewItem = this.Parent as TreeViewItemData;
            //            if (treeViewItem != null)
            //            {
            //                if (treeViewItem.IsExpanded)
            //                {
            //                    //if(this.IsSelected)
            //                    //{
            //                    //      treeViewItem.data.Childrens[treeViewItem.data.Childrens.Count-1].SetSelected(true, null, true);
            //                    // }
            //                    //  else
            //                    treeViewItem.data.SetSelected(true, null, true);
            //                    //treeViewItem.data.Childrens[0].SetSelected(true, null, true);
            //                }

            //                //else
            //                //{

            //                //}
            //            }
            //        }
            //    }

                
               
            //}
            e.Handled = true;
        }
        private void createUI()
        {
            DataTemplate dt = new DataTemplate();
            // FrameworkElementFactory expander = new FrameworkElementFactory(typeof(Expander));
            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
          
            FrameworkElementFactory label = new FrameworkElementFactory(typeof(Label));
            label.SetValue(Label.ContentProperty, string.Format("{0} [{1}]", this.data.TagName, this.data.Childrens.Count));

            if (this.data.Childrens.Count > 0)
            {
                FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
                button.SetValue(Button.ContentProperty, "+");
                button.SetValue(Button.BackgroundProperty, Brushes.Transparent);
                button.SetValue(Button.BorderBrushProperty, Brushes.White);
                button.SetValue(Button.IsTabStopProperty, false);
                LengthConverter lc = new LengthConverter();
                string qualifiedDouble = "24px";

                var converted = lc.ConvertFrom(qualifiedDouble);
                button.SetValue(Button.WidthProperty, converted);
                button.SetValue(Button.HeightProperty, converted);
                button.SetValue(Button.BorderThicknessProperty, new Thickness(0));
                button.AddHandler(Button.ClickEvent, new RoutedEventHandler(ExpanderClick));
                button.AddHandler(Button.LoadedEvent, new RoutedEventHandler(LoaderBtn));
                //Create a style
                Style st = new Style();

                Trigger tg = new Trigger()
                {
                    Property = Button.IsMouseOverProperty,
                    Value = true
                //     <Trigger Property="IsMouseOver" Value="true">
                //                    <!--<Setter Property="Fill" TargetName="checkBoxFill" Value="{StaticResource OptionMark.MouseOver.Background}"/>-->
                //                    <Setter Property="Fill" TargetName="optionMark" Value="{StaticResource OptionMark.MouseOver.Glyph}"/>
                //                    <Setter Property="Fill" TargetName="indeterminateMark" Value="{StaticResource OptionMark.MouseOver.Glyph}"/>
                //                </Trigger>
                    //Binding = new Binding("CustomerId"),
                    //Value = 1
                };

                tg.Setters.Add(new Setter()
                {
                    Property = Button.BackgroundProperty,
                    Value = Brushes.Red
                });

                st.Triggers.Add(tg);
                //end style

                button.SetValue(Button.StyleProperty, st);
               
                // expander.SetValue(Expander.HeaderProperty, this.data.TagName);
                stackPanel.AppendChild(button);
            }
            stackPanel.AppendChild(label);

            this.HeaderTemplate = dt;
            dt.VisualTree = stackPanel;
            
        }
        private void LoaderBtn(object sender, RoutedEventArgs e)
        {
            if (buttonUI == null)
                buttonUI = sender as Button;
        }
        public void ExpanderClick(object sender, RoutedEventArgs e)
        {
            
            if (this.IsExpanded)
                this.IsExpanded = false;
            else
                this.data.SetSelected(true,true, true);
            string btnContent = "+";
            if (this.IsExpanded)
                btnContent = "-";
            if (buttonUI == null) 
                buttonUI = sender as Button;
            (sender as Button).Content = btnContent;

        }
        public override string ToString()
        {
            return data.ToString();
        }
    }
}
