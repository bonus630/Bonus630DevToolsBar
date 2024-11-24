using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WorkspaceUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
               
                var w = new MainWindow(e.Args[0]);
               // w.Show();
                w.StartMonitor();
               

            }
            else
            {
                var w = new MainWindow( @"C:\Users\bonus\OneDrive\Ambiente de Trabalho\i.it");
               // w.Show();
                w.StartMonitor();
               

            }
        }
    }
}
