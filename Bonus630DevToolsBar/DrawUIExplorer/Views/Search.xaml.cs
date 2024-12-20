﻿using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using System;
using System.Windows;
using System.Windows.Controls;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        SearchViewModel searchViewModel;
        public StylesController StylesController { get; set; }
        public Search(Core core)
        {
            InitializeComponent();
            searchViewModel = new SearchViewModel(core);
            this.DataContext = searchViewModel;
            StylesController = new StylesController(this.Resources);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            //DependencyObject el1 = VisualTreeHelper.GetParent(sender as Button);
            //DependencyObject el2 = VisualTreeHelper.GetParent(el1);
            //DependencyObject el3 = VisualTreeHelper.GetParent(el2);
            //DependencyObject el = VisualTreeHelper.GetParent(el3);

            DependencyObject el = Core.FindParentControl<ListViewItem>(sender as Button);

            searchViewModel.AdvancedSearchListAction.Remove((SearchAdvancedParamsViewModel)(el as ListViewItem).DataContext);

            //listView_tags.ItemsSource = null;
            //listView_tags.ItemsSource = this.AdvancedSearchListAction;

        }
        private void btnDisableSearchItem(object sender, RoutedEventArgs e)
        {
            DependencyObject el = Core.FindParentControl<ListViewItem>(sender as Button);

            SearchAdvancedParamsViewModel searchItem = ((SearchAdvancedParamsViewModel)(el as ListViewItem).DataContext);

            searchItem.Enable = !searchItem.Enable;

        }

        private void txt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox text = sender as TextBox;
                searchViewModel.AddParam.Execute(text.Tag);
            }
        }

        internal void StartSearchEngine()
        {
            searchViewModel.StartSeachEngine();
        }
    }
}
