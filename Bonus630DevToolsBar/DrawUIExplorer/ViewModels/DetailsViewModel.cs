﻿
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
            corelAutomation = new CorelAutomation(core);
            this.Core = core;
            this.Core.InCorelChanged += Core_InCorelChanged;
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

        private void Core_InCorelChanged(bool obj,int lastVersion)
        {
            corelAutomation.CorelApp = this.Core.CorelApp;
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
            temp = Core.Route;
            string route = "";
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].XmlChildrenID > 0 && !temp[i].IAmUniqueTag())
                {
                    if (!string.IsNullOrEmpty(temp[i].Guid))
                        route += string.Format("/{0}[@guid='{1}']", temp[i].TagName, temp[i].Guid);
                    else
                        route += string.Format("/{0}[{1}]", temp[i].TagName, temp[i].XmlChildrenID);
                }
                else
                    route += string.Format("/{0}", temp[i].TagName);
            }
            Route = route;
        }
        protected override void UpdateNoAttached(IBasicData basicData)
        {
            Update(basicData);
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
            Core.FindByGuid(Core.ListPrimaryItens.Childrens, attribute.Value);
        }
        private void attributeSearchItem(Attribute attribute)
        {
            Core.FindItemContainsGuidRef(Core.ListPrimaryItens, attribute.Value);
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
            if (Core.InCorel)
                if (Core.CorelApp.ActiveDocument != null && Core.CorelApp.ActiveLayer != null)
                    Core.CorelApp.ActiveLayer.Import(obj);
        }
        private void openIconsFolder(string obj)
        {
            System.Diagnostics.Process.Start(Core.IconsFolder);
        }
        private bool canImportInCorel()
        {
            if (Core.InCorel)
                if (Core.CorelApp.ActiveDocument != null && Core.CorelApp.ActiveLayer != null)
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
                    Caption = tryGetAnyCaption(Core.SearchItemFromGuidRef(Core.ListPrimaryItens, CurrentBasicData.GetAttribute(searchs[i])), out t);
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
                        caption = tryGetAnyCaption(Core.SearchItemFromGuidRef(Core.ListPrimaryItens, guid), out method);
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

        private void showHighLight()
        {

        }
     
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
