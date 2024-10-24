using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Win32;
using c = Corel.Interop.VGCore;
using Corel.Interop.VGCore;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
//using System.Security.Cryptography;



namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    public abstract class MMBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MacrosManager : MMBase
    {

        c.Application corelApp;
        private ObservableCollection<string> icos;

        public ObservableCollection<string> Icos
        {
            get { return icos; }
            set { icos = value; OnPropertyChanged(); }

        }
        private ObservableCollection<MacroCommand> commands;

        public ObservableCollection<MacroCommand> Commands
        {
            get { return commands; }
            set { commands = value; OnPropertyChanged(); }

        }
        private ObservableCollection<string> commandBars;

        public ObservableCollection<string> CommandBars
        {
            get { return commandBars; }
            set { commandBars = value; OnPropertyChanged(); }

        }

        private string newCommandBar;

        public string NewCommandBar
        {
            get { return newCommandBar; }
            set { newCommandBar = value; OnPropertyChanged(); }

        }

        private string selectedCommandBar;

        public string SelectedCommandBar
        {
            get { return selectedCommandBar; }
            set { selectedCommandBar = value; OnPropertyChanged(); }

        }

        public MacrosManager(c.Application corelApp, string[] files, string theme)
        {
            try
            {
                this.corelApp = corelApp;

                corelApp.InitializeVBA();
            }
            catch
            {
                MessageBox.Show("VBA not initialized");
                return;
            }

            icos = new ObservableCollection<string>();
            commandBars = new ObservableCollection<string>();
            commands = new ObservableCollection<MacroCommand>();
            for (int i = 1; i <= corelApp.FrameWork.CommandBars.Count; i++)
            {
                CommandBars.Add(corelApp.FrameWork.CommandBars[i].Name);
            }
            processFiles2(files);

            CommandManager cm = new CommandManager(this, theme);
            Status install = cm.ShowDialog();


            if (!string.IsNullOrEmpty(NewCommandBar) && !commandBars.Contains(newCommandBar) && install.Equals(Status.CreateCommandBar))
                this.corelApp.FrameWork.CommandBars.Add(newCommandBar);
            List<string> controlsID =  new List<string>();;
            if (!install.Equals(Status.Cancel))
            { 
                for (int i = 0; i < this.Commands.Count; i++)
                {
                    if (this.commands[i].Add)
                    {
                        string name = Path.GetFileName(this.Commands[i].FilePath);
                        string dest = string.Format("{0}{1}", this.corelApp.GMSManager.UserGMSPath, name);
                        try
                        {
                            File.Copy(this.Commands[i].FilePath, dest);
                        }
                        catch { }
                        this.corelApp.GMSManager.Projects.Load(dest);
                        c.Control c = null;

                        if (install.Equals(Status.CreateCommandBar))
                        {

                            var dsp = corelApp.FrameWork.Application.DataContext.GetDataSource("MacroMgrDockerDS");
                            string xmlString = (string)dsp.GetProperty("MacroItemList");
                            if (string.IsNullOrEmpty(xmlString))
                                return;

                            XDocument xdoc = XDocument.Parse(xmlString);

                            string[] paths = this.commands[i].Command.Split(new char[] { '.' }, options: StringSplitOptions.RemoveEmptyEntries);

                            XElement resultElement = FindElementByPath(xdoc.Root, "VBA", paths);

                            if (resultElement != null)
                            {
                                dsp.SetProperty("CurrentMacroItem", resultElement.ToString());
                                dsp.InvokeMethod("OnAssignHotkey");


                            }
                            c = corelApp.FrameWork.CommandBars[newCommandBar].Controls.AddCustomButton("2cc24a3e-fe24-4708-9a74-9c75406eebcd", this.commands[i].Command);
                            controlsID.Add(c.ID);
                        }
                    }
                }
               
            }
            if (install.Equals(Status.CreateCommandBar))
            {
                this.corelApp.FrameWork.CommandBars[newCommandBar].Visible = true;

                SetIcons(controlsID);
              
                
            }
        }
        private void SetIcons(List<string> controlsID)
        {

            for (int i = 0; i < this.Commands.Count; i++)
            {

                if (!string.IsNullOrEmpty(this.Commands[i].Ico) && this.commands[i].Add)
                {
                    string name = Path.GetFileName(this.commands[i].Ico);
                    string dest = string.Format("{0}{1}", this.corelApp.GMSManager.UserGMSPath, name);
                    try
                    {
                        if(!File.Exists(dest))
                            File.Copy(this.Commands[i].Ico, dest);
                        c.Controls controls =  corelApp.FrameWork.CommandBars[newCommandBar].Controls;
                        for (int k = 1; k <= controls.Count; k++)
                        {
                            if (controls[k].ID == controlsID[i])
                            {
                                controls[k].SetIcon2(dest);
                                break;
                            }

                        }
                        
                    }
                    catch { }
                }
            }
        }

        private XElement FindElementByPath(XElement parentElement, string targetGuid, string[] paths)
        {
            XElement currentElement = parentElement
                .Elements("itemData")
                .FirstOrDefault(e => (string)e.Attribute("guid") == targetGuid);

            foreach (string path in paths)
            {
                if (currentElement != null)
                {
                    currentElement = currentElement
                        .Elements("container")
                        .Elements("itemData")
                        .FirstOrDefault(e => (string)e.Attribute("text") == path);
                }

                if (currentElement == null)
                {
                    return null;
                }
            }

            return currentElement;
        }
        private void processZip(string zipFile)
        {
            try
            {
                string path = GetTempDir();
                ZipFile.ExtractToDirectory(zipFile, path);
                processFolder(path);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("zip erro!");
            }
        }

        private string GetTempDir()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        private void processFolder(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            string[] folders = Directory.GetDirectories(folderPath);
            processFiles2(files);
            for (int i = 0; i < folders.Length; i++)
            {
                processFolder(folders[i]);
            }
        }
        private void processFiles2(string[] files)
        {
            SharpCompressManager sharpCompressManager = null;
            for (int i = 0; i < files.Length; i++)
            {
                if (Directory.Exists(files[i]))
                    processFolder(files[i]);
                else
                {
                    string ext = Path.GetExtension(files[i]).ToLower();
                    switch (ext)
                    {
                        case ".zip":
                            processZip(files[i]);
                            break;

                        case ".rar":
                            if (sharpCompressManager == null)
                                sharpCompressManager = new SharpCompressManager();
                            sharpCompressManager.EnqueueFile(files[i]);
                            break;
                        case ".gms":
                            processGMS(files[i]);
                            break;
                        case ".ico":
                            processICO(files[i]);
                            break;
                        default:
                            processOthers(files[i]);
                            break;
                    }
                }
            }
            if (sharpCompressManager != null)
            {
                sharpCompressManager.ProcessFiles();
                processFolder(sharpCompressManager.TempPath);
                //     System.Diagnostics.Process.Start(sharpCompressManager.TempPath);

            }
        }
        private void processGMS(string path)
        {
            GMSProject gmp = corelApp.GMSManager.Projects.Load(path);
            GMSMacros macros = gmp.Macros;

            for (int r = 1; r <= macros.Count; r++)
            {
                string name = string.Format("{0}.{1}", gmp.Name, macros[r].Name);

                Commands.Add(new MacroCommand(name) { FilePath = path });
            }
            gmp.Unload();


        }
        private void processICO(string path)
        {
            Icos.Add(path);
        }
        private void processOthers(string path)
        {
            try
            {
                File.Copy(path, this.corelApp.GMSManager.UserGMSPath);

            }
            catch { }
        }
        private PTYPE getType(string path)
        {
            if (Directory.Exists(path))
                return PTYPE.FOLDER;
            else
            {
                string ext = Path.GetExtension(path).ToLower();
                switch (ext)
                {
                    case ".zip":
                        return PTYPE.ZIP;
                    case ".rar":
                        return PTYPE.RAR;
                    case ".gms":
                        return PTYPE.GMS;
                    case ".ico":
                        return PTYPE.ICO;
                    default:
                        return PTYPE.OTHERS;



                }
            }
        }
        enum PTYPE
        {
            FOLDER,
            ZIP,
            RAR,
            GMS,
            ICO,
            OTHERS
        }

    }

    public class MacroCommand : MMBase
    {
        public string FilePath
        {
            get;
            set;
        }
        private string ico;
        public string Ico
        {
            get { return ico; }
            set
            {
                ico = value;
                OnPropertyChanged();
            }
        }


        private string command;

        public string Command
        {
            get { return command; }
            set { command = value; OnPropertyChanged(); }

        }
        private bool add;


        public bool Add
        {
            get { return add; }
            set
            {
                add = value;
                OnPropertyChanged();
            }
        }
        //private bool selected;

        //public bool Selected
        //{
        //    get { return selected; }
        //    set { selected = value;
        //        OnPropertyChanged();
        //    }
        //}


        public MacroCommand()
        {
            
        }
        public MacroCommand(string command)
        {
            Command = command;
        }
     
    }

}
