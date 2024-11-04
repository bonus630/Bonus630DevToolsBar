using Bonus630DevToolsBar;
using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;

//using System.Windows.Forms;


namespace br.com.Bonus630DevToolsBar.DataSource
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("0c820c68-e9cc-4ca7-84bb-8f8d3768a214")]
    //[ClassInterface(ClassInterfaceType.None)]
    public class Bonus630DevToolsBarDataSource : BaseDataSource
    {

        private string caption = "Bonus630 Dev ToolsBar";
        private string cqlCaption = "Enter to Run";
        private string cqlTooltip = "Enter to Run";
        private string foldersCaption = "Open a folder";
        private string foldersTooltip = "Open a folder";
        private string reopenDocumentCaption = "Reopen Active Document";


        private readonly string runCommandGuid = "5087687d-337d-4d0e-acaf-c0b1df967757";
        private readonly string shortcutsCommandGuid = "512194ae-a540-4979-8991-bcadded6726e";

        public Bonus630DevToolsBarDataSource(DataSourceProxy proxy, Corel.Interop.VGCore.Application corelApp) : base(proxy, corelApp)
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
            set
            {
                cqlSucessedList = value;
                NotifyPropertyChanged();
            }
        }
        private string cqlTempText = "";

        public string CQLTempText
        {
            get { return cqlTempText; }
            set
            {
                cqlTempText = value;
                NotifyPropertyChanged();
            }
        }
        private int cqlContext = 0;

        public int CQLContext
        {
            get { return cqlContext; }
            set
            {
                cqlContext = value;
                NotifyPropertyChanged();
            }
        }//testado
        private bool shortcutDockerFirstUse = true;

        public bool ShortcutDockerFirstUse
        {
            get { return shortcutDockerFirstUse; }
            set
            {
                shortcutDockerFirstUse = value;
                NotifyPropertyChanged();
            }
        }

        public string ReOpenDocumentCaption
        {
            get { return reopenDocumentCaption; }
            set
            {
                reopenDocumentCaption = value;
                NotifyPropertyChanged();
            }
        }
        public string XmlItems
        {
            get
            {
                //"<item  guidRef=\"1752053c-096c-4534-90b2-af8b101abac3\" />"; 

                return "<placeholderData> " +
                    "<itemData guid=\"1752053c-096c-4534-90b2-af8b101abac3\" />" +

                    " </placeholderData>";
            }
            set {; }
        }
        private void CloseDocker(string guid)
        {
            CorelApp.FrameWork.HideDocker(guid);
        }
        public void RunCommandDocker()
        {
            CloseDocker(runCommandGuid);
            CorelApp.FrameWork.ShowDocker(runCommandGuid);
        }
        public void RunShortcutsDocker()
        {
            CloseDocker(shortcutsCommandGuid);
            CorelApp.FrameWork.ShowDocker(shortcutsCommandGuid);
        }
        public void RunDrawUIExplorer()
        {
            if (Keyboard.IsKeyToggled(Key.LeftCtrl))
            {
                try
                {
                    string path = Path.Combine(CorelApp.AddonPath,
                    ControlUI.AddonFolderName, "DrawUIExplorer.exe");
                    ProcessStartInfo p = new ProcessStartInfo();
                    p.Arguments = CorelApp.VersionMajor.ToString();
                    p.FileName = path;
                    System.Diagnostics.Process.Start(p);
                }
                catch { }
            }
            else
            {
                DrawUIExplorer.Views.XMLTagWindow xMLTagsForm = new DrawUIExplorer.Views.XMLTagWindow(CorelApp, "");
                xMLTagsForm.Closed += (s, e) => { xMLTagsForm = null; };
                WindowInteropHelper helper = new WindowInteropHelper(xMLTagsForm);
                helper.Owner = new IntPtr(CorelApp.AppWindow.Handle);
                xMLTagsForm.Show();
            }

        }

        public void RunIconCreatorHelper()
        {
            CorelApp.FrameWork.ShowDocker("488c069a-7535-4af9-9c88-eda17c4808f7");
            // CorelApp.FrameWork.Automation.InvokeItem("488c069a-7535-4af9-9c88-eda17c4808f7");
        }
        public void UnloadNDeleteUserGMS()
        {
            string path = CorelApp.GMSManager.UserGMSPath;
            GMSProjects gp = CorelApp.GMSManager.Projects;

            while (gp.Count > 0)
            {
                gp[1].Unload();

            }
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                try

                {
                    if (files[i].ToLower().EndsWith(".gms"))
                        File.Move(files[i], files[i] + ".bak");
                }
                catch
                {

                }
            }

        }
        public void ReloadNRestoreUserGMS()
        {
            string path = CorelApp.GMSManager.UserGMSPath;

            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                try

                {
                    if (files[i].EndsWith(".bak"))
                        File.Move(files[i], files[i].Replace(".bak", ""));
                }
                catch
                {

                }
            }
            GMSProjects gp = CorelApp.GMSManager.Projects;
            files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                try

                {
                    if (files[i].ToLower().EndsWith(".gms"))

                        gp.Load(files[i], false, false);
                }
                catch
                {

                }
            }

        }
        public void OpenCQLGuide()
        {

            try
            {
                string path = Path.Combine(CorelApp.AddonPath,
                    ControlUI.AddonFolderName, "CQLReference.pdf");

                System.Diagnostics.Process.Start(path);
            }
            catch { }
        }
        public void RunCommandBarBuilder()
        {
            if (Keyboard.IsKeyToggled(Key.LeftCtrl))
            {
                try
                {
                    string path = Path.Combine(CorelApp.AddonPath,
                    ControlUI.AddonFolderName, "CDRCommandBarBuilder.exe");
                    System.Diagnostics.Process.Start(path);
                }
                catch { }
            }
            else
            {
                try
                {
                    string path = Path.Combine(CorelApp.AddonPath,
                    ControlUI.AddonFolderName, "CDRCommandBarBuilder.exe");
                    ProcessStartInfo p = new ProcessStartInfo();
                    p.Arguments = CorelApp.VersionMajor.ToString();
                    p.FileName = path;
                    System.Diagnostics.Process.Start(p);
                }
                catch { }

            }

        }
        public void RunGuidGen()
        {
            try
            {
                string path = Path.Combine(CorelApp.AddonPath,
               ControlUI.AddonFolderName, "GuidGen.exe");

                System.Diagnostics.Process.Start(path);
            }
            catch { }
        }
        public void LoadIcon()
        {
            //svg não funciona
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
#if X7
            of.Filter = "ico file (*.ico)|*.ico";
            of.Title = "Select a ico file";
#else
            of.Filter = "Image files (*.bmp, *.jpg, *.ico,*.png,*.gif)|*.bmp;*.jpg;*.ico;*.png;*.gif";
            of.Title = "Select a image file";
#endif
            of.Multiselect = false;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var commandBar = CorelApp.FrameWork.CommandBars["Bonus630 Dev Tools"];

                foreach (Corel.Interop.VGCore.Control item in commandBar.Controls)
                {
                    if (item.ID.Equals("657042cb-3594-43a1-80bf-c8a27fd43146"))
                        item.SetIcon2(of.FileName);
                }
            }
        }
        public void PrintScreen()
        {
            string path = Path.GetTempPath();
            string file = "untitled";

            if (CorelApp.ActiveDocument != null && !CorelApp.ActiveDocument.Dirty)
            {
                path = CorelApp.ActiveDocument.FilePath;
                file = CorelApp.ActiveDocument.FileName;
            }
            PrintScreen.Cropper cropper = new PrintScreen.Cropper(CorelApp);
            cropper.SetFolderPath(file, path);
            cropper.Show();
        }

        public void CallFolders()
        {
            CorelApp.FrameWork.Automation.InvokeItem("d13b83a4-3ef6-4ead-b95d-44d467dc47f5");
        }
        public void CallCQL()
        {
            CorelApp.FrameWork.Automation.InvokeItem("8a8ca94c-cc61-4f14-b24d-cbd447d2fd56");
        }
        public void CallGMSReloader()
        {
            CorelApp.FrameWork.Automation.InvokeItem("b0a4b2ff-7bf5-47c3-a92a-16e2a4520746");
        }
        public void ReOpenDocument()
        {
            try
            {
                if (CorelApp.ActiveDocument.Dirty)
                {
                    double x = 0, y = 0, w = 0, h = 0;
                    int activePage = 1;
                    List<int> staticID = new List<int>();
                    for (int i = 1; i <= CorelApp.ActiveSelection.Shapes.Count; i++)
                    {
                        int id = CorelApp.ActiveSelection.Shapes[i].StaticID;
                        if (!staticID.Contains(id))
                            staticID.Add(id);
                    }
                    CorelApp.ActiveWindow.ActiveView.GetViewArea(out x, out y, out w, out h);
                    activePage = CorelApp.ActivePage.Index;
                    string path = CorelApp.ActiveDocument.FullFileName;

                    CorelApp.BeginDraw();
                    CorelApp.ActiveDocument.Close();
                    CorelApp.OpenDocument(path);
                    if (CorelApp.ActiveDocument.Pages.Count >= activePage)
                        CorelApp.ActiveDocument.Pages[activePage].Activate();
                    CorelApp.ActiveWindow.ActiveView.SetViewArea(x, y, w, h);
                    try
                    {
                        for (int i = 0; i < staticID.Count; i++)
                        {
                            Shape n = CorelApp.ActivePage.Shapes.FindShape(StaticID: staticID[i]);
                            if (n != null)
                                n.AddToSelection();
                        }
                    }
                    catch { }
                    CorelApp.EndDraw();
                }
            }
            catch { }
        }

    }

}
