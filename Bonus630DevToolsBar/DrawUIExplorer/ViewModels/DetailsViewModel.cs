
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    public class DetailsViewModel : ViewModelDataBase
    {

        List<IBasicData> temp;
        CorelAutomation corelAutomation;
        private Corel.Interop.VGCore.Application corelApp;
        public DetailsViewModel(Core core) : base(core)
        {
            corelAutomation = new CorelAutomation(core.CorelApp, core);
            this.core = core;
            corelApp = core.CorelApp;
            RunBindCommand = new ViewModels.Commands.AttributeCommand(attributeContentExec, attributeContentCanExec);
            RunBindWithParamCommand = new ViewModels.Commands.AttributeCommand(attributeContentExecWithParam, attributeContentCanExec);
            RunMacroCommand = new ViewModels.Commands.AttributeCommand(attributeMacro, attributeCanMacro);
            SearchGuidCommand = new ViewModels.Commands.AttributeCommand(attributeSearchGuid, attributeCanSearchGuid);
            SearchItemCommand = new ViewModels.Commands.AttributeCommand(attributeSearchItem, attributeCanSearchItem);
            CopyCommand = new ViewModels.Commands.AttributeCommand(attributeCopy, attributeCanCopy);
            CopyIconCommand = new ViewModels.Commands.RoutedCommand<string>(copyIcon);
            ImportInCorelCommand = new ViewModels.Commands.RoutedCommand<string>(importInCorel);
            OpenIconsFolderCommand = new ViewModels.Commands.RoutedCommand<string>(openIconsFolder);

        }



        public Commands.AttributeCommand RunBindCommand { get; set; }
        public Commands.AttributeCommand RunBindWithParamCommand { get; set; }
        public Commands.AttributeCommand RunMacroCommand { get; set; }
        public Commands.AttributeCommand SearchGuidCommand { get; set; }
        public Commands.AttributeCommand SearchItemCommand { get; set; }
        public Commands.AttributeCommand CopyCommand { get; set; }
        public Commands.RoutedCommand<string> CopyIconCommand { get; set; }
        public Commands.RoutedCommand<string> ImportInCorelCommand { get; set; }
        public Commands.RoutedCommand<string> OpenIconsFolderCommand { get; set; }
        private string caption;

        public string Caption
        {
            get { return caption; }
            set { caption = value; OnPropertyChanged(); }
        }
        private string route;

        public string Route
        {
            get { return route; }
            set { route = value; OnPropertyChanged(); }
        }
        private string captionLocalization;

        public string CaptionLocalization
        {
            get { return captionLocalization; }
            set { captionLocalization = value; OnPropertyChanged(); }
        }
        //private string guid;

        //public string Guid
        //{
        //    get { return guid; }
        //    set { guid = value; NotifyPropertyChanged(); }
        //}
        //private string guidRef;

        //public string GuidRef
        //{
        //    get { return guidRef; }
        //    set { guidRef = value; NotifyPropertyChanged(); }
        //}
        //private int treeLevel = 0;

        //public int TreeLevel
        //{
        //    get { return treeLevel; }
        //    set { treeLevel = value; NotifyPropertyChanged(); }
        //}
        //private List<Attribute> attributes;

        //public List<Attribute> Attributes
        //{
        //    get { return attributes; }
        //    set { attributes = value; NotifyPropertyChanged(); }
        //}

        private int index = 0;

        public int Index
        {
            get { return index; }
            set { index = value; OnPropertyChanged(); }
        }

        private int indexRef = 0;
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged(); }
        }

        ////Teste getresource
        //private string icon;

        //public string Icon
        //{
        //    get { return icon; }
        //    set { icon = value; NotifyPropertyChanged(); }
        //}
        public int IndexRef
        {
            get { return indexRef; }
            set { indexRef = value; OnPropertyChanged(); }
        }
        protected override void Update(IBasicData basicData)
        {
          
            CurrentBasicData = basicData;
            //Guid = basicData.Guid;
            //GuidRef = basicData.GuidRef;
            //TreeLevel = basicData.TreeLevel;
           // Attributes = basicData.Attributes;
            TryGetAnyCaption();
            //Quando comentar algo que não está certo, colocar o pq!!!!
            //Index = basicData.XmlChildreID;
            //IndexRef = basicData.XmlChildreParentID;
            //Text = basicData.Text;
            temp = core.Route;
            string route = "";
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].XmlChildreID > 0 && !temp[i].IAmUniqueTag())
                {
                    if (!string.IsNullOrEmpty(temp[i].Guid))
                        route += string.Format("/{0}[@guid='{1}']", temp[i].TagName, temp[i].Guid);
                    else
                        route += string.Format("/{0}[{1}]", temp[i].TagName, temp[i].XmlChildreID);
                }
                else
                    route += string.Format("/{0}", temp[i].TagName);
            }
            Route = route;
        }

        private void attributeContentExec(Attribute attribute)
        {
            bool invoke = false;
            if (attribute.Name == "onInvoke")
                invoke = true;
            corelAutomation.RunBindDataSource(attribute.Value, invoke);
        } 
        private void attributeContentExecWithParam(Attribute attribute)
        {
            bool invoke = false;
            if (attribute.Name == "onInvoke")
                invoke = true;
            corelAutomation.RunBindWithParamDataSource(attribute.Value, invoke);
        }
        private void attributeMacro(Attribute attribute)
        {
            corelAutomation.RunMacro(attribute.Value);
        }
        private void attributeSearchGuid(Attribute attribute)
        {
            core.FindByGuid(core.ListPrimaryItens.Childrens, attribute.Value);
        }
        private void attributeSearchItem(Attribute attribute)
        {
            core.FindItemContainsGuidRef(core.ListPrimaryItens, attribute.Value);
        }
        private void attributeCopy(Attribute attribute)
        {
            Clipboard.SetText(attribute.ToString());
        }
        private bool attributeContentCanExec(Attribute attribute)
        {
            if (attribute.Value.Contains("*Bind"))
                return true;
            return false;
        }
        private bool attributeCanMacro(Attribute attribute)
        {
            if (attribute.Name.Contains("dynamicCommand"))
                return true;
            return false;

        }
        private bool attributeCanSearchGuid(Attribute attribute)
        {
            return attribute.IsGuid;
        }
        private bool attributeCanSearchItem(Attribute attribute)
        {
            if (attribute.Name == "guid")
                return attribute.IsGuid;
            return false;
        }
        private bool attributeCanCopy(Attribute attribute)
        {
            return true;
        }
        private void copyIcon(string obj)
        {
            BitmapDecoder bd = PngBitmapDecoder.Create(new System.Uri(obj), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            // byte[] buffer = File.ReadAllBytes(obj);
            // Clipboard.SetImage()
        }
        private void importInCorel(string obj)
        {
            if (core.InCorel)
                if (core.CorelApp.ActiveDocument != null && core.CorelApp.ActiveLayer != null)
                    core.CorelApp.ActiveLayer.Import(obj);
        }
        private void openIconsFolder(string obj)
        {
            System.Diagnostics.Process.Start(core.IconsFolder);
        }
        private bool canImportInCorel()
        {
            if (core.InCorel)
                if (core.CorelApp.ActiveDocument != null && core.CorelApp.ActiveLayer != null)
                    return true;
            return false;
        }
        private void TryGetAnyCaption()
        {

            if (!string.IsNullOrEmpty(CurrentBasicData.Caption))
            {
                Caption = CurrentBasicData.Caption;
                CaptionLocalization = "Caption";
                if (!string.IsNullOrEmpty(caption))
                    return;
            }
            if (corelApp != null)
            {
                Caption = corelAutomation.GetItemCaption(CurrentBasicData.Guid);
                CaptionLocalization = "Automation GetCaption";
                if (!string.IsNullOrEmpty(caption))
                    return;

            }
            string[] searchs = new string[] { "captionGuid", "guidRef" };



            for (int i = 0; i < searchs.Length; i++)
            {


                if (this.CurrentBasicData.ContainsAttribute(searchs[i]))
                {
                    string t = "";
                    //caption = core.SearchItemFromGuidRef(core.ListPrimaryItens, basicData.GetAttribute(searchs[i])).Caption;
                    Caption = tryGetAnyCaption(core.SearchItemFromGuidRef(core.ListPrimaryItens, CurrentBasicData.GetAttribute(searchs[i])), out t);
                    CaptionLocalization = searchs[i];

                }
                if (!string.IsNullOrEmpty(caption))
                    return;
            }
            string search = "nonLocalizableName";
            if (this.CurrentBasicData.ContainsAttribute(search))
            {

                Caption = CurrentBasicData.GetAttribute(search);
                CaptionLocalization = search;

                if (!string.IsNullOrEmpty(caption))
                    return;
            }
        }
        private string tryGetAnyCaption(IBasicData basicData, out string method)
        {
            string caption = "";
            method = "";
            if (!string.IsNullOrEmpty(this.CurrentBasicData.Caption))
            {
                caption = basicData.Caption;
                CaptionLocalization = "Caption";
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            if (corelApp != null)
            {
                caption = corelAutomation.GetItemCaption(basicData.Guid);
                method = "Automation GetCaption";
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            string[] searchs = new string[] { "captionGuid", "guidRef" };
            for (int i = 0; i < searchs.Length; i++)
            {
                if (this.CurrentBasicData.ContainsAttribute(searchs[i]))
                {
                    string guid = basicData.GetAttribute(searchs[i]);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        caption = tryGetAnyCaption(core.SearchItemFromGuidRef(core.ListPrimaryItens, guid), out method);
                        method = searchs[i];
                    }
                }
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            string search = "nonLocalizableName";
            if (this.CurrentBasicData.ContainsAttribute(search))
            {
                caption = basicData.GetAttribute(search);
                method = search;
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            return caption;
        }
        private void GetRoute(IBasicData basic)
        {
            if (basic.Parent != null)
            {
                temp.Add(basic.Parent);
                GetRoute(basic.Parent);
            }
        }

        //private void btn_showTreeView_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        showHighLightItem();
        //    }
        //    catch (System.Exception erro)
        //    {
        //        core.DispactchNewMessage(erro.Message,MsgType.Console);
        //    }

        //}
        private void showHighLight()
        {

        }
        //private void showHighLightItem()
        //{
        //    try
        //    {
        //        IBasicData itemData = this.basicData;
        //        IBasicData parentItemData = parentItemData = basicData.Parent;
        //        IBasicData specialData = null;

        //        if (basicData.IsSpecialType)
        //        {
        //            specialData = basicData;
        //        }
        //        else
        //        {
        //            while ((specialData.GetType() != typeof(CommandBarData) || specialData.GetType() == typeof(DockerData)
        //           || specialData.GetType() == typeof(DialogData)) && specialData.Parent != null)
        //            {
        //                specialData = specialData.Parent;
        //            }
        //        }
        //        //string guidItem = temp[temp.Count - 1].Guid;
        //        //DataClass.IBasicData basicData = core.SearchItemContainsGuidRef(core.ListPrimaryItens, guidItem);
        //        while (string.IsNullOrEmpty(parentItemData.Guid) && parentItemData.Parent != null)
        //        {
        //            parentItemData = parentItemData.Parent;
        //        }
        //        if (specialData.GetType() == typeof(CommandBarData))
        //        {
        //            corelApp.FrameWork.Automation.ShowBar(specialData.Guid, true);
        //            specialData = new CommandBarData();
        //            specialData.Guid = specialData.Guid;
        //        }
        //        if (specialData.GetType() == typeof(DockerData))
        //        {
        //            if (!corelApp.FrameWork.IsDockerVisible(specialData.Guid))
        //            {
        //                corelApp.FrameWork.ShowDocker(specialData.Guid);
        //                specialData = new DockerData();
        //                specialData.Guid = specialData.Guid;
        //            }
        //        }
        //        if (specialData.GetType() == typeof(DialogData))
        //        {

        //            corelApp.FrameWork.ShowDialog(specialData.Guid)
        //            specialData = new DialogData();
        //            specialData.Guid = specialData.Guid;
        //        }
        //        LoadHighLightForm(basicData, parentItemData, specialData);
        //    }
        //    catch (System.Exception erro)
        //    {
        //        core.DispactchNewMessage(erro.Message);
        //    }
        //}








        //private void btn_getActiveGuid_Click(object sender, RoutedEventArgs e)
        //{
        //    copyItemCaptionAndGuid();
        //}
        public void copyItemCaptionAndGuid()
        {

            Clipboard.SetText(string.Format("{0} - {1}", this.caption, this.CurrentBasicData.Guid));
        }
        private void selectItem()
        {
            for (int i = 0; i < temp.Count; i++)
            {
                if (i != temp.Count - 1)
                    temp[i].SetSelected(false, true, true);
                else
                    temp[i].SetSelected(true, false, true);
            }
        }


    }
}
