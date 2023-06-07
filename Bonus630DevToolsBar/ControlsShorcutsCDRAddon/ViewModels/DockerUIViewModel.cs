using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using br.com.Bonus630DevToolsBar.DrawUIExplorer;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.Models;
using Corel.Interop.VGCore;

namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.ViewModels
{
    public class DockerUIViewModel
    {
        private Application corelApp;
        public ObservableCollection<Shortcut> Shortcuts { get; set; }
        private Core core;
        private readonly string keyName = "keySequence";
        private readonly string itemRef = "itemRef";
        private readonly string altAttribute = "alt";
        private Dispatcher dispatcher;
        public RunCommand RunCommand { get; set; }

        public DockerUIViewModel(Application corelApp)
        {
            Shortcuts = new ObservableCollection<Shortcut>();
            RunCommand = new RunCommand(InvokeItem);
            this.corelApp = corelApp;
            this.dispatcher = Dispatcher.CurrentDispatcher;
            core = new Core();
            core.StartCore(Path.Combine(this.corelApp.Path,"UIConfig\\DrawUI.xml"),this.corelApp);
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
                if(obj.Childrens[i].TagName == keyName)
                {
                    s = new Shortcut();
                    
                    if (obj.Childrens[i].Childrens.Count > 0)
                    {
                        Boolean.TryParse(obj.Childrens[i].Childrens[0].GetAttribute(altAttribute), out alt);
                        s.Alt = alt;
                        if(obj.Childrens[i].Childrens[0].Childrens.Count > 0)
                            s.Key = obj.Childrens[i].Childrens[0].Childrens[0].Text.Replace("VK","");
                    }
                    s.Shift = shift;
                    s.Control = control;
                    s.Guid = obj.Childrens[i].GetAttribute(itemRef);
                    s.Name = core.CorelAutomation.GetCaption(s.Guid);
                    dispatcher.Invoke(() =>
                    {
                        this.Shortcuts.Add(s);
                    });
                }
            }
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
        }

        public void InvokeItem(Shortcut shortcut)
        {
            try
            {
                this.corelApp.FrameWork.Automation.InvokeItem(shortcut.Guid);
            }
            catch { }
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
            contentExec?.Invoke((parameter as Shortcut));
        }
    }
}
