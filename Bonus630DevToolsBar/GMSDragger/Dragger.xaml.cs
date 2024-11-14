using br.com.Bonus630DevToolsBar.CustomControls;
using br.com.Bonus630DevToolsBar.Folders;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using Corel.Interop.VGCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using f = System.Windows.Forms;
using c = Corel.Interop.VGCore;



namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    /// <summary>
    /// Interaction logic for Dragger.xaml
    /// </summary>
    public partial class Dragger : Button
    {
        c.Application corelApp;

        string gmsPath = "";
        // List<string> resultList = new List<string>();
        public readonly string VBAEditorGuid = "28e16db6-6339-440d-af0d-f58ac27c115d";



        public Dragger(object app)
        {
                this.corelApp = app as c.Application;
        

            InitializeComponent();
            try
            {
                gmsPath = corelApp.GMSManager.UserGMSPath;
                this.Loaded += Dragger_Loaded;
                this.corelApp.OnApplicationEvent += CorelApp_OnApplicationEvent;
                //this.SizeChanged += (s, e) => { 
                //    if(e.NewSize.Height > 256)
                //    {
                //        this.Width = 256;
                //        this.Height = 256;
                //    }

                //    Console.WriteLine(e.NewSize.ToString());
                //};
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
        }

        private void Dragger_Loaded(object sender, RoutedEventArgs e)
        {
            LoadThemeFromPreference();
            //teste remover
            // InstallThis(new string[] { @"C:\Users\bonus\OneDrive\Ambiente de Trabalho\Power Resize 1-03 2022.rar" });
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragEnter(e);
        }
        protected override void OnDrop(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                bool remove = e.KeyStates == (DragDropKeyStates.ControlKey | DragDropKeyStates.ShiftKey);
                if (remove)
                {
                    processFiles(files, remove);
                    return;
                }
                if(e.KeyStates==DragDropKeyStates.ControlKey)
                {
                    processFiles(files, remove);
                    return;
                }
                InstallThis(files);
                //  if (e.KeyStates == (DragDropKeyStates.ControlKey | DragDropKeyStates.ShiftKey | DragDropKeyStates.AltKey))
                //else
                //{
                //    processFiles(files, remove);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on drop process!\nError:" + ex.Message);
            }
            base.OnDrop(e);
        }
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
                drgevent.Effects = DragDropEffects.Copy;
            base.OnDragEnter(drgevent);
        }
        private void processFiles(string[] files, bool remove = false)
        {
            string result = "";
            this.corelApp.InitializeVBA();
            bool r = false;

            for (int i = 0; i < files.Length; i++)
            {
                var project = this.corelApp.GMSManager.Projects.Load(files[i]);
                if (project.PasswordProtected)
                {
                    project.Unload();
                    if (processFile(files[i], out result, remove))
                    {
                        project = this.corelApp.GMSManager.Projects.Load(result);
                        // https://learn.microsoft.com/en-us/office/vba/language/reference/visual-basic-add-in-model/properties-visual-basic-add-in-model
                        //  project.0000000000
                        //  try
                        // {
                        //      //    System.Windows.Forms.MessageBox.Show((corelApp.VBE as dynamic).VBProjects.Count.ToString());
                        //      //    (corelApp.VBE as dynamic).ActiveVBProject.Protection = 0;
                        //      for (int j = 1; j <= (corelApp.VBE as dynamic).VBProjects.Count; j++)
                        //      {
                        //          dynamic p = (corelApp.VBE as dynamic).VBProjects[j];
                        //          if (p.FileName == files[i])
                        //          {
                        //              (corelApp.VBE as dynamic).VBProjects[j].Protection = 0;
                        //          }
                        //      }
                        //  }
                        //  catch (Exception e)
                        //{
                        //      System.Windows.Forms.MessageBox.Show(e.Message);
                        //  }
                        r = true;
                    }
                }
                else
                    r = true;
            }
            if (r)
            {
                this.corelApp.FrameWork.Automation.InvokeItem(VBAEditorGuid);
            }
        }
        private bool processFile(string arquivo, out string result, bool remove = false)
        {
            string fileResultName = arquivo.Substring(arquivo.LastIndexOf("\\") + 1, arquivo.Length - arquivo.LastIndexOf("\\") - 5) + "-Cracked.gms";
            result = string.Format("{0}\\{1}", this.gmsPath, fileResultName);
            //string dpbInitial = "DPB=\"";
            try
            {
                if (File.Exists(result))
                    File.Delete(result);
                if (!Directory.Exists(gmsPath))
                {
                    Directory.CreateDirectory(gmsPath);
                }
            }
            catch (IOException ie)
            {
                MessageBox.Show("Error on acess File!\nError:" + ie.Message);
            }
            if (File.Exists(arquivo))
            {
                try
                {
                    if (remove)
                    {
                        string toReplace = "5351FFE607030703F8FD0803BECFCA6AB7796054D8A2E61F539CAD491169CD680FA4C6D26A";
                        // string arquivo = @"D:\CDRGMS\Shaping-X64-2020.gms";
                        string dpbInitial = "DPB";

                        byte[] buffer = new byte[4096];
                        //byte[] dpbBuffer = Encoding.ASCII.GetBytes("080AEEF1EFF1EFF1");
                        byte[] dpbBuffer = Encoding.ASCII.GetBytes("DPx");
                        bool tryFind = true;
                        using (FileStream sr = new FileStream(arquivo, FileMode.Open, FileAccess.Read))
                        {

                            using (FileStream fs = new FileStream(result, FileMode.OpenOrCreate, FileAccess.Write))
                            {

                                int i = 0;
                                int table = 0;
                                while ((i = sr.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    if (tryFind)
                                    {
                                        //DPB="080AEEF1EFF1EFF1"
                                        string f = Encoding.ASCII.GetString(buffer);
                                        int index = f.IndexOf(dpbInitial);
                                        if (index > -1)
                                        {
                                            //index += dpbInitial.Length;
                                            //string p = f.Substring(index, f.Length - index);
                                            string p = f.Substring(index, 3);
                                            Console.WriteLine(p.IndexOf("\""));
                                            //string p1 = p.Substring(0, p.IndexOf("\""));
                                            string p1 = p;
                                            Console.WriteLine(p1);
                                            byte[] dpbToReplace = Encoding.ASCII.GetBytes(p1);
                                            byte[] bufferFinal = new byte[buffer.Length - index - dpbToReplace.Length];
                                            Array.Copy(buffer, index + dpbToReplace.Length, bufferFinal, 0, bufferFinal.Length);

                                            Array.Resize(ref buffer, buffer.Length - dpbToReplace.Length + dpbBuffer.Length);

                                            Array.Copy(dpbBuffer, 0, buffer, index, dpbBuffer.Length);

                                            Array.Copy(bufferFinal, 0, buffer, index + dpbBuffer.Length, bufferFinal.Length);




                                            Console.WriteLine("----------------------- index:" + index + " table:" + table);
                                            tryFind = false;
                                        }
                                    }
                                    fs.Write(buffer, 0, buffer.Length);
                                    table++;
                                }
                            }
                        }
                        return true;
                    }
                    else
                        File.Copy(arquivo, string.Format("{0}\\{1}", this.gmsPath, arquivo.Substring(arquivo.LastIndexOf("\\") + 1, arquivo.Length - arquivo.LastIndexOf("\\") - 1)));

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Process!\nError:" + ex.Message);
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("File not found!");
            }
            return false;
        }

        private void InstallThis(string[] files)
        {
            MacrosManager mm = new MacrosManager(this.corelApp, files, currentTheme);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //f.MessageBox.Show("Começou o teste");
            //try
            //{
            //    corelApp.AddPluginCommand("Olha so isso");
            //    // corelApp.AdviseEvents()
                

            //}
            //catch(Exception ex)
            //{
            //    f.MessageBox.Show("Exception "+ex.Message);
            //}



            //return;
            /////fim teste

            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Any File (*.*)|*.*";
            of.Title = "Select any file(s)";
            of.Multiselect = true;
            if ((bool)of.ShowDialog())
            {
                InstallThis(of.FileNames);
                //processFiles(of.FileNames);
            }
        }
        private void MenuItemUserGMS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(this.corelApp.GMSManager.UserGMSPath))
                    System.Diagnostics.Process.Start(this.corelApp.GMSManager.UserGMSPath);
            }
            catch(Exception ex)
            {
                f.MessageBox.Show(ex.Message);
            }
           
        }

        private void MenuItemGMS_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.corelApp.GMSManager.GMSPath);
        }
        #region controle style
        private void CorelApp_OnApplicationEvent(string EventName, ref object[] Parameters)
        {

            if (EventName.Equals("WorkspaceChanged") || EventName.Equals("OnColorSchemeChanged"))
            {
                LoadThemeFromPreference();
            }
            //if(EventName.Equals("FrameworkManagerToolbarListChanged"))
            //{
            //    var bar = corelApp.FrameWork.CommandBars["Bonus630 Dev Tools"];

            //    //foreach (CommandBar i in corelApp.FrameWork.CommandBars)
            //    //    Debug.WriteLine(i.Name);

            //   Debug.WriteLine(bar.Controls[1].Height.ToString());
            //    int x, y, w, h;

            //    corelApp.FrameWork.Automation.GetItemScreenRect("48024933-d18b-4e0a-9b59-96a6b99a418e", "7acb54e6-084e-494f-ad31-2718f34ddad2",out x,out y,out w,out h);
            //    Debug.WriteLine(h);
            //    var d = corelApp.FrameWork.Application.DataContext.GetDataSource("Bonus630DevToolsBarDS");
            //    d.SetProperty("TesteAltura",h);

             //7acb54e6-084e-494f-ad31-2718f34ddad2 48024933-d18b-4e0a-9b59-96a6b99a418e 

            //    this.Height = h;
            //    this.Width = h;
            //}

        }
        //Keys resources name follow the resource order to add a new value, order to works you need add 5 resources colors and Resources/Colors.xaml
        //1º is default, is the same name of StyleKeys string array
        //2º add LightestGrey. in start name of 1º for LightestGrey style in corel
        //3º MediumGrey
        //4º DarkGrey
        //5º Black
        private readonly string[] StyleKeys = new string[] {

          "ControlUI.Button.MouseOver.Background" ,
         "ControlUI.Button.MouseOver.Border",
         "ControlUI.Button.Static.Border" ,
         "ControlUI.Button.Static.Background" ,
         "ControlUI.Button.Pressed.Background" ,
         "ControlUI.Button.Pressed.Border",
          "Default.Static.Foreground" ,
         "Default.Static.Background",
         "Container.Text.Static.Background" ,
         "Container.Text.Static.Foreground" ,
         "Container.Static.Background" ,
         "Default.Static.Inverted.Foreground",
            "Button.MouseOver.Background" ,
         "Button.MouseOver.Border",
         "Button.Static.Border" ,
         "Button.Static.Background" ,
         "Button.Pressed.Background" ,
         "Button.Pressed.Border" ,
         "Button.Disabled.Foreground",
         "Button.Disabled.Background",
         "NumericTextBox.Static.Background",
        "NumericTextBox.Static.Border",
        "NumericTextBox.Selected.Border"

        };
        string currentTheme = "";
        private void LoadStyle(string name)
        {

            string style = name.Substring(name.LastIndexOf("_") + 1);
            for (int i = 0; i < StyleKeys.Length; i++)
            {
                this.Resources[StyleKeys[i]] = this.Resources[string.Format("{0}.{1}", style, StyleKeys[i])];
            }
        }
        public void LoadThemeFromPreference()
        {
            try
            {
                string result = string.Empty;
#if !X7
                result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif

                if (!result.Equals(currentTheme))
                {
                    if (!result.Equals(string.Empty))
                    {
                        currentTheme = result;
                        LoadStyle(currentTheme);
                    }
                }
            }
            catch { }
        }
        #endregion

    }


}

