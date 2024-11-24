using br.com.Bonus630DevToolsBar;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using f = System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using System.Drawing;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Input;



namespace WorkspaceUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //WorkspaceObjectTranporter data;
        //FileSystemWatcher fsw;
        string path;
        DispatcherTimer timer;
        private f.NotifyIcon _notifyIcon;
        public MainWindow()
        {
            // InitializeComponent();

        }
        public MainWindow(string path)
        {
            this.path = path;
            _notifyIcon = new f.NotifyIcon
            {
                Icon = GetApplicationIcon(),
                Visible = true,
                Text = "Workspace Backup"
            };
        }
        internal void StartMonitor()
        {
            try
            {
                EnviarMensagemTray("CorelDraw Workspace Backup", "Will start in 5 seconds!");
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += Timer_Tick;
                timer.Start();

                //data = GetData(paths[0]);
                //fsw = new FileSystemWatcher(data.WorkspacePath, "*.cdws");
                //fsw = new FileSystemWatcher(@"C:\Users\bonus\AppData\Roaming\Corel\CorelDRAW Graphics Suite X8\Draw\Workspace", "*.cdws");
                //fsw.EnableRaisingEvents = true;
                //fsw.Changed += Fsw_Changed;
            }
            catch (Exception e) { EnviarMensagemTray("Error", e.Message); Application.Current.Shutdown(0); }
        }
        private void EnviarMensagemTray(string titulo, string mensagem)
        {
            _notifyIcon.BalloonTipTitle = titulo;
            _notifyIcon.BalloonTipText = mensagem;
            _notifyIcon.BalloonTipIcon = f.ToolTipIcon.Info; // Info, Warning ou Error
            _notifyIcon.ShowBalloonTip(3000); // Mostra por 3 segundos
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            var data = GetData(path);
            if (data != null)
            {
                timer.Stop();
                MakeChanges(data);
            }
            else
                timer.Interval = TimeSpan.FromMilliseconds(500);
        }
        private Icon GetApplicationIcon()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;

            return System.Drawing.Icon.ExtractAssociatedIcon(exePath);
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            System.Windows.MessageBox.Show("Changed");
            if (e.ChangeType.Equals(WatcherChangeTypes.Changed))
            {
                // MakeChanges();
            }
        }
        private void MakeChanges(WorkspaceObjectTranporter data)
        {
            string workspace = CreateBackup(data);
            if (!string.IsNullOrEmpty(workspace))
            {

                try
                {
                    ChangeWorkspace(workspace, data);
                    File.Delete(path);
                    EnviarMensagemTray("CorelDraw Workspace Backup", "Well Done!");


                }
                catch
                {

                }

                Application.Current.Shutdown(0);
            }
        }
        public string CreateBackup(WorkspaceObjectTranporter data)
        {
            string path = data.WorkspacePath + data.WorkspaceName;
            try
            {
                File.Copy(path + ".cdws", path + "_" + DateTime.Now.ToString("yy-MM-dd-HH-mm-ss") + ".cdws");
                path = path + ".cdws";
                return path;
            }
            catch { return ""; }
        }
//          	<shortcutKeyTables customized = "true" >

//        < table tableID="bc175625-191c-4b95-9053-756e5eee26fe">
//<keySequence itemRef = "126d9444-17c7-4715-bdf8-8ee08605186c" >< key > w </ key ></ keySequence ></ table >
//</ shortcutKeyTables >
        public void ChangeWorkspace(string zipFilePath, WorkspaceObjectTranporter data)
        {
            // System.Windows.MessageBox.Show("ChangeWorkspace");
            string xmlFileName = "content/workspace.xml";
            using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
            {
                // Procura o arquivo XML no ZIP
                var xmlEntry = archive.GetEntry(xmlFileName);
                if (xmlEntry != null)
                {
                    // Lê o arquivo XML
                    using (var xmlStream = xmlEntry.Open())
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlStream);
                        //var rootNamespace = xmlDoc.DocumentElement.NamespaceURI;
                        // var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                        // namespaceManager.AddNamespace("p", "http://schemas.microsoft.com/developer/msbuild/2003");
                        XmlNodeList xmlNodeList = xmlDoc.ChildNodes[1].ChildNodes;
                        for (int i = 0; i < xmlNodeList.Count; i++)
                        {
                            if (xmlNodeList[i].Name == "items")
                            {
                                var nodes = xmlNodeList[i].ChildNodes;
                                for (int j = 0; j < nodes.Count; j++)
                                {
                                    if (nodes[j].Name == "itemData")
                                    {
                                        for (int k = 0; k < data.Items.Count; k++)
                                        {
                                            if (nodes[j].Attributes["guid"].Value == data.Items[k])
                                            {
                                                XmlAttribute a = xmlDoc.CreateAttribute("icon");
                                                a.Value = "guid://" + data.Items[k];
                                                nodes[j].Attributes.Append(a);
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        xmlStream.SetLength(0);
                        xmlDoc.Save(xmlStream);
                    }
                }
                else
                {
                    Console.WriteLine("Arquivo XML não encontrado no ZIP.");
                }
                for (int i = 0; i < data.Items.Count; i++)
                {
                    ZipArchiveEntry entry = archive.CreateEntryFromFile(string.Format("{0}\\{1}.ico", data.IconFolder, data.Items[i]), string.Format("content/icons/{0}.ico", data.Items[i]));

                }
            }
        }



        private WorkspaceObjectTranporter GetData(string path)
        {
            try
            {
                var formatter = new BinaryFormatter();
                using (Stream s = File.OpenRead(path))
                {

                    WorkspaceObjectTranporter w = (WorkspaceObjectTranporter)formatter.Deserialize(s);

                    return w;

                }

            }
            catch
            {
                return null;
            }

        }
        private void SetData(WorkspaceObjectTranporter data, string path)
        {
            using (Stream s = File.OpenWrite(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, data);
            }

        }

    }
}
