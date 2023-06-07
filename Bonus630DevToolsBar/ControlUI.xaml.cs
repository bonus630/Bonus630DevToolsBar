using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Xml.Linq;

namespace br.com.Bonus630DevToolsBar
{

    public partial class ControlUI : UserControl
    {
        public static corel.Application corelApp = null;
        
        public static IntPtr corelHandle;
        
        private string currentTheme;
        public ControlUI(object app)
        {
            
            try
            {
                corelApp = app as corel.Application;
                corelHandle = new IntPtr(corelApp.AppWindow.Handle);
                var dsf = new DataSource.DataSourceFactory();
                dsf.AddDataSource("Bonus630DevToolsBarDS", typeof(DataSource.Bonus630DevToolsBarDataSource));
                dsf.Register();
            }
            catch (Exception)
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }

        }
   
        public static void CallXMLForm(string filePath)
        {
            DrawUIExplorer.Views.XMLTagWindow xMLTagsForm = new DrawUIExplorer.Views.XMLTagWindow(corelApp, filePath);
            xMLTagsForm.Closed += (s, e) => { xMLTagsForm = null; };
            WindowInteropHelper helper = new WindowInteropHelper(xMLTagsForm);
            helper.Owner = corelHandle;
            xMLTagsForm.Show();
        }

    }
}