//public  class Form1 
//{
//    private Microsoft.Vbe.Interop.VBProject vb;



//    private void Form1_Load(object sender, EventArgs e)
//    {
//        Microsoft.Vbe.Interop.VBE vbe = Microsoft.VisualBasic.Interaction.CreateObject("VBE", "");

//        foreach (Microsoft.Vbe.Interop.VBProject vbp in vbe.VBProjects)
//        {
//            list.Items.Add(vbp.Name);
//        }
//    }

//    private void CreateDir(string path)
//    {
//        string exist = Directory.Exists(path) ? path : string.Empty;
//        if (exist == string.Empty)
//        {
//            Directory.CreateDirectory(path);
//        }
//    }

//    private void exp_Click(object sender, EventArgs e)
//    {
//        if (list.SelectedIndex == -1)
//        {
//            return;
//        }

//        Microsoft.Vbe.Interop.VBProject vbp = (Microsoft.Vbe.Interop.VBProject)vbe.VBProjects.Item(list.SelectedIndex + 1);

//        string path = CorelScriptTools.GetFolder();
//        if (path.Length == 0)
//        {
//            return;
//        }

//        if (cbIntoNewFolder.Checked)
//        {
//            path = path + "\\" + vbp.Name;
//            CreateDir(path);
//        }

//        string srcPath = path + "\\source";
//        CreateDir(srcPath);

//        foreach (Microsoft.Vbe.Interop.VBComponent vc in vbp.VBComponents)
//        {
//            string fileName = vc.Name;

//            switch (vc.Type)
//            {
//                case Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_StdModule:
//                    fileName = fileName + ".bas";
//                    break;
//                case Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_ClassModule:
//                case Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_Document:
//                    fileName = fileName + ".cls";
//                    break;
//                case Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_MSForm:
//                    fileName = fileName + ".frm";
//                    break;
//            }

//            vc.Export(srcPath + "\\" + fileName);
//        }

//        FileSystem.FileCopy(vbp.FileName, path + "\\" + vbp.Name + ".gms");
//        MessageBox.Show("Done!");
//    }
//}


