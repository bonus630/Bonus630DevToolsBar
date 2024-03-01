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
using Microsoft.Win32;
using c = Corel.Interop.VGCore;
using Corel.Interop.VGCore;
using System.Runtime.CompilerServices;

namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    public abstract class MMBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MacrosManager : MMBase
    {
        string rarExePath = string.Empty;
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
        public MacrosManager(c.Application corelApp, string[] files)
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
            CommandManager cm = new CommandManager(this);


            //Commands.Add(new MacroCommand("ab.cd.df") { FilePath = "abc"});
            //icos.Add(@"C:\Users\bonus\OneDrive\Imagens\ICOS\packer.ico");
            //icos.Add(@"C:\Users\bonus\OneDrive\Imagens\ICOS\qrcode.ico");


            if ((bool)cm.ShowDialog())
            {
                if(!string.IsNullOrEmpty(NewCommandBar) && !commandBars.Contains(newCommandBar))
                    this.corelApp.FrameWork.CommandBars.Add(newCommandBar);
                //else
                //{
                //    MessageBox.Show("Co");
                //}
                for (int i = 0; i < this.Commands.Count; i++)
                {
                    string name = Path.GetFileName(this.Commands[i].FilePath);
                    string dest = string.Format("{0}{1}", this.corelApp.GMSManager.UserGMSPath, name);
                    try
                    {
                        File.Copy(this.Commands[i].FilePath, dest);
                    }
                    catch { }
                    this.corelApp.GMSManager.Projects.Load(dest);
                    c.Control c = corelApp.FrameWork.CommandBars[newCommandBar].Controls.AddCustomButton("2cc24a3e-fe24-4708-9a74-9c75406eebcd", this.commands[i].Command);
                   
                    if (!string.IsNullOrEmpty(this.Commands[i].Ico))
                    {
                        name = Path.GetFileName(this.commands[i].Ico);
                        dest = string.Format("{0}{1}", this.corelApp.GMSManager.UserGMSPath, name);
                        try
                        {
                            File.Copy(this.Commands[i].Ico, dest);
                        }
                        catch { }
                        c.SetIcon2(dest);
                    }

                }
                this.corelApp.FrameWork.CommandBars[newCommandBar].Visible = true;
               // MessageBox.Show("aqui vamos instalar de fato");

            }


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
        private void processRar(string rarFile)
        {
            if (string.IsNullOrEmpty(rarExePath))
                rarExePath = FindWinrarPath();
            if (string.IsNullOrEmpty(rarExePath))
            {
                System.Windows.Forms.MessageBox.Show("Winrar not founded!");
                return;
            }
            rarExePath = Path.Combine(rarExePath, "Rar.exe");
            using (Process processo = new Process())
            {
                string path = GetTempDir();
                processo.StartInfo.FileName = rarExePath;
                processo.StartInfo.Arguments = string.Format("x \"{0}\" \"{1}\"", rarFile, path);
                processo.StartInfo.UseShellExecute = false;
                processo.StartInfo.RedirectStandardOutput = true;
                processo.StartInfo.CreateNoWindow = true;

                processo.Start();
                processo.WaitForExit();
                processFolder(path);
            }
        }
        private string GetTempDir()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
        private string FindWinrarPath()
        {
            string[] paths = { @"C:\Program Files\WinRAR", @"C:\Program Files (x86)\WinRAR" };

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            string chaveRegistro = @"SOFTWARE\WinRAR";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(chaveRegistro))
            {
                if (key != null)
                {
                    object valor = key.GetValue("Path");
                    if (valor != null)
                    {
                        return valor.ToString();
                    }
                }
            }
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "Select WinRAR.exe";
            openFileDialog.Filter = "Rar.exe (Rar.exe)|*.exe";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;


            }
            return null;
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
                            processRar(files[i]);
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

        public MacroCommand()
        {

        }
        public MacroCommand(string command)
        {
            Command = command;
        }

    }

}
