﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using br.com.Bonus630DevToolsBar.DrawUIExplorer;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.Models;
using Corel.Interop.VGCore;
using System.Linq;
using System.Net.Configuration;
using System.Diagnostics.Eventing.Reader;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using System.Xml;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;


namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.ViewModels
{
    public class DockerUIViewModel : ViewModelBase, IDisposable
    {
        private Application corelApp;
        private bool firstUse = false;
        public ObservableCollection<Shortcut> Shortcuts { get; set; }
        private ObservableCollection<Shortcut> AllItems = new ObservableCollection<Shortcut>();
        private Core core;
        private readonly string keyName = "keySequence";
        private readonly string itemRef = "itemRef";
        private readonly string altAttribute = "alt";
        private readonly string ctrlAttribute = "ctrl";
        private readonly string shiftAttribute = "shift";
        private Dispatcher dispatcher;
        public RunCommand RunCommand { get; set; }
        public RunCommand CopyGuidCommand { get; set; }
        public ICommand SortAscendingCommand { get; set; }
        public ICommand SortDescendingCommand { get; set; }


        private System.Windows.Visibility loadingVisible = System.Windows.Visibility.Visible;

        public System.Windows.Visibility LoadingVisible
        {
            get { return loadingVisible; }
            set { loadingVisible = value; OnPropertyChanged(); }
        }


        public DockerUIViewModel(Application corelApp)
        {
            var dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(ControlUI.DataSourceName);
            firstUse = (bool)dsp.GetProperty("ShortcutDockerFirstUse");

            Shortcuts = new ObservableCollection<Shortcut>();
            RunCommand = new RunCommand(InvokeItem);
            CopyGuidCommand = new RunCommand(CopyGuid);
            SortAscendingCommand = new DrawUIExplorer.ViewModels.Commands.SimpleCommand(SortAscending);
            SortDescendingCommand = new DrawUIExplorer.ViewModels.Commands.SimpleCommand(SortDescending);
            this.corelApp = corelApp;
            this.dispatcher = Dispatcher.CurrentDispatcher;
            core = new Core();

            using (FileStream fs = new FileStream(string.Format("{0}{1}.cdws", this.corelApp.UserWorkspacePath, this.corelApp.ActiveWorkspace.Name),
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                WorkspaceUnzip workspaceUnzip = new WorkspaceUnzip(fs);
                StreamReader sr = workspaceUnzip.XmlStreamReader;
                try
                {
                    File.WriteAllText(string.Format("{0}\\ExtractWorkspace{1}.xml", core.WorkerFolder,this.corelApp.VersionMajor), sr.ReadToEnd());
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            core.PartialStart(new List<string>() { Path.Combine(this.corelApp.Path, "UIConfig\\DrawUI.xml"),string.Format("{0}\\ExtractWorkspace{1}.xml", core.WorkerFolder,this.corelApp.VersionMajor)
           },
                this.corelApp, new List<string>() { "shortcutKeyTables" }, !firstUse);

            if (firstUse)
            {
                dsp.SetProperty("ShortcutDockerFirstUse", false);
            }


            core.LoadListsFinish += Core_LoadListsFinish;
            core.SearchResultEvent += Core_SearchResultEvent;

        }

        private void Core_SearchResultEvent(DrawUIExplorer.DataClass.IBasicData obj)
        {
            Shortcut s;
            bool alt = false;
            bool shift = false;
            bool control = false;
            for (int i = 0; i < obj.Childrens.Count; i++)
            {
                if (obj.Childrens[i].TagName == keyName)
                {
                    s = new Shortcut();

                    if (obj.Childrens[i].Childrens.Count > 0)
                    {
                        Boolean.TryParse(obj.Childrens[i].Childrens[0].GetAttribute(altAttribute), out alt);
                        s.Alt = alt;
                        if (obj.Childrens[i].Childrens[0].Childrens.Count > 0)
                        {
                            s.Key = new string[obj.Childrens[i].Childrens[0].Childrens.Count];
                            for(int j = 0;j< s.Key.Length;j++)
                                s.Key[j] = obj.Childrens[i].Childrens[0].Childrens[j].Text.Replace("VK", "");
                        }
                        Boolean.TryParse(obj.Childrens[i].Childrens[0].GetAttribute(shiftAttribute), out shift);
                        s.Shift = shift;
                        Boolean.TryParse(obj.Childrens[i].Childrens[0].GetAttribute(ctrlAttribute), out control);
                        s.Control = control;
                    }
                    s.Guid = obj.Childrens[i].GetAttribute(itemRef);
                    s.Name = this.GetCaption(s.Guid, true);
                    if(string.IsNullOrEmpty(s.Name))
                    {
                        s.Name = GetCaptionFromWorkspace(s.Guid);
                    }
                    dispatcher.Invoke(() =>
                    {
                        if (!this.AllItems.Contains(s))
                        {
                            this.Shortcuts.Add(s);
                            this.AllItems.Add(s);
                        }
                    });
                }
            }
        }
        private string GetCaption(string guid, bool removeAmpersand = false)
        {
            try
            {
                if (removeAmpersand)
                    return this.corelApp.FrameWork.Automation.GetCaptionText(guid).Replace("&", "");
                return this.corelApp.FrameWork.Automation.GetCaptionText(guid);
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }
        private bool workspaceRequestItems = true;
        private string GetCaptionFromWorkspace(string itemGuid)
        {
            if (workspaceRequestItems)
            {
                string filePath = string.Format("{0}\\ExtractWorkspace{1}.xml", core.WorkerFolder, this.corelApp.VersionMajor);
                IBasicData basicData = core.XmlDecoder.GetItemDataByGuidFromXml(filePath, itemGuid);
                if (basicData == null)
                    return "";
                else
                return basicData.Caption;
                //XmlDocument xmlDocument = new XmlDocument();
                //string xmlString = File.ReadAllText(filePath);
                //try
                //{
                //    xmlDocument.LoadXml(xmlString);
                //}
                //catch (XmlException erro)
                //{
                //    throw erro;
                //}
                //core.XmlDecoder.LoadXmlNodes(core.ListPrimaryItens, xmlDocument.ChildNodes.Item(1).SelectSingleNode("items"));
            }
            return "";
        }
        private void Core_LoadListsFinish()
        {
            List<SearchAdvancedParamsViewModel> actions = new List<SearchAdvancedParamsViewModel>();
            SearchAdvancedParamsViewModel searchAdvancedParams = new SearchAdvancedParamsViewModel();
            searchAdvancedParams.SearchBasicData = core.ListPrimaryItens;
            searchAdvancedParams.Enable = true;
            searchAdvancedParams.IsUnique = false;
            searchAdvancedParams.SearchAction = core.SearchEngineGet.GetDataByTagName;
            searchAdvancedParams.SearchParam = keyName;
            actions.Add(searchAdvancedParams);
            core.SearchEngineGet.SearchAdvanced(actions);
            dispatcher.Invoke(() => { LoadingVisible = System.Windows.Visibility.Collapsed; });
        }

        public void InvokeItem(Shortcut shortcut)
        {
            try
            {
                this.corelApp.FrameWork.Automation.InvokeItem(shortcut.Guid);
            }
            catch { }
        }
        public void CopyGuid(Shortcut shortcut)
        {
            try
            {
                System.Windows.Clipboard.SetText(shortcut.Guid);
            }
            catch { }
        }
        private string searchTerm;
        public string SearchTerm
        {
            get { return searchTerm; }
            set
            {
                if (searchTerm != value)
                {
                    searchTerm = value;
                    OnPropertyChanged("SearchTerm");
                    UpdateFilteredItems();
                }
            }
        }
        private void UpdateFilteredItems()
        {
            if (string.IsNullOrEmpty(SearchTerm))
            {
                Shortcuts = new ObservableCollection<Shortcut>(AllItems);
            }
            else
            {
                if (SearchTerm.Length == 1)
                    Shortcuts = new ObservableCollection<Shortcut>(
                   AllItems.Where<Shortcut>(item => item.Name.StartsWith(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                   item.Key[0].StartsWith(SearchTerm, StringComparison.OrdinalIgnoreCase)));
                else
                    Shortcuts = new ObservableCollection<Shortcut>(
                        AllItems.Where<Shortcut>(item => item.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0
                        || item.Guid.IndexOf(SearchTerm) >= 0));
            }

            OnPropertyChanged("Shortcuts");
        }
        private void SortAscending()
        {
            Shortcuts = new ObservableCollection<Shortcut>(Shortcuts.OrderBy(item => item.Name));
            OnPropertyChanged("Shortcuts");
        }

        private void SortDescending()
        {
            Shortcuts = new ObservableCollection<Shortcut>(Shortcuts.OrderByDescending(item => item.Name));
            OnPropertyChanged("Shortcuts");
        }

        public void Dispose()
        {
            Shortcuts.Clear();
            AllItems.Clear();
            core.Dispose();
            Dispose();
        }
    }
    public class RunCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<Shortcut> contentExec;
        public RunCommand(Action<Shortcut> contentExec)
        {
            this.contentExec = contentExec;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            if (contentExec != null)
                contentExec.Invoke((parameter as Shortcut));
        }
    }
}
