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
    public class Core
    {
        XMLDecoder xmlDecoder;
        WorkspaceUnzip workspaceUnzip;
        string workerFolder;
      

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
        private MethodInfo[] commands;
        public MethodInfo[] Commands { get { return commands; } }
        private InputCommands inputCommands;
        private IBasicData currentData;
        private Corel.Interop.VGCore.Application app;

        public Corel.Interop.VGCore.Application CorelApp
        {
            get { return app; }
        }
        public string FilePath { get; private set; }
        public bool InCorel { get; private set; }
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
            workerFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bonus630\\DrawUIExplorer";
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
                InCorel = true;
                CorelAutomation = new CorelAutomation(corelApp, this);
                this.app = corelApp;
                this.HighLightItemHelper = new HighLightItemHelper(this.CorelAutomation, this.app);
                corelApp.OnApplicationEvent += CorelApp_OnApplicationEvent;
                //Teste get guids from resources
                ResourcesExtractor = new ResourcesExtractor(CorelApp.ProgramPath);
                SaveIcons();
                ResourcesExtractor.GuidsIsLoaded += ResourcesExtractor_GuidsIsLoaded;
            }
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;
                try
                {
                    if (!Directory.Exists(workerFolder))
                        Directory.CreateDirectory(workerFolder);
                }
                catch (IOException ioE)
                {
                    DispactchNewMessage(ioE.Message, MsgType.Erro);
                    return;
                }
                string newPath = workerFolder + "\\" + fileOri.Name;
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
            commands = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            xmlDecoder = new XMLDecoder();
            xmlDecoder.LoadFinish += XmlDecoder_LoadFinish;
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);
        }

        private void ResourcesExtractor_GuidsIsLoaded(Dictionary<UInt16, List<string>> obj)
        {
            guidIsLoaded = true;
            TestGetResourceRCDATA(obj);
        }
        private Dictionary<UInt16, List<string>> guids;
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
                    throw new Exception("The guid is empty!");
            }
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            if (!ignoreError && guids == null)
                throw new Exception("Guids list is null");
            if (!ignoreError && string.IsNullOrEmpty(IconsFolder))
                throw new Exception("IconsFolders is invalid!");
            foreach (var item in guids)
            {
                if (item.Value.Contains(guid))
                    dispatcher.Invoke(new Action(() =>
                    {

                        basicData.Icon = string.Format("{0}\\{1}.png", IconsFolder, item.Key);
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
                try
                {
                    if (!Directory.Exists(workerFolder))
                        Directory.CreateDirectory(workerFolder);
                }
                catch (IOException ioE)
                {
                    DispactchNewMessage(ioE.Message, MsgType.Erro);
                    return;
                }
                string newPath = workerFolder + "\\" + fileOri.Name;
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
                    eventName += "\n\t Param[" + i + "] => " + Parameters[i].GetType() + " = " + Parameters[i].ToString()+";";
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
                string[] pierces = commandName.Split(" ".ToCharArray());
                int j = 0;
                List<object> param = new List<object>();
                while (!string.IsNullOrEmpty(pierces[j]) || pierces[j] == " ")
                {
                    if (!string.IsNullOrEmpty(pierces[j]) && pierces[j] != " ")
                    {
                        commandName = pierces[j];
                        j++;
                        break;
                    }
                    j++;
                    if (j >= pierces.Length)
                        break;
                }
                while (j < pierces.Length && (!string.IsNullOrEmpty(pierces[j]) || pierces[j] == " "))
                {
                    if (!string.IsNullOrEmpty(pierces[j]) && pierces[j] != " ")
                    {
                        param.Add(pierces[j]);
                    }
                    j++;
                }
                for (int i = 0; i < commands.Length; i++)
                {
                    if (commands[i].Name == commandName)
                        result = commands[i].Invoke(inputCommands, param.ToArray()).ToString();
                }
                return result;
            }
            catch (Exception err)
            {
                return err.Message;
            }

        }
        public bool IsXMLString(string text)
        {
            return xmlDecoder.IsXMLString(text);
        }
        public string FormatXml(string xml)
        {
            return xmlDecoder.Beautify(xml);
        }
        private void XmlDecoder_LoadFinish()
        {
            if (this.LoadXmlFinish != null)
                LoadXmlFinish(xmlDecoder.XmlString);

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

        public string GetXml(IBasicData basicData)
        {
            return xmlDecoder.GetXml(basicData);
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
                catch(Exception e)
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
            xmlDecoder.XmlString = sb.ToString();
            if (LoadStarting != null)
                LoadStarting(true, "Deserializing xml");
            sb = null;
            fs.Close();
            fs.Dispose();
            if(workspaceUnzip!=null)
                workspaceUnzip.Dispose();
            try
            {
                xmlDecoder.Process(file.FullName);
            }
            catch (Exception erro)
            {
                DispactchNewMessage(erro.Message, MsgType.Erro);
                return;
            }

            ListPrimaryItens = xmlDecoder.FirstItens;
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
            if(InCorel)
                ResourcesExtractor.GetGuids();
            //listIsLoaded = true;
            //TestGetResourceRCDATA();

        }
        private void SaveIcons()
        {
            IconsFolder = Path.Combine(workerFolder, string.Format("Icons_CDR{0}", CorelApp.VersionMajor));
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
            t = new Thread(new ThreadStart(saveIcons));
            t.IsBackground = true;
            t.Start();
        }
        private bool listIsLoaded = false;
        private bool guidIsLoaded = false;
        private int c = 1;
        //public void TestGetResourceRCDATA(Dictionary<UInt16,List<string>> guids)
        //{
        //    DispactchNewMessage("Getting icons Guids", MsgType.Console);
        //    Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        //    //t = new Thread(new ThreadStart(new Action(() =>
        //    //{
        //    //return;
        //    //if (!listIsLoaded || !guidIsLoaded)
        //    //    return;
        //    // List<string> guids = ResourcesExtractor.GetGuids();
        //    //List<IBasicData> datas = new List<IBasicData>();
        //    foreach (var item in guids)
        //    {
        //        List<string> guidL = item.Value;
        //        for (int i = 0; i < guidL.Count; i++)
        //        {

        //            IBasicData refBasicData = searchEngine.SearchAllAttributesValueNoEvent(ListPrimaryItens, guidL[i]);
        //            //datas.Add(refBasicData);
        //            for (int r = 0; r < refBasicData.Childrens.Count; r++)
        //            {
        //                dispatcher.Invoke(new Action(() =>
        //                {
        //                    refBasicData.Childrens[r].Icon = string.Format("{0}\\{1}.png",IconsFolder, item.Key);
        //                }));

        //            }
        //        }
        //    }
        //    DispactchNewMessage("Icons crawleds", MsgType.Console);
        //    //})));
        //    //t.IsBackground = true;
        //    //t.Start();
        //    // Debug.WriteLine(datas.Count);
        //}
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
                int line = xmlDecoder.GetLineNumber(basicData);
                if(line == -1)
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
            catch(Exception erro)
            {
                DispactchNewMessage(erro.Message, MsgType.Erro);
            }
            
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
