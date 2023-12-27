using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Views;

namespace VisualTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XMLTagWindow w = new XMLTagWindow(@"C:\Users\bonus\AppData\Roaming\Corel\CorelDraw Graphics Suite X8\Draw\Workspace\lite.cdws");
            w.WindowState = WindowState.Normal;
            w.WindowStartupLocation = WindowStartupLocation.Manual;
            w.Left = 1080;
            w.Width = 840;
            w.Show();   
            w.ChangeTheme("theme_Black");
        }
    }
}
