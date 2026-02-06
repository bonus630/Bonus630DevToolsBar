using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class ProjectsManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> RequestNewModuleEvent;
        public event Action<string, string> RequestRemoveModule;
        public event Action<string> ErroReceived;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public BindingCommand<Command> ExecuteCommand { get; set; }
        public BindingCommand<Command> ExecutePinCommand { get; set; }
        public BindingCommand<Command> StopCommand { get; set; }
        public BindingCommand<Command> TogglePinCommand { get; set; }
        public BindingCommand<Module> EditModuleCommand { get; set; }
        public BindingCommand<Command> EditCommandCommand { get; set; }
        public BindingCommand<Module> OrderModuleCommand { get; set; }
        public BindingCommand<Module> OrderOriginalModuleCommand { get; set; }
        public BindingCommand<Module> OrderDescModuleCommand { get; set; }
        public BindingCommand<Command> SetCommandToValueCommand { get; set; }
        public BindingCommand<Project> SwitchProjectCommand { get; set; }
        public BindingCommand<Project> LoadProjectCommand { get; set; }


        public BindingCommand<int> AddProjectCommand { get; set; }
        public BindingCommand<Project> AddModuleCommand { get; set; }
        public BindingCommand<Module> RemoveModuleCommand { get; set; }
        public BindingCommand<Module> AddCommandCommand { get; set; }

        public BindingCommand<Reflected> CopyValueCommand { get; set; }
        public BindingCommand<object> CopyReturnsValueCommand { get; set; }
        public SimpleCommand SetShapeRangeToValueCommand { get; set; }
        public SimpleCommand CreateSelectionShapeRangeCommand { get; set; }


        private bool myPopupIsOpen;

        public bool MyPopupIsOpen
        {
            get { return myPopupIsOpen; }
            set
            {
                myPopupIsOpen = value;
                OnPropertyChanged("MyPopupIsOpen");
            }
        }
        private bool requestNewModule;

        public bool RequestNewModule
        {
            get { return requestNewModule; }
            set
            {
                requestNewModule = value;
                OnPropertyChanged();
            }
        }
        private bool myPopupExceptionIsOpen;

        public bool MyPopupExceptionIsOpen
        {
            get { return myPopupExceptionIsOpen; }
            set
            {
                myPopupExceptionIsOpen = value;
                OnPropertyChanged("MyPopupExceptionIsOpen");
            }
        }


        private Point myPopupPosition;

        public Point MyPopupPosition
        {
            get { return myPopupPosition; }
            set
            {
                myPopupPosition = value;
                OnPropertyChanged("MyPopupPosition");
            }
        }


        public ShapeRangeManager shapeRangeManager { get; set; }

        private ObservableCollection<Project> projects;

        public ObservableCollection<Project> Projects
        {
            get { return projects; }
            set { projects = value; OnPropertyChanged("Projects"); }
        }

        private ObservableCollection<Command> pinnedCommands;

        public ObservableCollection<Command> PinnedCommands
        {
            get { return pinnedCommands; }
            set { pinnedCommands = value; OnPropertyChanged("PinnedCommands"); }
        }


        private Command selectedCommand;
        public Command SelectedCommand
        {
            get { return selectedCommand; }
            set
            {
                selectedCommand = value;
                OnPropertyChanged("SelectedCommand");
            }
        }

        private CommandSearch commandSearch;

        public CommandSearch CommandSearch
        {
            get { return commandSearch; }
            set
            {
                commandSearch = value;
                OnPropertyChanged("CommandSearch");
            }
        }


        string assemblyDirectory = "";
        public string AssemblyDirectory
        {
            get { return assemblyDirectory; }
            set
            {
                assemblyDirectory = value;
                OnPropertyChanged("AssemblyDirectory");
            }
        }


        FileSystemWatcher dllMonitor;
        Thread startUpThread;
        ProxyManager proxyManager;
        Dispatcher dispatcher;
        public ProjectsManager(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public void Start(ProxyManager proxyManager)
        {
            projects = new ObservableCollection<Project>();
            pinnedCommands = new ObservableCollection<Command>();
            AssemblyDirectory = Properties.Settings.Default.FolderPath;
            this.proxyManager = proxyManager;
            VSDetection();
            StartCommands();
            CheckFolder(assemblyDirectory);
        }
        private void StartCommands()
        {
            AddModuleCommand = new BindingCommand<Project>(AddModule, CanAddModule);
            RemoveModuleCommand = new BindingCommand<Module>(RemoveModule, CanRemoveModule);
            ExecuteCommand = new BindingCommand<Command>(RunCommandAsync);
            ExecutePinCommand = new BindingCommand<Command>(RunPinCommandAsync);
            StopCommand = new BindingCommand<Command>(StopCommandAsync);
            TogglePinCommand = new BindingCommand<Command>(PinCommand, CanPin);
            EditModuleCommand = new BindingCommand<Module>(EditModule, CanEditModule);
            OrderModuleCommand = new BindingCommand<Module>(OrderByName);
            OrderDescModuleCommand = new BindingCommand<Module>(OrderByNameDesc);
            OrderOriginalModuleCommand = new BindingCommand<Module>(OrderOriginal);
            EditCommandCommand = new BindingCommand<Command>(EditMethod, CanEditCommand);
            SwitchProjectCommand = new BindingCommand<Project>(UnloadProject);
            CopyValueCommand = new BindingCommand<Reflected>(CopyValue);
            CopyReturnsValueCommand = new BindingCommand<object>(CopyReturnsValue);
            SetCommandToValueCommand = new BindingCommand<Command>(SetCommandReturnArgumentValue, CanRunSetCommandReturnArgVal);
            SetShapeRangeToValueCommand = new SimpleCommand(SetShapeRangeArgumentValue);
            CreateSelectionShapeRangeCommand = new SimpleCommand(CreateSelectionShapeRange);
        }
        private void UnloadProject(Project project)
        {
            try
            {
                projects.Remove(project);
                project.SwitchLoaded();
                CreateProject(project.Name, project.Path);
                //File.Move(project.Path,Path.ChangeExtension(project.Path, ".bak"));

            }
            catch { }

        }



        public void LoadPinnedCommands()
        {
            try
            {

                //encontrar o comando pelo caminho vai garantir melhor desempenho
                var commandNames = Properties.Settings.Default.PinnedCommands;
                // PinnedCommands.Clear();

                for (int i = 0; i < commandNames.Count; i++)
                {
                    var command = FindCommandByXPath(commandNames[i]);

                    // Command c1 = pinnedCommands.FirstOrDefault(m => m.ToString().Equals(commandNames[i]));
                    if (command != null)
                    {
                        if (!PinnedCommands.Contains(command))
                            PinnedCommands.Add(command);
                    }
                }
                //Properties.Settings.Default.PinnedCommands.Clear();
                foreach (var item in PinnedCommands)
                {
                    if (!Properties.Settings.Default.PinnedCommands.Contains(item.ToString()))
                        Properties.Settings.Default.PinnedCommands.Add(item.ToString());
                }
                Properties.Settings.Default.Save();
                OnPropertyChanged("PinnedCommands");
            }
            catch { }
        }

        private Command FindCommandByXPath(string commandXPath)
        {
            string[] pierces = commandXPath.Split('/');
            if (pierces.Length > 0)
            {
                Project project = Projects.FirstOrDefault(n => n.Name.Equals(pierces[0])) as Project;

                if (project != null)
                {
                    Module module = project.Items.FirstOrDefault(v => v.Name.Equals(pierces[1]));
                    if (module != null)
                        return module.Items.FirstOrDefault(q => q.ToString().Equals(commandXPath));
                }
            }
            return null;
        }


        /// <summary>
        /// Use this to fill parameters in command
        /// </summary>
        /// <param name="command"></param>
        public void RunCommand(Command command)
        {
            if (command.HasParam)
                command.PrepareArguments();
            proxyManager.RunCommand(command);
        }
        /// <summary>
        /// Use this form run in button
        /// </summary>
        /// <param name="command"></param>
        public void RunCommandAsync(Command command)
        {
            if (command.HasParam)
                command.PrepareArguments();
            command.LastRunFails = false;
            proxyManager.RunCommandAsync(command);
        }

        public void RunPinCommandAsync(Command command)
        {
            if (command.HasParam)
                command.PrepareArguments();
            proxyManager.RunCommandAsync(command, false);
        }
        private void SetCommandReturnArgumentValue(Command command)
        {
            //Need checks for recursive 
            //Is require a cache?
            //Can pass the command to value and create the func in command runtime?
            Argument argument = GetArgument(command);
            if (argument != null)
            {
                //if (!argument.ArgumentType.Equals(typeof(Corel.Interop.VGCore.ShapeRange)))
                //    argument.SetIncompatibleArgumentType();
                argument.Value = new FuncToParam()
                {
                    Name = command.Name,
                    FullPath = command.ToString(),
                    MyFunc = new Func<Command, object>(
                    (c) =>
                    {
                        RunCommand(command);
                        return command.Returns;
                    })
                };
            }
        }
        private void StopCommandAsync(Command command)
        {
            proxyManager.StopCommandAsync(command);
        }
        private void PinCommand(Command command)
        {
            Command c = pinnedCommands.FirstOrDefault(r => r.ToString().Equals(command.ToString()));
            if (c == null)
            {
                PinnedCommands.Add(command);
                Properties.Settings.Default.PinnedCommands.Add(command.ToString());
            }
            else
            {
                PinnedCommands.Remove(c);
                Properties.Settings.Default.PinnedCommands.Remove(c.ToString());
            }
            OnPropertyChanged("PinnedCommands");
            Properties.Settings.Default.Save();
        }
        private bool CanPin(Command command)
        {
            bool canPin = false;
            if (command != null)
            {
                if (!command.HasParam)
                    canPin = true;
            }
            return canPin;
        }
        private void AddModule(Project project)
        {
            RequestNewModule = true;
            if (RequestNewModuleEvent != null)
                RequestNewModuleEvent(project.ProjFilePath);
            //FileInfo fi = new FileInfo(project.ProjFilePath);
            //preciso extrair o modelo do template
            //Editalo, alterar nome da classe e do arquivo para o que o usuario digitar

            //adicionar no proj file
            //compilar o projeto
        }
        private bool CanAddModule(Project project)
        {
            if (project == null || RequestNewModule)
                return false;
            return !string.IsNullOrEmpty(project.ProjFilePath);
        }
        private void RemoveModule(Module module)
        {
            //Por enquanto vamos fazer assim
            if (RequestRemoveModule != null)
                RequestRemoveModule(module.Parent.ProjFilePath, module.Name);

        }
        private bool CanRemoveModule(Module module)
        {
            if (module == null)
                return false;
            return !string.IsNullOrEmpty(module.Parent.ProjFilePath);
        }

        private bool vsFounded = false;
        private string vsExecutablePath;
        private bool CanEditCommand(Command command)
        {
            if (command != null && !string.IsNullOrEmpty(command.Parent.Parent.ProjFilePath) && vsFounded)
                return true;
            return false;
        }

        private void VSDetection()
        {

            Task.Run(() =>
            {

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();


                startInfo.FileName = "vswhere.exe";
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = "-latest -property installationPath";

                process.StartInfo = startInfo;
                try
                {
                    process.Start();
                }
                catch (Exception ex) { vsFounded = false; return; }
                string vsInstallationPath = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(vsInstallationPath))
                {

                    vsExecutablePath = System.IO.Path.Combine(vsInstallationPath, "Common7\\IDE\\devenv.exe");
                    vsFounded = true;
                }
                else
                {
                    vsFounded = false;
                }
            });


        }

        private void EditMethod(Command command)
        {
            //devenv.exe "caminho\para\a\solucao.sln" /edit "caminho\para\o\arquivo.extensão"
            //devenv.exe "caminho\para\a\solucao.sln" /edit "caminho\para\o\arquivo.extensão":linha
            //devenv.exe "caminho\para\o\arquivo.extensão" /command "Edit.GoTo nomeDoMétodo"
            //devenv.exe "caminho\para\o\arquivo.extensão" /command "Edit.GoToDefinition nomeDoMétodo"
            string proj = command.Parent.Parent.ProjFilePath;
            string file = command.Parent.ModulePath;
            string method = command.Name;
            if (string.IsNullOrEmpty(proj) || string.IsNullOrEmpty(file) || string.IsNullOrEmpty(method))
                return;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // startInfo.FileName = "devenv.exe";
            startInfo.FileName = this.vsExecutablePath;

            startInfo.Arguments = string.Format("\"{0}\" /command \"Edit.openfile {1}\"", proj, file);
            //startInfo.Arguments = string.Format("\"Edit.openfile {1}\"", proj, file, method); 
            //startInfo.Arguments = string.Format("\"{0}\" /Command Edit.OpenFile \"{1}\" /command Edit.GoTo {2}", proj, file, method);*
            // startInfo.Arguments = string.Format("\"{0}\" /edit \" {1}:20\"", proj, file, method);*
            //startInfo.Arguments = string.Format("\"{0}\" /Command Edit.OpenFile \"{1}\" /command Edit.GoToDefinition {2}", proj, file, method);*
            //startInfo.Arguments = string.Format("\"{0}\"  Edit.OpenFile \"{1}\" /command Edit.GoToDefinition {2}", proj, file, method);*
            //startInfo.Arguments = string.Format("\"{0}\"  /command Edit.GoToDefinition mais1.{2}", proj, file, method);*
            //startInfo.Arguments = string.Format("\"{0}\" /Edit \"{1}\" /command Edit.GoToDefinition {2}",proj, file, method);
            //startInfo.Arguments = string.Format("\"{0}\" /Edit \"{1}\" /command Edit.GoToDefinition {2}",proj, file, method);

            process.StartInfo = startInfo;
            process.Start();

        }

        private void EditModule(Module module)
        {
            //teste apagar
            //module.Order();
            Process.Start(module.ModulePath);
        }

        private void OrderByName(Module module)
        {
            module.Order();
        }
        private void OrderByNameDesc(Module module)
        {
            module.OrderDesc();
        }
        private void OrderOriginal(Module module)
        {
            module.OrderOriginal();
        }
        private bool CanEditModule(Module module)
        {
            if (module == null)
                return false;
            if (!string.IsNullOrEmpty(module.ModulePath))
                return File.Exists(module.ModulePath);
            return false;
        }
        private bool CanRunSetCommandReturnArgVal(Command command)
        {
            if (command.ReturnsType == null || command.ReturnsType == typeof(void))
                return false;
            return true;
        }
        private Argument GetArgument(Command command)
        {

            if (this.SelectedCommand != null && this.SelectedCommand.Items != null)
            {
                Argument arg = this.SelectedCommand.Items.FirstOrDefault(r => r.IsSelectedBase);
                if (arg == null && this.selectedCommand.Items.Count > 0)
                    arg = this.selectedCommand.Items[0];
                return arg;
            }
            return null;
        }
        private void SetShapeRangeArgumentValue()
        {
            if (this.SelectedCommand != null && this.SelectedCommand.Items != null)
            {
                Argument argument = this.SelectedCommand.Items.FirstOrDefault(r => r.IsSelectedBase);
                if (argument != null)
                {
                    //if (!argument.ArgumentType.Equals(typeof(Corel.Interop.VGCore.ShapeRange)))
                    //    argument.SetIncompatibleArgumentType();
                    argument.Value = new FuncToParam()
                    {
                        Name = "GetShapes",
                        FullPath = "br.com.Bonus630DevToolsBar.RunCommandDocker.GetShapes",
                        MyFunc = new Func<Command, object>(shapeRangeManager.GetShapes)
                    };
                }

            }
        }
        private void CreateSelectionShapeRange()
        {

            shapeRangeManager.SetSelection();

        }
        //private bool CanRunSetShapeRangeArgumentValue(object o)
        //{
        //    if (this.SelectedCommand != null && this.SelectedCommand.Items != null)
        //    {
        //        Argument argument = this.SelectedCommand.Items.FirstOrDefault(r => r.IsSelectedBase);
        //        return (argument != null);



        //    }
        //    return false;
        //}
        private void CopyValue(Reflected o)
        {
            Clipboard.SetText(o.Value.ToString());
        }
        private void CopyReturnsValue(object o)
        {
            if (o != null)
            {
                if (Utils.IsCollectionOfValueTypeOrString(o))
                {
                    Clipboard.SetText(Utils.ConcatenateValues(o));
                }
                //else if(Utils.IsTypeAny(o, typeof(Corel.Interop.VGCore.Shape),typeof(Corel.Interop.VGCore.Shapes),typeof(Corel.Interop.VGCore.ShapeRange)))
                //{
                //    Clipboard.set
                //}
                else
                {
                    Clipboard.SetText(o.ToString());
                }
            }
        }
        public bool SelectAssembliesFolder()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "Select a destination folder for your Command Assemblies (DLL)!";
            if (fbd.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
            {
                if (fbd.SelectedPath.Equals(AssemblyDirectory))
                    return true;
                this.dispatcher.Invoke(new Action(() =>
                {
                    if (projects != null)
                        projects.Clear();
                }));
                try
                {
                    startUpThread.Abort();
                    startUpThread = null;
                }
                catch { }
                AssemblyDirectory = fbd.SelectedPath;
                Properties.Settings.Default.FolderPath = assemblyDirectory;
                Properties.Settings.Default.Save();
                if (Directory.Exists(AssemblyDirectory))
                {

                    startFolderMonitor(assemblyDirectory);
                    return true;
                }

                else
                    return false;

            }
            else
                return false;
        }
        public void OpenFolder()
        {
            if (Directory.Exists(assemblyDirectory))
                System.Diagnostics.Process.Start(assemblyDirectory);
        }
        private bool CheckFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                if (System.Windows.MessageBox.Show("Please define a directory to store your dlls, this is a mandatory step for this tool to work", "Invalid or not defined directory!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.Yes))
                {
                    return SelectAssembliesFolder();

                }
                return false;
            }
            else
            {
                startFolderMonitor(folder);
            }
            return true;

        }
        public void SetAssembliesFolder(string folder, ProjectCreator pc)
        {
            List<string> projects = GetProjectList(folder);
            for (int i = 0; i < projects.Count; i++)
                using (var projManager = new ProjManager(projects[i]))
                    projManager.ChangeAssembliesPathInProj(this.AssemblyDirectory);
            pc.BuildCollection(projects);

        }
        private List<string> GetProjectList(string folder)
        {
            string csExt = "*.csproj";
            string vbExt = "*.vbproj";

            List<string> projects = new List<string>();

            if (Directory.Exists(folder + "\\cs"))
            {
                string[] csProjs = Directory.GetFiles(folder + "\\cs", csExt, SearchOption.AllDirectories);
                for (int i = 0; i < csProjs.Length; i++)
                    projects.Add(csProjs[i]);
            }
            if (Directory.Exists(folder + "\\vb"))
            {
                string[] vbProjs = Directory.GetFiles(folder + "\\vb", vbExt, SearchOption.AllDirectories);
                for (int i = 0; i < vbProjs.Length; i++)
                    projects.Add(vbProjs[i]);
            }
            return projects;
        }
        private void startFolderMonitor(string dir)
        {

            dllMonitor = new FileSystemWatcher(dir);
            dllMonitor.EnableRaisingEvents = true;
            dllMonitor.Changed += Fsw_Changed;
            dllMonitor.Deleted += Fsw_Deleted;
            dllMonitor.Created += Fsw_Created;

            //Projects = new ObservableCollection<Project>();
            startUpThread = new Thread(new ThreadStart(ReadFiles));
            startUpThread.IsBackground = true;
            startUpThread.Start();
        }

        private void SetModulesCommands(Project project)
        {
            //Its works but, can are better
            try
            {
                CommandProxy proxy = proxyManager.LoadAssembly(project);
                string[] lastCommand = null;
                if (!string.IsNullOrEmpty(proxyManager.LastCommandPath))
                {
                    lastCommand = proxyManager.LastCommandPath.Split('/');
                }
                if (lastCommand != null && lastCommand[0].Equals(project.Name))
                    project.IsExpanded = true;
                Tuple<string, string, string>[] typesNames = proxy.GetTypesNames();
                ObservableCollection<Module> tempList = new ObservableCollection<Module>();
                for (int i = 0; i < typesNames.Length; i++)
                {
                    Module m = new Module()
                    {
                        Name = typesNames[i].Item1,
                        FullName = typesNames[i].Item2,
                        ModulePath = typesNames[i].Item3,
                        Parent = project
                    };
                    if (lastCommand != null && lastCommand[1].Equals(m.Name))
                        m.IsExpanded = true;
                    tempList.Add(m);
                    //project.Add(m);
                    string[] commandNames = proxy.GetMethodNames(m.FullName);
                    for (int k = 0; k < commandNames.Length; k++)
                    {
                        Command command = new Command()
                        {
                            Method = commandNames[k],
                            Name = commandNames[k],
                            Parent = m,
                            Index = k
                        };
                        m.Add(command);
                        var arguments = proxy.GetArguments(command);

                        command.AddRange(arguments);


                        command.CommandSelectedEvent += CommandSelected;
                        if (lastCommand != null && lastCommand[2].Equals(command.Name))
                            command.IsSelectedBase = true;
                        //if(!m.Contains(command))

                    }

                }
                project.AddAndCheckRange(tempList);

            }
            catch (AssemblyInspectionException)
            {
                throw; // já está rica
            }
            catch (Exception ex)
            {
                throw new AssemblyInspectionException(
                    "Erro ao processar módulos.",
                    ex
                );
            }
            finally
            {
                proxyManager.UnloadDomain();
            }
        }

        private void TryMapSource(string folder, Project project)
        {
            if (string.IsNullOrEmpty(project.ProjFilePath))
            {
                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bonus630\\Projects");
                List<string> projects = GetProjectList(folder);
                for (int i = 0; i < projects.Count; i++)
                    using (var projManager = new ProjManager(projects[i]))
                    {
                        string file = Path.GetFileNameWithoutExtension(project.Path);
                        string assName = projManager.GetAssemblyName();

                        if (file.Equals(assName))
                        {
                            project.ProjFilePath = projects[i];
                            return;
                        }
                    }
            }
        }


        private void CommandSelected(Command command)
        {
            //Ref.:01 
            // Compare to another Ref.:01

            //if (command.IsSelected)
            //    this.SelectedCommand = command;

        }
        private void ReadFiles()
        {
            FileInfo[] files = (new DirectoryInfo(assemblyDirectory)).GetFiles();

            foreach (var item in files)
            {
                if (item.Extension.ToLower().Equals(".dll") || item.Extension.ToLower().Equals(".bak"))
                    CreateProject(item.Name, item.FullName);
            }

        }
        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            CreateProject(e.Name, e.FullPath);

        }
        private Project GetProject(string name, string path)
        {
            return Projects.FirstOrDefault(r => r.Name.Equals(name) && r.Path.Equals(path));
        }

        private void Fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            DeleteProject(e.Name, e.FullPath);
        }
        private void CreateProject(string name, string path)
        {
            Project project = GetProject(name, path);
            if (project == null)
            {
                project = new Project()
                {
                    Name = name,
                    Path = path,
                    Parent = this
                };
            }
            this.dispatcher.Invoke(new Action(() =>
            {

                if (System.IO.Path.GetExtension(project.Path).ToLower().Equals(".dll"))
                {
                    project.Name = project.Name.Replace("Unloaded ", "");

                }
                else
                {
                    project.Loaded = false;
                    project.Name = string.Format("Unloaded {0}", project.Name);
                    project.Name = project.Name.Replace(".bak", ".dll");
                }
                if (!Projects.Contains(project))
                    Projects.Add(project);
                //if (Projects.Count == 0)
                //    Projects.Add(project);
                //else
                //{
                //    if (!Projects.Contains(project))
                //    {
                //        if (project.Loaded)
                //        {
                //            for (int i = projects.Count - 1; i >= 0; i--)
                //            {

                //                if (Projects[i].Loaded || i == 0)
                //                {
                //                    projects.Insert(i, project);
                //                    break;
                //                }
                //            }
                //        }
                //        else
                //            Projects.Add(project);

                //    }
                //}
                if (project.Loaded)
                {
                    try
                    {
                        this.TryMapSource("", project);
                        SetModulesCommands(project);
                        LoadPinnedCommands();
                    }
                    catch (AssemblyInspectionException ex)
                    {
                        var sb = new System.Text.StringBuilder();

                        sb.AppendLine(ex.Message);

                        if (ex.LoaderExceptions.Any())
                        {
                            sb.AppendLine();
                            sb.AppendLine("Dependências não encontradas:");

                            foreach (var le in ex.LoaderExceptions)
                            {
                                sb.AppendLine($"- {le.Message}");

                                if (le is FileNotFoundException fnf)
                                    sb.AppendLine($"  DLL: {fnf.FileName}");
                            }
                        }

                        OnErroReceived(sb.ToString());
                    }
                    catch (Exception ex)
                    {
                        OnErroReceived("Erro inesperado:\n" + ex.Message);
                    }
                }
            }));
        }
        private void DeleteProject(string name, string path)
        {
            Project project = GetProject(name, path);
            if (project != null)
            {
                this.dispatcher.Invoke(new Action(() =>
                {
                    Projects.Remove(project);
                    LoadPinnedCommands();
                }));
            }
        }
        private void ChangesProject(string name, string path)
        {
            Project project = GetProject(name, path);
            if (project != null)
            {
                this.dispatcher.Invoke(new Action(() =>
                {
                    //Projects.Remove(project);
                    CreateProject(name, path);
                }));
            }
        }
        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType.Equals(WatcherChangeTypes.Created))
                CreateProject(e.Name, e.FullPath);
            if (e.ChangeType.Equals(WatcherChangeTypes.Deleted))
                DeleteProject(e.Name, e.FullPath);
            if (e.ChangeType.Equals(WatcherChangeTypes.Changed))
                ChangesProject(e.Name, e.FullPath);
        }

        internal void PrepareSearch()
        {
            if (this.CommandSearch == null)
            {
                this.CommandSearch = new CommandSearch();
                this.CommandSearch.projects = this.projects;
            }


        }

        internal void CloseSearch()
        {
            this.CommandSearch = null;
        }
        private void OnErroReceived(string msg)
        {
            if (ErroReceived != null)
                ErroReceived(msg);
        }
    }
}
