﻿using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    /// <summary>
    /// Interaction logic for CommandManager.xaml
    /// </summary>
    public partial class CommandManager : Window
    {
        public StylesController stylesController;
        string theme = string.Empty;
        public Status GetStatus { get; protected set; }
        public new Status  DialogResult{get;set;}
        public CommandManager(MacrosManager macrosManager, string theme)
        {
            InitializeComponent();
            this.DataContext = macrosManager;
            stylesController = new StylesController(this.Resources);
            this.theme = theme;
            this.Loaded += CommandManager_Loaded;
            this.DialogResult = Status.Cancel;
        }

        private void CommandManager_Loaded(object sender, RoutedEventArgs e)
        {
            stylesController.LoadStyle(theme);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = Status.CreateCommandBar;
            this.Close();
        }

        private void btn_copy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = Status.Copy;
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = Status.Cancel;
            this.Close();
        }
        public new Status ShowDialog()
        {
            base.ShowDialog();
            return this.DialogResult;
        }
    }
    public enum Status
    {
        CreateCommandBar,
        Copy,
        Cancel
    }
}
