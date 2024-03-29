using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands;
using br.com.Bonus630DevToolsBar.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    class XMLTagWindowViewModel : ViewModelDataBase
    {
        private bool incorel = false;
        private Dispatcher dispatcher;
        public CorelAutomation CorelCmd { get; set; }
        public event Action<IBasicData> XmlDecode;

        public XMLTagWindowViewModel(Core core) : base(core)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            //autoCompleteInputCommand();
            this.mainList = new ObservableCollection<IBasicData>();
            this.refList = new ObservableCollection<IBasicData>();
            this.searchList = new ObservableCollection<IBasicData>();
            core.LoadListsFinish += Core_LoadListsFinish;
            core.SearchResultEvent += Core_SearchResultEvent;
            core.InCorelChanged += (b) => InCorel = b;
            initializeCommands();
        }

        private void Core_SearchResultEvent(IBasicData obj)
        {
            searchList.Add(obj);
            OnPropertyChanged("SearchList");
        }
        private void Core_LoadListsFinish()
        {
            //tenho que fazer o merge aqui?
            //Preciso verificar as tags que já estão carregadas e entao adicionar somente as nao carregadas nas posiçoes corretas
            //mainList.Add(core.ListPrimaryItens);
            dispatcher.Invoke(new Action(() =>
            {
                mainList.Add(Core.ListPrimaryItens);
                OnPropertyChanged("MainList");
            }));
        }
        public bool InCorel
        {
            get { return incorel; }
            set
            {
                incorel = value;
                OnPropertyChanged();
            }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        private bool consoleExpanded = false;

        public bool ConsoleExpanded
        {
            get { return consoleExpanded; }
            set
            {
                consoleExpanded = value;
                OnPropertyChanged();
            }
        }

        System.Windows.Forms.AutoCompleteStringCollection AutoCompleteSource { get; set; }

        public SimpleCommand ConfigCommand { get { return new SimpleCommand(config); } }
        public SimpleCommand DrawUICommand { get { return new SimpleCommand(drawUI); } }
        public SimpleCommand WorkSpaceCommand { get { return new SimpleCommand(workSpace); } }
        public SimpleCommand ExpandConsoleCommand { get { return new SimpleCommand(expandConsole); } }
        public SimpleCommand ActiveGuidCommand { get { return new SimpleCommand(activeGuid); } }
        public SimpleCommand HighLightCommand { get; protected set; }
        public BaseDataCommand LayoutCommand { get; protected set; }

        public AttributeCommand FindRef { get; protected set; }
        public BaseDataCommand CopyGuidCommand { get; protected set; }
        public BaseDataCommand XmlCommand { get; protected set; }
        public BaseDataCommand OpenLineCommand { get; protected set; }
        public BaseDataCommand GetCaptionCommand { get; protected set; }
        public BaseDataCommand ShowCommandBarCommand { get; protected set; }
        public BaseDataCommand HideCommandBarCommand { get; protected set; }
        public BaseDataCommand CommandBarModeCommand { get; protected set; }
        public BaseDataCommand ShowThisCommand { get; protected set; }
        public BaseDataCommand HideThisCommand { get; protected set; }
        public BaseDataCommand ShowDialogCommand { get; protected set; }
        public BaseDataCommand HideDialogCommand { get; protected set; }
        public BaseDataCommand ShowDockerCommand { get; protected set; }
        public BaseDataCommand HideDockerCommand { get; protected set; }
        public BaseDataCommand InvokeItemCommand { get; protected set; }
        public BaseDataCommand GetDockersCaptionCommand { get; protected set; }
        public BaseDataCommand GetDockersGuidCommand { get; protected set; }
        public BaseDataCommand GetIUnknownTypesCommand { get; protected set; }
        public BaseDataCommand RemoveMeCommand { get; protected set; }
        public RoutedCommand<Tuple<SolidColorBrush, IBasicData>> MarkCommand { get; protected set; }
        public BaseDataCommand UnMarkCommand { get; protected set; }



        private void initializeCommands()
        {
            CopyGuidCommand = new BaseDataCommand(CopyGuidExec, HasGuid);
            FindRef = new AttributeCommand(FindRefExec, IsAttributeRef);
            XmlCommand = new BaseDataCommand(XmlExec, IsTrue);
            OpenLineCommand = new BaseDataCommand(OpenEditor, IsTrue);
            GetCaptionCommand = new BaseDataCommand(GetCaptionTextExec, HasGuid);
            ShowCommandBarCommand = new BaseDataCommand(ShowCommandBarExec, IsCommandBarData);
            HideCommandBarCommand = new BaseDataCommand(HideCommandBarExec, IsCommandBarData);
            CommandBarModeCommand = new BaseDataCommand(CommandBarModeExec, IsCommandBarData);
            ShowThisCommand = new BaseDataCommand(ShowItemExec, IsItemData);
            HideThisCommand = new BaseDataCommand(CopyGuidExec, IsItemData);
            ShowDialogCommand = new BaseDataCommand(ShowDialogExec, IsDialogData);
            HideDialogCommand = new BaseDataCommand(HideDialogExec, IsDialogData);
            ShowDockerCommand = new BaseDataCommand(ShowDockerExec, IsDockerData);
            HideDockerCommand = new BaseDataCommand(HideDockerExec, IsDockerData);
            InvokeItemCommand = new BaseDataCommand(ItemInvokeExec, IsItemData);
            GetIUnknownTypesCommand = new BaseDataCommand(IUnknownTypeInvokeExec, IsItemData);
            GetDockersCaptionCommand = new BaseDataCommand(GetDockersCaptionExec, IsDockers);
            GetDockersGuidCommand = new BaseDataCommand(GetDockersGuidExec, IsDockers);
            RemoveMeCommand = new BaseDataCommand(RemoveMeExec, IsSearchData);
            MarkCommand = new RoutedCommand<Tuple<SolidColorBrush,IBasicData>>(MarkColor, IsUnMarked);
            UnMarkCommand = new BaseDataCommand(UnMark, IsMarked);
            HighLightCommand = new SimpleCommand(showHighLightItem);
            LayoutCommand = new BaseDataCommand(layoutAdorms, IsComplexLayout);
        }

        private void MarkColor(Tuple<SolidColorBrush,IBasicData> data)
        {
            data.Item2.Marked = true;
            data.Item2.MarkColor = data.Item1;
        }
        private bool IsUnMarked(object data)
        {
            return true;
            //verificar
            Console.WriteLine(data.GetType());
            return true;
            //return !(data as IBasicData).Marked;
        } 
        private bool IsMarked(object data)
        {
            return IsUnMarked(data);
        }
        private bool IsMarked(IBasicData data)
        {
            return data.Marked;
        }
        private void UnMark(IBasicData data)
        {
            data.Marked = false;
            data.MarkColor = Brushes.Transparent;
        }
        private bool IsDockers(IBasicData basicData)
        {
            if (basicData.TagName == "dockers")
                return true;
            return false;
        }
        private bool IsTrue(IBasicData basicData)
        {
            return true;
        }
        private bool IsItemData(IBasicData basicData)
        {
            if (!incorel)
                return false;
            return IsTypeOf(basicData, typeof(ItemData));
        }
        private bool IsDockerData(IBasicData basicData)
        {
            if (!incorel)
                return false;
            return IsTypeOf(basicData, typeof(DockerData));
        }
        private bool IsDialogData(IBasicData basicData)
        {
            if (!incorel)
                return false;
            return IsTypeOf(basicData, typeof(DialogData));
        }
        private bool IsSearchData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(SearchData));
        }
        private bool IsOtherData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(OtherData));
        }
        private bool IsCommandBarData(IBasicData basicData)
        {
            if (!incorel)
                return false;
            return IsTypeOf(basicData, typeof(CommandBarData));
        }
        private bool IsTypeOf(IBasicData basicData, Type type)
        {
            if (basicData.GetType() == type)
                return true;
            return false;
        }
        private bool HasGuid(IBasicData basicData)
        {
            if (!string.IsNullOrEmpty(basicData.Guid) && Core.InCorel)
                return true;
            return false;
        }
        private void FindRefExec(DataClass.Attribute att)
        {
            Core.FindByGuid(Core.ListPrimaryItens.Childrens, att.Value);
        }
        private bool IsAttributeRef(DataClass.Attribute att)
        {
            //Console.WriteLine("Attribute {0}",att);
           // return false;
            //verificar
            if (att.Name == "guid" || att.Name == "guidRef")
                return false;
            return att.IsGuid;
        }

        private void ShowDialogExec(IBasicData basicData)
        {

            CorelCmd.ShowDialog(basicData.Guid);
        }
        private void HideDialogExec(IBasicData basicData)
        {
#if !X7
            CorelCmd.HideDialog(basicData.Guid);
#endif
        }
        private void ShowDockerExec(IBasicData basicData)
        {
            CorelCmd.ShowDocker(basicData.Guid);
        }
        private void HideDockerExec(IBasicData basicData)
        {
            CorelCmd.HideDocker(basicData.Guid);
        }
        private void ShowCommandBarExec(IBasicData basicData)
        {
            CorelCmd.ShowHideCommandBar(basicData, true);

        }
        private void HideCommandBarExec(IBasicData basicData)
        {
            CorelCmd.ShowHideCommandBar(basicData, false);
        }
        private void CommandBarModeExec(IBasicData basicData)
        {
            CorelCmd.CommandBarMode(basicData, false);
        }
        private void ShowItemExec(IBasicData basicData)
        {
            CorelCmd.ShowBar(basicData.Guid);
        }
        private void GetCaptionTextExec(IBasicData basicData)
        {
            Core.DispactchNewMessage(CorelCmd.GetCaption(basicData.Guid), MsgType.Console);
        }
        private void ItemInvokeExec(IBasicData basicData)
        {
            CorelCmd.InvokeItem(basicData);
        }
        private void IUnknownTypeInvokeExec(IBasicData basicData)
        {
            List<Type> types = Core.GetIUnknown(basicData);

            string r = "";
            for (int i = 0; i < types.Count; i++)
            {
                if (string.IsNullOrEmpty(r))
                    r += types[i].Name;
                else
                    r += "," + types[i].Name;
            }
            r = string.Format("Types:{0}", r);
            Core.DispactchNewMessage(r, MsgType.Result);

        }
        readonly List<string> guidDockersNotOpen = new List<string>()
        {
            "1b65813d-c4b8-46f7-b596-d5b17d13fda2",
            "e328ca81-0c4e-4109-9208-cc4dfcc018b0",
            "0799f357-efd9-45b0-acf5-1e2915bcdc1a"
        };
        
        private void GetDockersCaptionExec(IBasicData basicData)
        {

            Core.DispactchNewMessage("Corel will open all Dockers, please wait...", MsgType.Console);

            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                IBasicData temp = basicData.Childrens[i];
                if (temp.GetType() == typeof(DockerData) && !guidDockersNotOpen.Contains(temp.Guid))
                {
                    try
                    {
                        Clipboard.SetText(temp.Guid);
                        Core.DispactchNewMessage("Dockers Caption is copied!", MsgType.Result);
                    }
                    catch (Exception e)
                    {
                        Core.DispactchNewMessage("Error when copying", MsgType.Erro);
                    }
                    if (string.IsNullOrEmpty(temp.Caption))
                    {
                        temp.Caption = Core.CorelApp.FrameWork.Automation.GetCaptionText(temp.Guid);
                    }
                    if (string.IsNullOrEmpty(temp.Caption))
                    {
                        try
                        {

                            //System.Windows.MessageBox.Show("Test");
                            Core.CorelApp.FrameWork.ShowDocker(temp.Guid);
                            temp.Caption = Core.CorelApp.FrameWork.Automation.GetCaptionText(temp.Guid);
                            Core.CorelApp.FrameWork.HideDocker(temp.Guid);
                        }
                        catch { }
                    }
                }
            }
            Core.DispactchNewMessage("All Dockers crawleds", MsgType.Console);


        }
        private void GetDockersGuidExec(IBasicData basicData)
        {
            string guid = "";
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                IBasicData temp = basicData.Childrens[i];
                if (temp.GetType() == typeof(DockerData))
                {
                    guid += "\"" + temp.Guid + "\",";
                }
            }
            try
            {
                Clipboard.SetText(guid);
                Core.DispactchNewMessage("Dockers Guids copied!", MsgType.Result);
            }
            catch (Exception e)
            {
                Core.DispactchNewMessage("Error when copying", MsgType.Erro);
            }
        }

        private void CopyGuidExec(IBasicData basicData)
        {
            try
            {
                System.Windows.Clipboard.SetText(basicData.Guid);
                Core.DispactchNewMessage(string.Format("Copied:>    {0}", basicData.Guid), MsgType.Result);
            }
            catch (Exception e)
            {
                Core.DispactchNewMessage("Error when copying", MsgType.Erro);
            }
        }
        private void RemoveMeExec(IBasicData basicData)
        {
            searchList.Remove(basicData);
            OnPropertyChanged("SearchList");
        }
        private void XmlExec(IBasicData basicData)
        {
            //Precisa fixar
            if (XmlDecode != null)
                XmlDecode(basicData);
        }
        private void OpenEditor(IBasicData basicData)
        {
            Core.RunEditor(basicData);
        }


        private ObservableCollection<IBasicData> mainList;
        public ObservableCollection<IBasicData> MainList
        {
            get { return mainList; }
            protected set
            {
                mainList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<IBasicData> refList;
        public ObservableCollection<IBasicData> RefList
        {
            get { return refList; }
            protected set
            {
                refList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<IBasicData> searchList;
        public ObservableCollection<IBasicData> SearchList
        {
            get { return searchList; }
            protected set
            {
                searchList = value;
                OnPropertyChanged();
            }
        }
        protected override void Update(IBasicData basicData)
        {
            CurrentBasicData = basicData;
        }
        protected override void UpdateNoAttached(IBasicData basicData)
        {
            Update(basicData);
        }
        //private void autoCompleteInputCommand()
        //{
        //    AutoCompleteSource = new System.Windows.Forms.AutoCompleteStringCollection();
        //    MethodInfo[] m = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance);
        //    for (int i = 0; i < m.Length; i++)
        //    {
        //        AutoCompleteSource.Add(m[i].Name);
        //    }

        //}



        private void showHighLightItem()
        {
            Core.HighLightItemHelper.ShowHighLightItem(Core.Route);
        }
        private void layoutAdorms(IBasicData basicData)
        {
            Core.HighLightItemHelper.InitializeLayoutMode(Core.CurrentBasicData);
        }
        private bool IsComplexLayout(IBasicData basicData)
        {
            return Core.InCorel;
           // return basicData is DockerData || basicData is CommandBarData;

        }
        private void activeGuid()
        {
           Core.CorelAutomation.GetActiveGuidTool();
        }

        private void config()
        {
            Views.Config config = new Views.Config();
            config.ShowDialog();
        }
        private void drawUI()
        {
            Views.Config config = new Views.Config();
            config.ShowDialog();
        }
        private void workSpace()
        {
            Views.Config config = new Views.Config();
            config.ShowDialog();
        }
        private void expandConsole()
        {
            this.ConsoleExpanded = !this.ConsoleExpanded;
        }
    }

}
