using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;
using Bonus630DevToolsBar;
using System.Windows.Media;
using System.Windows.Documents;
using System.IO.Compression;
using System.Xml;
using System.Windows.Media.TextFormatting;
using SharpCompress.Compressors.Xz;


namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public partial class DockerUI : UserControl
    {
        private corel.Application corelApp;
        //private object corelObj;
        private Styles.StylesController stylesController;
        private readonly string ProjectsFolder;


        ProxyManager proxyManager;
        ProjectsManager projectsManager;
        ShapeRangeManager shapeRangeManager;
        ProjectCreator projectCreator = new ProjectCreator();

        public DockerUI(object app)
        {
            InitializeComponent();
            ProjectsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bonus630\\Projects");
            try
            {

                proxyManager = new ProxyManager(app, System.IO.Path.Combine((app as corel.Application).AddonPath, "Bonus630DevToolsBar"));
                this.corelApp = app as corel.Application;
                stylesController = new Styles.StylesController(this.Resources, this.corelApp);
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
            this.Loaded += DockerUI_Loaded;

            shapeRangeManager = new ShapeRangeManager(this.corelApp);

            AppDomain.CurrentDomain.AssemblyLoad += LoadDomain_AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }

        private void DockerUI_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.PinnedCommands == null)
                Properties.Settings.Default.PinnedCommands = new System.Collections.Specialized.StringCollection();

            projectsManager = new ProjectsManager(this.Dispatcher);
            projectsManager.shapeRangeManager = shapeRangeManager;
            projectsManager.Start(proxyManager);
            this.DataContext = projectsManager;

            txt_projectName.TextChanged += (s, ev) => ChangeProjectDirectory();
            cb_projectType.SelectionChanged += (s, ev) => ChangeProjectDirectory();
        }



        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //  try
            //   {
            if (args.RequestingAssembly != null)
                return args.RequestingAssembly;
            Assembly asm = null;
            string name = args.Name;
            if (name.Contains(".resources"))
                name = name.Replace(".resources", "");
            asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(r => string.Equals(r.FullName.Split(',')[0], name.Split(',')[0]));
            if (args.Name.Contains(".resources"))
                asm = LoadResourceAssembly(asm);
            if (asm == null)
                asm = Assembly.LoadFrom(Name);
            return asm;
            //}
            //catch(IOException ioe)
            //{
            //    Logger.Log(ioe);

            //}
            //catch(Exception e)
            //{
            //    Logger.Log(e);

            //}
            //return null;
        }
        private Assembly LoadResourceAssembly(Assembly executingAsm)
        {
            if (executingAsm == null)
                return executingAsm;
            string[] resourcesName = executingAsm.GetManifestResourceNames();
            string resourceName = string.Empty;
            for (int i = 0; i < resourcesName.Length; i++)
            {
                if (!resourcesName[i].Contains(".g."))
                {
                    resourceName = resourcesName[i];
                    break;
                }
            }
            if (string.IsNullOrEmpty(resourceName))
                return executingAsm;
            using (var stream = executingAsm.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            }
            return executingAsm;
        }
        private void LoadDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Debug.WriteLine("AssemblyLoad sender:{0} args{1}", sender, args.LoadedAssembly.CodeBase);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            stylesController.LoadThemeFromPreference();
            projectCreator.DataReceived += Pc_DataReceived;
            projectCreator.ErroReceived += Pc_ErroReceived;
            projectCreator.VgCore = corelApp.ProgramPath + "Assemblies\\Corel.Interop.VGCore.dll";
            projectCreator.AddonFolder = Path.Combine(corelApp.AddonPath, "Bonus630DevToolsBar");
        }

        private void Pc_ErroReceived(string obj)
        {
            txt_log.Dispatcher.Invoke(() =>
            {
                popup_log.IsOpen = true;
                TextRange tr;
                txt_log.BeginChange();

                tr = new TextRange(txt_log.Document.ContentEnd, txt_log.Document.ContentEnd);
                tr.Text = string.Format("{0}\r", obj);
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Crimson);

                txt_log.EndChange();
                txt_log.ScrollToEnd();

            });
        }

        private void Pc_DataReceived(string obj)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextRange tr;
                txt_log.BeginChange();

                tr = new TextRange(txt_log.Document.ContentEnd, txt_log.Document.ContentEnd);
                tr.Text = string.Format("{0}\r", obj);


                txt_log.EndChange();

                //txt_log.AppendText(obj);
                //txt_log.AppendText(Environment.NewLine);
                txt_log.ScrollToEnd();
            });
        }

        private void btn_selectFolder_Click(object sender, RoutedEventArgs e)
        {
            if (projectsManager.SelectAssembliesFolder())
            {

                projectsManager.SetAssembliesFolder(ProjectsFolder, projectCreator);
            }
        }

        private void btn_openFolder_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.OpenFolder();
        }

        private void btn_addSelection_Click(object sender, RoutedEventArgs e)
        {
            shapeRangeManager.AddActiveSelection();
        }

        private void btn_removeSelection_Click(object sender, RoutedEventArgs e)
        {
            shapeRangeManager.RemoveActiveSelection();
        }

        private void btn_clearRange_Click(object sender, RoutedEventArgs e)
        {
            shapeRangeManager.Clear();
        }

        private void Label_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            projectsManager.MyPopupIsOpen = true;
        }

        private void MyPopup_PopupCloseEvent()
        {
            projectsManager.MyPopupIsOpen = false;
        }
        private void img_fails_MouseOver(object sender, System.Windows.Input.MouseEventArgs e)
        {
            projectsManager.MyPopupExceptionIsOpen = true;
        }

        private void MyPopup_PopupCloseExceptionEvent()
        {
            projectsManager.MyPopupExceptionIsOpen = false;
        }
        //Ref.:01 
        // Compare to another Ref.:01
        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                projectsManager.SelectedCommand = (sender as TreeView).SelectedItem as Command;

                //if(Application.Current.Dispatcher.che)
                //    ((sender as TreeView).SelectedItem as TreeViewItem).BringIntoView();
                //else
                //{
                //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                //     {
                //        ((sender as TreeView).SelectedItem as TreeViewItem).BringIntoView();
                //    }));
                ////}

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void ScrollToSelectedItem()
        {
            var selectedItem = treeViewCommands.SelectedItem as Command; // Substitua YourItemType pelo tipo real dos itens no seu TreeView
            if (selectedItem != null)
            {
                // Encontrar o item correspondente no TreeView
                var container = treeViewCommands.ItemContainerGenerator.ContainerFromItem(selectedItem) as FrameworkElement;

                if (container != null)
                {
                    // Obter o ScrollViewer interno do TreeView
                    var scrollViewer = FindVisualChild<ScrollViewer>(treeViewCommands);

                    if (scrollViewer != null)
                    {
                        // Calcular o deslocamento para rolar até o item
                        var offset = container.TranslatePoint(new System.Windows.Point(0, 0), scrollViewer);

                        // Rolar para o item
                        scrollViewer.ScrollToVerticalOffset(offset.Y);
                    }
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject visual) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(visual, i);
                if (child != null && child is T)
                    return (T)child;

                T childItem = FindVisualChild<T>(child);
                if (childItem != null)
                    return childItem;
            }
            return null;
        }


        private void btn_newProject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(projectsManager.AssemblyDirectory))
            {
                projectsManager.SelectAssembliesFolder();
                return;
            }
            popup_newProject.IsOpen = !popup_newProject.IsOpen;
        }


        private void btn_buildProject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(projectCreator.LastProject))
            {
                btn_setProject_Click(null, null);
            }
            if (!string.IsNullOrEmpty(projectCreator.LastProject))
            {
                //popup_log.IsOpen = true;
                //pc.MSBuild();
                projectCreator.DirectBuild();
            }

        }

        private void btn_createProject_Click(object sender, RoutedEventArgs e)
        {
            int index = cb_projectType.SelectedIndex;
            if (index > -1
                && !string.IsNullOrEmpty(txt_projectFolder.Text)
                && !string.IsNullOrEmpty(txt_projectName.Text))
            {
                projectCreator.Index = index;
                projectCreator.SetProjectName(txt_projectName.Text);

                projectCreator.ProjectFolder = CheckProjectFolder(txt_projectFolder.Text);

                if (string.IsNullOrEmpty(this.projectsManager.AssemblyDirectory))
                    this.projectsManager.SelectAssembliesFolder();
                projectCreator.AssembliesFolder = this.projectsManager.AssemblyDirectory;

                projectCreator.ExtractTemplate();


                projectCreator.ReplaceFiles();
                // popup_log.IsOpen = true;
                projectCreator.MSBuild();

            }
            Reset();
        }

        private string CheckProjectFolder(string projectPath)
        {
            int cont = 1;
            while (Directory.Exists(projectPath) && Directory.GetFiles(projectPath).Length > 0)
            {
                if (cont == 1)
                    projectPath += cont.ToString("0000");
                else
                    projectPath = projectPath.Substring(0, projectPath.Length - 4) + cont.ToString("0000");
                cont++;
            }
            if (!Directory.Exists(projectPath))
                Directory.CreateDirectory(projectPath);
            return projectPath;
        }

        private void btn_importCGSAddon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.Title = "Select CGSaddon file";
                ofd.Multiselect = false;
                ofd.Filter = "CGSAddon (*.CGSaddon)|*.CGSaddon";
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                ZipFile.ExtractToDirectory(ofd.FileName, tempPath);
                tempPath = string.Format("{0}\\content", tempPath);


                if (!Directory.Exists(tempPath))
                {
                    corelApp.MsgShow("Project not found!");
                    return;
                }

                string projectPath = Path.Combine(ProjectsFolder, "cs", Path.GetFileName(ofd.FileName));
                projectPath = CheckProjectFolder(projectPath);
                //System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                //8
                //fbd.Description = "Select a  folder for your Project!";
                //if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    try
                //    {
                //        string testDir = Path.Combine(fbd.SelectedPath, "test.txt");
                //        using (File.Create(testDir)) { }
                //        txt_projectFolder.Text = fbd.SelectedPath;
                //        File.Delete(testDir);
                //    }
                //    catch (IOException ioe)
                //    {
                //        corelApp.MsgShow("Directory access limited, please choose another!");
                //    }
                //}
                //else
                //    return;
                string[] files = Directory.GetFileSystemEntries(tempPath);
                if (files.Length == 1)
                    files = Directory.GetFileSystemEntries(files[0]);

                string csprojFile = string.Empty;

                foreach (string item in files)
                {
                    string fileName = Path.GetFileName(item);
                    string destinoItem = Path.Combine(projectPath, fileName);

                    if (string.IsNullOrEmpty(this.projectsManager.AssemblyDirectory))
                        this.projectsManager.SelectAssembliesFolder();
                    projectCreator.AssembliesFolder = this.projectsManager.AssemblyDirectory;
                    if (File.Exists(item))
                    {

                        if (Path.GetExtension(item).Equals(".csproj"))
                        {
                            csprojFile = destinoItem;
                            projectCreator.PrepareGSAddonProj(item, csprojFile);
                        }
                       // else
                    //    {
                            File.Copy(item, destinoItem, true);
                    //    }
                    }
                    else if (Directory.Exists(item))
                    {
                        CopyDirectory(item, destinoItem);
                    }
                }
                if (string.IsNullOrEmpty(csprojFile))
                {
                    corelApp.MsgShow("Project not found!");
                    return;
                }

                projectCreator.LastProject = csprojFile;
                Properties.Settings.Default.LastProject = projectCreator.LastProject;
                Properties.Settings.Default.Save();
                //popup_log.IsOpen = true;
                projectCreator.MSBuild();
            }
            catch (Exception ep)
            {
                corelApp.MsgShow(ep.Message);
            }
        }

        private void Reset()
        {
            popup_newProject.IsOpen = false;
            cb_projectType.SelectedIndex = 0;
            txt_projectFolder.Text = "";
            txt_projectName.Text = "";
            txt_moduleName.Text = "";
            projectsManager.RequestNewModule = false;
        }
        private void btn_selectProjectFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(txt_projectFolder.Text))
                System.Diagnostics.Process.Start(txt_projectFolder.Text);
            return;

            popup_newProject.IsOpen = !popup_newProject.IsOpen;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "Select a  folder for your Project!";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string testDir = Path.Combine(fbd.SelectedPath, "test.txt");
                    using (File.Create(testDir)) { }
                    txt_projectFolder.Text = fbd.SelectedPath;
                    File.Delete(testDir);
                }
                catch (IOException ioe)
                {
                    corelApp.MsgShow("Directory access limited, please choose another!");
                }
            }
            popup_newProject.IsOpen = !popup_newProject.IsOpen;
        }

        private void btn_cancelProject_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void btn_setProject_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Proj (*.csproj,*.vbproj)|*.csproj;*.vbproj";
            ofd.InitialDirectory = ProjectsFolder;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                projectCreator.LastProject = ofd.FileName;
                Properties.Settings.Default.LastProject = projectCreator.LastProject;
                Properties.Settings.Default.Save();
            }
        }

        private void btn_close_popupLog_Click(object sender, RoutedEventArgs e)
        {
            popup_log.IsOpen = false;
            txt_log.Document.Blocks.Clear();
        }
        // Função recursiva para copiar diretórios
        private void CopyDirectory(string origem, string destiny)
        {

            if (!Directory.Exists(destiny))
            {
                Directory.CreateDirectory(destiny);
            }


            string[] files = Directory.GetFileSystemEntries(origem);

            foreach (string item in files)
            {
                string source = Path.GetFileName(item);
                string destinoItem = Path.Combine(destiny, source);

                if (File.Exists(item))
                {
                    File.Copy(item, destinoItem, true);

                }
                else if (Directory.Exists(item))
                {
                    CopyDirectory(item, destinoItem);

                }
            }
        }


        #region Search Events

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            projectsManager.CommandSearch.Search(textBoxSearch.Text);
        }

        private void buttonPreview_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.CommandSearch.Navegate(-1);
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.CommandSearch.Navegate(1);
        }



        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.CloseSearch();
            gridSearchBox.Visibility = Visibility.Collapsed;
        }

        private void RadioButtonCondition_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.CommandSearch.termPosition = Int32.Parse((string)(sender as RadioButton).Tag);
            projectsManager.CommandSearch.Search(textBoxSearch.Text, true);
        }
        private void OpenSearch()
        {
            gridSearchBox.Visibility = Visibility.Visible;
            textBoxSearch.Focus();
            projectsManager.PrepareSearch();
            if (!string.IsNullOrEmpty(textBoxSearch.Text))
                projectsManager.CommandSearch.Search(textBoxSearch.Text, true);
        }
        bool control = false;
        private void treeViewCommands_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftCtrl)
                control = true;
        }

        private void treeViewCommands_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftCtrl)
                control = false;
            if (control && e.Key == System.Windows.Input.Key.F)
                OpenSearch();
        }

        private void checkBoxWholeWord_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.CommandSearch.wholeWord = (bool)checkBoxWholeWord.IsChecked;
            projectsManager.CommandSearch.Search(textBoxSearch.Text, true);
        }

        private void checkBoxMatchCase_Click(object sender, RoutedEventArgs e)
        {
            projectsManager.CommandSearch.matchCase = (bool)checkBoxMatchCase.IsChecked;
            projectsManager.CommandSearch.Search(textBoxSearch.Text, true);
        }

        #endregion


        private void ChangeProjectDirectory()
        {
            if (string.IsNullOrEmpty(txt_projectName.Text))
                return;
            string projectLang = "cs";
            if (cb_projectType.SelectedIndex == 0)
                projectLang = "cs";
            else
                projectLang = "vb";
            char[] invalids = Path.GetInvalidPathChars();
            for (int i = 0; i < invalids.Length; i++)
            {
                txt_projectName.Text = txt_projectName.Text.Replace(invalids[i], '_');
            }
            string path = Path.Combine(ProjectsFolder, projectLang, txt_projectName.Text);
            txt_projectFolder.Text = path;
            txt_projectFolder.CaretIndex = path.Length;
        }

        private void btn_createModule_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_moduleName.Text))
            {
                FileInfo fi = new FileInfo(lba_projPath.Content.ToString());
                string templatePath;
                string ext;
                if (fi.Extension.Equals(".csproj"))
                {
                    templatePath = Path.Combine(corelApp.AddonPath, "RunCommandDocker\\Templates\\MacroClassLibraryCS.zip");
                    ext = ".cs";
                }
                else
                {
                    templatePath = Path.Combine(corelApp.AddonPath, "RunCommandDocker\\Templates\\MacroClassLibraryVB.zip");
                    ext = ".vb";
                }

                string filePath = Path.Combine(fi.Directory.FullName, txt_moduleName.Text + ext);
                using (var extractor = new TemplateExtractor(templatePath))
                {
                    extractor.ExtractFile(filePath, "Main" + ext);
                }




                projectsManager.RequestNewModule = false;
            }
        }

        private void txt_moduleName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                btn_createModule_Click(null, null);
        }
    }
}
