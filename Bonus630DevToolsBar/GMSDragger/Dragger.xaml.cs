using Corel.Interop.VGCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


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
        List<string> resultList = new List<string>();
        private readonly string VBAEditorGuid = "28e16db6-6339-440d-af0d-f58ac27c115d";
        public Dragger(object app)
        {
            InitializeComponent();
            try
            {

                this.corelApp = app as c.Application;
                gmsPath = corelApp.GMSManager.UserGMSPath;
               ;
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
        }
        protected override void OnDragOver(DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            try
            {
                processFiles(files);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on drop process!\nError:" + ex.Message);
            }


            base.OnDragEnter(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
                drgevent.Effects = DragDropEffects.Copy;
            base.OnDragEnter(drgevent);
        }
        private void processFiles(string[] files)
        {
            string result = "";
            this.corelApp.InitializeVBA();
            bool r = false;
            for (int i = 0; i < files.Length; i++)
            {
                 
                if(processFile(files[i],out result))
                {
                    var project =  this.corelApp.GMSManager.Projects.Load(result);
                    r = true;
                }
            }
            if (r)
                this.corelApp.FrameWork.Automation.InvokeItem(VBAEditorGuid);
        }
        private bool processFile(string arquivo,out string result)
        {
            string toReplace = "5351FFE607030703F8FD0803BECFCA6AB7796054D8A2E61F539CAD491169CD680FA4C6D26A";
            // string arquivo = @"D:\CDRGMS\Shaping-X64-2020.gms";
            string fileResultName = arquivo.Substring(arquivo.LastIndexOf("\\") + 1, arquivo.Length - arquivo.LastIndexOf("\\") - 5) + "-Cracked.gms";
            result = string.Format("{0}\\{1}", this.gmsPath, fileResultName);
            //string dpbInitial = "DPB=\"";
            string dpbInitial = "DPB";
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
#if NoCrack
                    File.Copy(arquivo, string.Format("{0}\\{1}", this.gmsPath, arquivo.Substring(arquivo.LastIndexOf("\\") + 1, arquivo.Length - arquivo.LastIndexOf("\\") - 1)));
#else

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
#endif
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Select gms files";
            if ((bool)of.ShowDialog())
            {
                processFiles(of.FileNames);
            }
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
    

}
