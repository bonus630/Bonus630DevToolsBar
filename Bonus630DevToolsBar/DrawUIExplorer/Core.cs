using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer
{
    public class Core : IDisposable
    {
        public XMLDecoder XmlDecoder { get; protected set; }
        WorkspaceUnzip workspaceUnzip;
        public string WorkerFolder { get; private set; }


        Thread t;

        public event Action<string> LoadXmlFinish;
        public event Action<bool> RequestUIHideVisibleChanged;
        public event Action LoadListsFinish;
        public event Action<bool, string> LoadStarting;
        public event Action<string> LoadFinish;
        public event Action<int> FilePorcentLoad;
        //public event Action<string> ErrorFound;
        public event Action<IBasicData> SearchResultEvent;
        //public event Action<List<object>> GenericSearchResultEvent;
        public event Action<string, MsgType> NewMessage;
        //public event PropertyChangedEventHandler PropertyChanged;
        public event Action<IBasicData> CurrentBasicDataChanged;
        public event Action<bool> InCorelChanged;
        public IBasicData ListPrimaryItens { get; set; }
        public IBasicData CurrentBasicData
        {
            get { return this.currentData; }
            set
            {
                this.currentData = value;
                if (CurrentBasicDataChanged != null) CurrentBasicDataChanged(this.currentData);
            }
        }
        public SearchEngine SearchEngineGet { get { return this.searchEngine; } }
        private SearchEngine searchEngine;
        private InputCommands inputCommands;
        private IBasicData currentData;
        private Corel.Interop.VGCore.Application app;

        public Corel.Interop.VGCore.Application CorelApp
        {
            get { return app; }
            set { app = value; }
        }
        public string FilePath { get; private set; }

        private bool inCorel;
        public bool InCorel
        {
            get { return inCorel; }
            set
            {
                inCorel = value;
                OnInCorelChanged(value);
            }
        }
        public string Title { get; private set; }
        public string IconsFolder { get; private set; }
        public CorelAutomation CorelAutomation { get; private set; }
        public ResourcesExtractor ResourcesExtractor { get; private set; }
        public HighLightItemHelper HighLightItemHelper { get; private set; }
        public List<IBasicData> Route { get { return getRoute(); } }
        public bool SetUIVisible { set { if (RequestUIHideVisibleChanged != null) RequestUIHideVisibleChanged(value); } }

        public IntPtr MainWindowHandler { get; internal set; }

        public Core()
        {
            WorkerFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bonus630\\DrawUIExplorer";
            try
            {
                if (!Directory.Exists(WorkerFolder))
                    Directory.CreateDirectory(WorkerFolder);
            }
            catch (IOException ioE)
            {
                DispactchNewMessage(ioE.Message, MsgType.Erro);
                return;
            }
        }
        private List<IBasicData> getRoute()
        {
            List<IBasicData> temp = new List<IBasicData>();
            temp.Add(CurrentBasicData);
            iRoute(temp);
            temp.Reverse();
            return temp;
        }
        private void iRoute(List<IBasicData> c)
        {
            try
            {
                IBasicData basicData = c[c.Count - 1];
                if (basicData != null && basicData.Parent != null)
                {

                    c.Add(basicData.Parent);
                    iRoute(c);
                }
            }
            catch { }
        }
        public void StartCore(string filePath, Corel.Interop.VGCore.Application corelApp)
        {
            if (corelApp != null)
            {
                this.app = corelApp;
                InCorel = true;
                //startCoreInCorel(corelApp);
            }
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;

                string newPath = WorkerFolder + "\\" + fileOri.Name;
                if (File.Exists(newPath))
                    File.Delete(newPath);
                file = fileOri.CopyTo(newPath);
            }
            catch (IOException ioErro)
            {
                DispactchNewMessage(ioErro.Message, MsgType.Erro);
                return;
            }
            inputCommands = new InputCommands(this);
            XmlDecoder = new XMLDecoder();
            XmlDecoder.LoadFinish += XmlDecoder_LoadFinish;
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);
        }
        private void startCoreInCorel(Corel.Interop.VGCore.Application corelApp)
        {
            CorelAutomation = new CorelAutomation(this);
            this.app = corelApp;
            this.HighLightItemHelper = new HighLightItemHelper(this.CorelAutomation);
            corelApp.OnApplicationEvent -= CorelApp_OnApplicationEvent;
            corelApp.OnApplicationEvent += CorelApp_OnApplicationEvent;
            //Teste get guids from resources
            ResourcesExtractor = new ResourcesExtractor(CorelApp.ProgramPath);
            SaveIcons();
            ResourcesExtractor.GuidsIsLoaded -= ResourcesExtractor_GuidsIsLoaded;
            ResourcesExtractor.GuidsIsLoaded += ResourcesExtractor_GuidsIsLoaded;
        }

        /// <summary>
        /// Use this to performace search in a pierce of xml
        /// </summary>
        public void PartialStart(string filePath, Corel.Interop.VGCore.Application corelApp)
        {
            this.app = corelApp;
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;

                string newPath = WorkerFolder + "\\" + fileOri.Name;
                if (File.Exists(newPath))
                    File.Delete(newPath);
                file = fileOri.CopyTo(newPath);
            }
            catch (IOException ioErro)
            {
                DispactchNewMessage(ioErro.Message, MsgType.Erro);
                return;
            }
            XmlDecoder = new XMLDecoder();
            XmlDecoder.LoadFinish += XmlDecoder_LoadFinish;
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);

        }
        public void PartialStart(List<string> filesPath, Corel.Interop.VGCore.Application corelApp, List<string> listTags, bool useCache = true)
        {
            //vamos refatoral isso depois
            this.app = corelApp;
            FileInfo file = null;
            XmlDecoder = new XMLDecoder();
            try
            {
                FileInfo[] fileOri = new FileInfo[filesPath.Count];
                Title = filesPath[0];

                string newPath = WorkerFolder + "\\partial" + corelApp.VersionMajor + ".xml";
                if (!useCache && File.Exists(newPath))
                    File.Delete(newPath);
                if (!useCache || !File.Exists(newPath))
                {
                    List<string> files = new List<string>();
                    for (int i = 0; i < filesPath.Count; i++)
                    {
                        string t = WorkerFolder + "\\partial" + i.ToString("00") + corelApp.VersionMajor + ".xml";
                        if (!useCache && File.Exists(t))
                            File.Delete(t);
                        try
                        {
                            XmlDecoder.ExtractTagsToNewXml(filesPath[i], t, listTags);
                            files.Add(t);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                    XmlDecoder.MergeXmlFilesForTags(listTags, files, newPath);
                }
                file = new FileInfo(newPath);
            }
            catch (IOException ioErro)
            {
                DispactchNewMessage(ioErro.Message, MsgType.Erro);
                return;
            }
            XmlDecoder.LoadFinish += XmlDecoder_LoadFinish;
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);
        }
        private void OnInCorelChanged(bool inCorel)
        {
            if (inCorel)
                startCoreInCorel(this.CorelApp);
            if (InCorelChanged != null)
                InCorelChanged(inCorel);
        }
        private void ResourcesExtractor_GuidsIsLoaded(Dictionary<UInt16, List<string>> obj)
        {
            guidIsLoaded = true;
            TestGetResourceRCDATA(obj);
        }
        private Dictionary<UInt16, List<string>> guids = new Dictionary<ushort, List<string>>();
        public void SetIcon(IBasicData basicData, bool ignoreError = true)
        {
            if (!string.IsNullOrEmpty(basicData.Icon))
            {
                return;
            }
            string guid = basicData.GetAnyGuidAttribute();
            if (string.IsNullOrEmpty(guid))
            {
                if (ignoreError)
                    return;
                else
                    DispactchNewMessage("The guid is empty!", MsgType.Erro);
            }
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            //if (!ignoreError && guids == null)
            //{
            //    DispactchNewMessage("Guids list is null", MsgType.Erro);
            //    return;
            //}
            if (!ignoreError && string.IsNullOrEmpty(IconsFolder))
            {
                DispactchNewMessage("IconsFolders is invalid!", MsgType.Erro);
                return;
            }

            //Process.Start(IconsFolder);
            //System.Windows.Forms.MessageBox.Show(guid);

            foreach (var item in guids)
            {
                //if(item.Key == 22)
                //    basicData.Icon = string.Format("{0}\\{1}.png", IconsFolder, item.Key);


                if (item.Value.Contains(guid))
                    dispatcher.Invoke(new Action(() =>
                    {
                        //#if X8 || X7
                        basicData.Icon = string.Format("{0}\\{1}.png", IconsFolder, item.Key);
                        //#else
                        //                        basicData.Icon = string.Format("{0}\\{1}.png", IconsFolder, item.Key+1);
                        //#endif
                        return;
                    }));
            }
        }
        private void TestGetResourceRCDATA(Dictionary<UInt16, List<string>> obj)
        {
            guids = obj;
        }
        public void MergeProcess(string filePath)
        {
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;

                string newPath = WorkerFolder + "\\" + fileOri.Name;
                if (File.Exists(newPath))
                    File.Delete(newPath);
                file = fileOri.CopyTo(newPath);
            }
            catch (IOException ioErro)
            {
                DispactchNewMessage(ioErro.Message, MsgType.Erro);
                return;
            }
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);
        }
        private void CorelApp_OnApplicationEvent(string EventName, ref object[] Parameters)
        {
            try
            {
                string eventName = EventName;
                for (int i = 0; i < Parameters.Length; i++)
                {
                    eventName += "\n\t Param[" + i + "] => " + Parameters[i].GetType() + " = " + Parameters[i].ToString() + ";";
                }
                DispactchNewMessage(eventName, MsgType.Event);
            }
            catch (Exception erro)
            {
                DispactchNewMessage(erro.Message, MsgType.Console);
            }
        }
        public string RunCommand(string commandName)
        {
            try
            {
                //commandName = commandName.ToLowerInvariant();
                string result = "";
                commandName = commandName.Trim(' ', '\r', '\n', '\t');
                string[] pierces = commandName.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int j = 0;
                List<object> param = new List<object>();
                commandName = pierces[0];

                bool isQuote = false;
                for (int i = 1; i < pierces.Length; i++)
                {
                    if (isQuote)
                        param[param.Count - 1] = param[param.Count - 1] + " " + pierces[i];
                    else
                        param.Add(pierces[i]);
                    if (pierces[i].StartsWith("\""))
                        isQuote = true;
                    if (pierces[i].EndsWith("\""))
                        isQuote = false;
                    if (pierces[i].Contains("\"") && pierces[i].Length < 2)
                    {

                        return "Badly formed command, you cannot start or end a string with spaces";
                    }
                }

                if (isQuote)
                {
                    return "Badly formed command, check the quotes";
                }

                bool commandNotFound = true;
                var commands = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < commands.Length; i++)
                {
                    if (commands[i].Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        commandNotFound = false;
                        int parCount = commands[i].GetParameters().Length;
                        if (parCount != param.Count)
                        {
                            return "Badly formed command, number of parameters is incorrect";
                        }
                        try
                        {
                            object o = commands[i].Invoke(inputCommands, param.ToArray());
                            if (o != null)
                                result = o.ToString();
                            else
                                result = "This command has no return";
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                if (commandNotFound)
                    return string.Format("\"{0}\" It is not a recognized command, type \"Help\" for a list of valid commands", commandName);
                return result;
            }
            catch (Exception err)
            {
                return err.Message;
            }

        }
        public bool IsXMLString(string text)
        {
            return XmlDecoder.IsXMLString(text);
        }
        public string FormatXml(string xml)
        {
            return XmlDecoder.Beautify(xml);
        }
        private void XmlDecoder_LoadFinish()
        {
            if (this.LoadXmlFinish != null)
                LoadXmlFinish(XmlDecoder.XmlString);

        }

        /// <summary>
        /// Dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        public void FindByGuid(ObservableCollection<IBasicData> list, string guid)
        {
            searchEngine.NewSearch();
            searchEngine.SearchItemFromGuidRef(list, guid);
        }

        public List<Type> GetIUnknown(IBasicData basicData)
        {
            List<Type> types = new List<Type>();
            try
            {
                types = CorelAutomation.GetIUnknown(GetParentGuid(basicData), basicData.Guid);
            }
            catch { }
            return types;
        }
        public string GetParentGuid(IBasicData basicData)
        {
            IBasicData parentBasicData = basicData;
            string guidItem = basicData.Guid;
            if (string.IsNullOrEmpty(guidItem) && basicData.TagName.Equals("item"))
            {
                guidItem = basicData.GuidRef;
            }
            if (!parentBasicData.IsContainer)
            {
                parentBasicData = SearchEngineGet.SearchItemContainsGuidRef(ListPrimaryItens, guidItem, false);
                if (parentBasicData != null)
                {
                    while ((string.IsNullOrEmpty(parentBasicData.Guid) && parentBasicData.Parent != null) || parentBasicData.GetType() == typeof(OtherData))
                    {
                        //neste momento temos a referencia do item correto
                        parentBasicData = parentBasicData.Parent;
                    }

                }
            }
            return parentBasicData.Guid;
        }

        /// <summary>
        /// Dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        public void FindItemContainsGuidRef(IBasicData list, string guid)
        {
            searchEngine.NewSearch();
            searchEngine.SearchItemContainsGuidRefEvent(list, guid);
        }
        /// <summary>
        /// Não dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IBasicData SearchItemFromGuidRef(IBasicData list, string guid)
        {

            return searchEngine.SearchItemFromGuidRef(list, guid);
        }
        public IBasicData SearchItemContainsGuidRef(string guid)
        {
            return searchEngine.SearchItemContainsGuidRef(this.ListPrimaryItens, guid);
        }
        public IBasicData SearchItemContainsGuidRef(IBasicData list, string guid)
        {
            return searchEngine.SearchItemContainsGuidRef(list, guid);
        }
        public void FindAllTags(IBasicData basicData)
        {
            searchEngine.NewSearch();
            searchEngine.SearchAllTags(basicData);
        }

        public string GetXml(IBasicData basicData, bool xmlHeader = true)
        {
            return XmlDecoder.GetXml(basicData, xmlHeader);
        }
        private void SearchEngine_SearchResultEvent(IBasicData obj)
        {
            if (SearchResultEvent != null)
                SearchResultEvent(obj);


        }

        private void LoadFile(object param)
        {

            FileInfo file = param as FileInfo;
            StreamReader fs = null;
            if (file == null)
            {
                DispactchNewMessage("Load file erro", MsgType.Erro);
                return;
            }
            if (LoadStarting != null)
                LoadStarting(false, "Loading xml file to memory");

            if (file.Extension == ".cdws")
            {
                try
                {
                    workspaceUnzip = new WorkspaceUnzip(file);
                    fs = workspaceUnzip.XmlStreamReader;
                }
                catch (Exception e)
                {
                    DispactchNewMessage(e.Message, MsgType.Erro);
                    return;
                }
            }
            else
            {
                fs = new StreamReader(file.FullName);
            }
            StringBuilder sb = new StringBuilder();
            string line;
            double length = file.Length;
            double totalPos = 0.1;
            while ((line = fs.ReadLine()) != null)
            {
                sb.Append(line);
                totalPos = sb.Length;
                int porcent = (int)(totalPos * 100 / length);
                if (FilePorcentLoad != null)
                    FilePorcentLoad(porcent);
            }
            XmlDecoder.XmlString = sb.ToString();
            if (LoadStarting != null)
                LoadStarting(true, "Deserializing xml");
            sb = null;
            fs.Close();
            fs.Dispose();
            if (workspaceUnzip != null)
                workspaceUnzip.Dispose();
            try
            {
                XmlDecoder.Process(file.FullName);
            }
            catch (Exception erro)
            {
                DispactchNewMessage(erro.Message, MsgType.Erro);
                return;
            }

            ListPrimaryItens = XmlDecoder.FirstItens;
            searchEngine = new SearchEngine();
            searchEngine.SearchResultEvent += SearchEngine_SearchResultEvent;
            searchEngine.GenericSearchResultEvent += SearchEngine_GenericSearchResultEvent;
            searchEngine.SearchStarting += SearchEngine_StartSearch;
            searchEngine.SearchFinished += SearchEngine_SearchFinished;
            searchEngine.SearchMessage += SearchEngine_SearchMessage;

            if (LoadListsFinish != null)
                LoadListsFinish();
            FilePath = file.FullName;
            //Teste para pegar os guids do RCDATA
            if (InCorel)
                ResourcesExtractor.GetGuids();
            //listIsLoaded = true;
            //TestGetResourceRCDATA();

        }
        private void SaveIcons()
        {
            IconsFolder = Path.Combine(WorkerFolder, string.Format("Icons_CDR{0}", CorelApp.VersionMajor));
            string fileName = Path.Combine(CorelApp.AddonPath, "Bonus630DevToolsBar\\IconsExtractor.exe");
            if (!File.Exists(fileName))
            {
                DispactchNewMessage("You missing IconsExtractor.exe!", MsgType.Erro);
                return;
            }

            if (!Directory.Exists(IconsFolder))
                Directory.CreateDirectory(IconsFolder);
            DispactchNewMessage("Save icons in: " + IconsFolder, MsgType.Console);

            Action saveIcons = new Action(() =>
            {
                // ResourcesExtractor.SaveIcons(IconsFolder);
                Process psi = new Process();
                // ProcessStartInfo psi = new ProcessStartInfo();
                psi.StartInfo.CreateNoWindow = true;
                psi.StartInfo.UseShellExecute = false;
                psi.EnableRaisingEvents = true;
                psi.StartInfo.FileName = fileName;
                psi.StartInfo.Arguments = string.Format("\"{0}|{1}\"", CorelApp.ProgramPath, IconsFolder);
                //psi.StartInfo.RedirectStandardOutput = true;

                psi.Start();
            });
            Action saveIcons2 = new Action(() =>
            {
                try
                {
                    ResourcesExtractor.SaveIcons(IconsFolder);
                }
                catch (Exception e)
                {
                    DispactchNewMessage(e.Message, MsgType.Erro);
                }
            });
            t = new Thread(new ThreadStart(saveIcons2));
            t.IsBackground = true;
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }
        private bool listIsLoaded = false;
        private bool guidIsLoaded = false;
        private int c = 1;

        public void DispactchNewMessage(string message, MsgType msgType)
        {

            if (NewMessage != null)
                NewMessage(message, msgType);
        }
        public void DispactchNewMessage(string message, MsgType msgType, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
                DispactchNewMessage(message, msgType);
            }

        }
        private void SearchEngine_SearchMessage(string obj)
        {

        }

        private void SearchEngine_SearchFinished()
        {
            if (LoadFinish != null)
                LoadFinish("Finished");
        }

        private void SearchEngine_GenericSearchResultEvent(List<object> obj)
        {

        }

        private void SearchEngine_StartSearch()
        {
            if (LoadStarting != null)
                LoadStarting(true, "Searching");
        }

        //public void MergeData(IBasicData mainData, IBasicData toMergeData)
        //{
        //    if(mainData.Equals(toMergeData))
        //    {
        //        if (toMergeData.Childrens != null)
        //        {
        //            for (int i = 0; i < toMergeData.Childrens.Count; i++)
        //            {
        //                if(mainData.Childrens.Contains)
        //            }
        //        }
        //    }
        //    else
        //    {
        //        mainData.Add(toMergeData);
        //    }
        //}
        public string TryGetAnyCaption(string itemGuid)
        {
            return TryGetAnyCaption(itemGuid, this.ListPrimaryItens);
           
        }
        public string TryGetAnyCaption(string itemGuid, IBasicData basicData)
        {
            IBasicData data = searchEngine.SearchItemFromGuid(basicData, itemGuid);
            if (data != null)
                return TryGetAnyCaption(data);
            return string.Empty;
        }
        public string TryGetAnyCaption(IBasicData basicData)
        {
            string caption = "";

            if (!string.IsNullOrEmpty(basicData.Caption))
            {
                caption = basicData.Caption;

                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            if (InCorel)
            {
                caption = this.CorelAutomation.GetItemCaption(basicData.Guid);

                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            string[] searchs = new string[] { "captionGuid", "guidRef" };
            for (int i = 0; i < searchs.Length; i++)
            {
                if (basicData.ContainsAttribute(searchs[i]))
                {
                    string guid = basicData.GetAttribute(searchs[i]);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        caption = TryGetAnyCaption(this.SearchItemFromGuidRef(this.ListPrimaryItens, guid));

                    }
                }
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            string search = "nonLocalizableName";
            if (basicData.ContainsAttribute(search))
            {
                caption = basicData.GetAttribute(search);

                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            return caption;
        }
        public static DependencyObject FindParentControl<T>(DependencyObject el) where T : DependencyObject
        {
            if (el == null)
                return null;
            DependencyObject parent = VisualTreeHelper.GetParent(el);
            if (parent is T)
            {
                return parent;
            }
            else
            {
                return FindParentControl<T>(parent);
            }
        }

        internal void RunEditor(IBasicData basicData)
        {
            try
            {
                int line = XmlDecoder.GetLineNumber(basicData);
                if (line == -1)
                {
                    DispactchNewMessage("Error when trying to find the line number", MsgType.Erro);
                    return;
                }
                var process = new Process();
                if (!File.Exists(Properties.Settings.Default.Editor))
                {
                    DispactchNewMessage("Editor File not found", MsgType.Erro);
                    return;
                }
                process.StartInfo.FileName = Properties.Settings.Default.Editor;
                //"{0} -n{1}"
                object[] args = { FilePath, line };
                process.StartInfo.Arguments = string.Format(Properties.Settings.Default.EditorArguments, args);
                process.Start();
            }
            catch (Exception erro)
            {
                DispactchNewMessage(erro.Message, MsgType.Erro);
            }

        }

        public void Dispose()
        {
            if (ListPrimaryItens != null)
            {
                ListPrimaryItens.Childrens.Clear();
                ListPrimaryItens = null;
            }
            inputCommands = null;
            //commands = null;
            XmlDecoder = null;
            CorelAutomation = null;
            this.HighLightItemHelper = null;
            ResourcesExtractor = null;
            Dispose();
        }

    }

    public enum MsgType
    {
        Console,
        Event,
        Xml,
        Erro,
        Result,
        Link
    }

}
