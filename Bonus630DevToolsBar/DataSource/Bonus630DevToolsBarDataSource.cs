using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace br.com.Bonus630DevToolsBar.DataSource
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Bonus630DevToolsBarDataSource : BaseDataSource
    {

        private string caption = "Bonus630 Dev ToolsBar";
        private string cqlCaption = "Enter to Run";
        private string cqlTooltip = "Enter to Run";     
        private string foldersCaption = "Open a folder";
        private string foldersTooltip = "Open a folder";


        private readonly string runCommandGuid = "5087687d-337d-4d0e-acaf-c0b1df967757";
        private readonly string shortcutsCommandGuid = "512194ae-a540-4979-8991-bcadded6726e";

        public Bonus630DevToolsBarDataSource(DataSourceProxy proxy) : base(proxy)
        {

        }

        // You can change caption/icon dynamically setting a new value here 
        //or loading the value from resource specifying the id of the caption/icon 
        public string Caption
        {
            get { return caption; }
            set { caption = value; NotifyPropertyChanged(); }
        }
        public string CqlCaption
        {
            get { return cqlCaption; }
            set { cqlCaption = value; NotifyPropertyChanged(); }
        }
        public string CqlTooltip
        {
            get { return cqlTooltip; }
            set { cqlTooltip = value; NotifyPropertyChanged(); }
        }  
        public string FoldersCaption
        {
            get { return foldersCaption; }
            set { foldersCaption = value; NotifyPropertyChanged(); }
        }
        public string FoldersTooltip
        {
            get { return foldersTooltip; }
            set { foldersTooltip = value; NotifyPropertyChanged(); }
        }

        private string cqlSucessedList = "";

        public string CQLSucessedList
        {
            get { return cqlSucessedList; }
            set { cqlSucessedList = value; NotifyPropertyChanged(); }
        }


        private void CloseDocker(string guid)
        {
            ControlUI.corelApp.FrameWork.HideDocker(guid);
        }
        public void RunCommandDocker()
        {
            CloseDocker(runCommandGuid);
            ControlUI.corelApp.FrameWork.ShowDocker(runCommandGuid);
        }
        public void RunShortcutsDocker()
        {
            CloseDocker(shortcutsCommandGuid);
            ControlUI.corelApp.FrameWork.ShowDocker(shortcutsCommandGuid);
        }
        public void RunDrawUIExplorer()
        {
            ControlUI.CallXMLForm("");
        }

        public void UnloadNDeleteUserGMS()
        {
            string path = ControlUI.corelApp.GMSManager.UserGMSPath;
            GMSProjects gp = ControlUI.corelApp.GMSManager.Projects;

            for (int i = 1; i <= gp.Count; i++)
            {
                gp[i].Unload();

            }
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                try

                {
                    File.Delete(files[i]);
                }
                catch
                {

                }
            }

        }
        public void OpenCQLGuide()
        {
            string path = Path.Combine(Path.GetTempPath(), "CQL Reference.pdf");

            try
            {
                File.WriteAllBytes(path, Properties.Resources.CQL_Reference);
                System.Diagnostics.Process.Start(path);
            }
            catch { }
        }
    }

}
