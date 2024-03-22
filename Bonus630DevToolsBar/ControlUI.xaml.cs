using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;

using System.Windows.Interop;
using Microsoft.Win32;
using System.Xml.Linq;

namespace br.com.Bonus630DevToolsBar
{

    public partial class ControlUI : UserControl
    { 
        // public  corel.Application app = null;
        public static  corel.Application corelApp = null;
        

        public static IntPtr corelHandle;
        
        private string currentTheme;

        public const string DataSourceName = "Bonus630DevToolsBarDS";
        public const string AddonFolderName = "Bonus630DevToolsBar";
        
        public ControlUI(object app)
        {
            
            try
            {
                corelApp = app as corel.Application;
                //corelApp = this.app;
                corelHandle = new IntPtr(corelApp.AppWindow.Handle);
                var dsf = new DataSource.DataSourceFactory(corelApp);
                dsf.AddDataSource(DataSourceName, typeof(DataSource.Bonus630DevToolsBarDataSource));
                dsf.Register();

               // var dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(DataSourceName);
               // dsp.SetProperty("CorelApp", corelApp);

            }
            catch (Exception)
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }

        }
   
        public void CallXMLForm(string filePath)
        {
            DrawUIExplorer.Views.XMLTagWindow xMLTagsForm = new DrawUIExplorer.Views.XMLTagWindow(corelApp, filePath);
            xMLTagsForm.Closed += (s, e) => { xMLTagsForm = null; };
            WindowInteropHelper helper = new WindowInteropHelper(xMLTagsForm);
            helper.Owner = corelHandle;
            xMLTagsForm.Show();
        }

    }
   
}
