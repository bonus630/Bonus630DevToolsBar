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
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Drawing.IconLib;
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
        private bool CheckShortcuts()
        {
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].UseShortcut)
                    return true;
            }
            return false;
        }
        private List<string> workspaceModifyDatas = new List<string>();
        public void InstallBar(c.Application corelApp, string[] files, string theme)
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
            // List<string> controlsID = new List<string>(); ;
            if (!install.Equals(Status.Cancel))
            {
                DataSourceProxy dsp = null;
                XDocument xdoc = null;
                if (install.Equals(Status.CreateCommandBar) && CheckShortcuts())
                {

                    dsp = corelApp.FrameWork.Application.DataContext.GetDataSource("MacroMgrDockerDS");
                    string xmlString = (string)dsp.GetProperty("MacroItemList");
                    if (string.IsNullOrEmpty(xmlString))
                        return;

                    xdoc = XDocument.Parse(xmlString);
                }
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

                            if (this.commands[i].UseShortcut)
                            {
                                string[] paths = this.commands[i].Command.Split(new char[] { '.' }, options: StringSplitOptions.RemoveEmptyEntries);

                                XElement resultElement = FindElementByPath(xdoc.Root, "VBA", paths);

                                if (resultElement != null)
                                {
                                    dsp.SetProperty("CurrentMacroItem", resultElement.ToString());
                                    dsp.InvokeMethod("OnAssignHotkey");
                                }
                            }
                            var commandBar = corelApp.FrameWork.CommandBars[newCommandBar];
                            c = commandBar.Controls.AddCustomButton("2cc24a3e-fe24-4708-9a74-9c75406eebcd", this.commands[i].Command);
                            this.Commands[i].Guid = c.ID;
                            // controlsID.Add(c.ID);
                        }
                    }
                }
            }
            if (install.Equals(Status.CreateCommandBar))
            {
                this.corelApp.FrameWork.CommandBars[newCommandBar].Visible = true;
                SetIcons();
                PrepareModifyWorkspace();
            }
        }
        private void SetIcons()
        {
            for (int i = 0; i < this.Commands.Count; i++)
            {

                if (!string.IsNullOrEmpty(this.Commands[i].Ico) && this.commands[i].Add)
                {
                    c.Controls controls = corelApp.FrameWork.CommandBars[newCommandBar].Controls;
                    for (int k = 1; k <= controls.Count; k++)
                    {
                        if (controls[k].ID == this.Commands[i].Guid)
                        {
                            string path = CopyIcon(this.Commands[i].Ico, this.Commands[i].Guid);
                            if (!string.IsNullOrEmpty(path))
                            {
                                controls[k].SetIcon2(path);
                            }
                            break;
                        }
                    }
                }
            }
        }
        private string CopyIcon(string sourceFilePath, string guid)
        {
            FileInfo fi = new FileInfo(sourceFilePath);
            string name = Path.GetFileName(sourceFilePath);
            string dest = string.Format("{0}{1}.ico", this.corelApp.GMSManager.UserGMSPath, guid);
            if (!fi.Extension.Equals(".ico"))
            {
                sourceFilePath = this.ContertToIcon(sourceFilePath);
            }
            try
            {
                if (!File.Exists(dest))
                {
                    File.Copy(sourceFilePath, dest);
                    return dest;
                }
            }
            catch { }
            return null;
        }
        private string ContertToIcon(string imagePath)
        {
            string iconPath = Path.GetTempFileName();
            iconPath = iconPath.Replace(".tmp", ".ico");
            MultiIcon mIcon = new MultiIcon();
            SingleIcon sIcon = mIcon.Add(Path.GetFileName(imagePath));
            System.Drawing.Image original = System.Drawing.Bitmap.FromFile(imagePath);
            int size = 16;
            if (original.Width > original.Height)
                size = RoundDownToNearest(original.Width);
            else
                size = RoundDownToNearest(original.Height);
            System.Drawing.Bitmap bitmap16 = new System.Drawing.Bitmap(original, size, size);

            sIcon.Add(bitmap16);
            if (size == 256)
                sIcon[0].IconImageFormat = IconImageFormat.PNG;
            mIcon.SelectedIndex = 0;
            mIcon.Save(iconPath, MultiIconFormat.ICO);
            //using (Bitmap bitmap = new Bitmap(imagePath))
            //{
            //    Icon icon = Icon.FromHandle(bitmap.GetHicon());
            //    using (System.IO.FileStream stream = new System.IO.FileStream(iconPath, System.IO.FileMode.Create,FileAccess.Write,FileShare.Write))
            //    {
            //        icon.Save(stream);
            //    }
            //}
            return iconPath;
        }
        int RoundDownToNearest(int number)
        {
            int[] values = { 16, 32, 48, 64, 128, 256 };

            int closest = values[0];
            foreach (int val in values)
            {
                if (val <= number)
                {
                    closest = val;
                }
                else
                {
                    break;
                }
            }
            return closest;
        }

    
        private void PrepareModifyWorkspace()
        {
            for (int i = 0; i < this.Commands.Count; i++)
            {
                if (!string.IsNullOrEmpty(this.Commands[i].Ico) && this.commands[i].Add)
                    workspaceModifyDatas.Add(this.Commands[i].Guid);
            }
        }
        private string StartModifyWorkspace()
        {
            WorkspaceObjectTranporter w = new WorkspaceObjectTranporter();
            w.WorkspacePath = this.corelApp.UserWorkspacePath;
            w.WorkspaceName = this.corelApp.ActiveWorkspace.Name;
            w.IconFolder = this.corelApp.GMSManager.UserGMSPath;
            w.Items.AddRange(workspaceModifyDatas);
            string path = Path.GetTempFileName();
            SetData(w, path);
            if (File.Exists(path))
                return path;
            return "";
            //SetData(w, @"C:\Users\bonus\OneDrive\Ambiente de Trabalho\i.it");
        }
        private void SetData(WorkspaceObjectTranporter data, string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream s = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(s, data);
                s.Flush();
            }

        }
        public void RunWorkspaceModifier()
        {
            if (workspaceModifyDatas.Count > 0)
            {
                string path = StartModifyWorkspace();
                if (string.IsNullOrEmpty(path))
                    return;
                ProcessStartInfo process = new ProcessStartInfo();
                //"C:\Program Files\Corel\CorelDRAW Graphics Suite X8\Programs64\Addons\Bonus630DevToolsBar\WorkspaceUpdater.exe"
                process.FileName = string.Format("{0}{1}\\WorkspaceUpdater.exe", this.corelApp.AddonPath, "Bonus630DevToolsBar");
                process.Arguments = path;
                Process p = new Process();
                p.StartInfo = process;
                p.Start();
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
    private string guid;

    public string Guid
    {
        get { return guid; }
        set { guid = value; OnPropertyChanged(); }

    }
    private bool add = true;


    public bool Add
    {
        get { return add; }
        set
        {
            add = value;
            OnPropertyChanged();
        }
    }
    private bool useShortcut;


    public bool UseShortcut
    {
        get { return useShortcut; }
        set
        {
            useShortcut = value;
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
