using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
//using br.com.Bonus630DevToolsBar.DrawUIExplorer.Views;

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
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Bonus630DevToolsBar.dll");
            Assembly asm = Assembly.LoadFile(path);
            Type type = asm.GetType("br.com.Bonus630DevToolsBar.DrawUIExplorer.Views.XMLTagWindow");

            dynamic instance = Activator.CreateInstance(type,new object[] {"" });

            //XMLTagWindow w = new XMLTagWindow("");
            instance.WindowState = WindowState.Normal;
            instance.WindowStartupLocation = WindowStartupLocation.Manual;
            instance.Left = 1080;
            instance.Width = 840;
            instance.Show();   
            instance.ChangeTheme("theme_Black");
        }
    }
}
