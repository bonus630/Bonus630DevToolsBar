using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using Corel.Interop.VGCore;
using win = System.Windows;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class HighLightItemHelper
    {
        private CorelAutomation automation;
        private Application corelApp;
        private OverlayForm overlayForm;
        private PrintScreenForm printScreenForm;
        public HighLightItemHelper(CorelAutomation automation,Application corelApp)
        {
            this.automation = automation;
            this.corelApp = corelApp;
        }
        public void ShowHighLightItem(List<IBasicData> temp)
        {
            //Thread th = new Thread(new ParameterizedThreadStart(showHighLightItem));
            //th.IsBackground = true;
            //th.Start(temp);
            showHighLightItem(temp);
        }
        private Thread RunInBackground(Action action)
        {
            Thread th = new Thread(new ThreadStart(action));
            th.IsBackground = true;
            th.Start();
            return th;
        }

        private void LoadHighLightForm(IBasicData itemData, IBasicData parentItemData, IBasicData specialData, win.DependencyObject dependencyObject)
        {
            WinAPI.SetFocus(br.com.Bonus630DevToolsBar.ControlUI.corelHandle);
            WinAPI.SetForegroundWindow(br.com.Bonus630DevToolsBar.ControlUI.corelHandle);

            Corel.Interop.VGCore.cdrWindowState state = corelApp.AppWindow.WindowState;

            corelApp.AppWindow.Activate();
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized || state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMinimized)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized;
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore;
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowNormal)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowNormal;

            System.Windows.Rect rect = automation.GetItemRect(parentItemData.Guid, itemData.Guid);


            if (rect.IsZero())
                return;
            Views.XMLTagWindow w = Core.FindParentControl<Views.XMLTagWindow>(dependencyObject) as Views.XMLTagWindow;
            w.Visibility = win.Visibility.Collapsed;
            OverlayForm form;
            if (corelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
                form = new OverlayForm(rect);
            else
            {
                System.Windows.Rect rect2 = new System.Windows.Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                form = new OverlayForm(rect, rect2);
            }

            form.Show();
            form.FormClosed += (s, e) =>
            {
                w.Visibility = win.Visibility.Visible;
                if (specialData != null)
                {
                    if (specialData.GetType() == typeof(CommandBarData))
                    {

                    }
                    if (specialData.GetType() == typeof(DockerData))
                    {
                        corelApp.FrameWork.HideDocker(specialData.Guid);
                    }
                    if (specialData.GetType() == typeof(DialogData))
                    {
#if !X7
                        corelApp.FrameWork.HideDialog(specialData.Guid);
#endif
                    }
                }
            };

        }
        private void showHighLightItem(object list)
        {
            List<IBasicData> temp = (List<IBasicData>)list;
            // string guidParent = "c2b44f69-6dec-444e-a37e-5dbf7ff43dae";
            //string guidItem = "fa65d0c1-879b-4ef5-9465-af09e00e91ab";
            try
            {
                IBasicData parentBasicData = temp[temp.Count - 1];
                string guidItem = parentBasicData.Guid;
                string guidParent = "";

                if (string.IsNullOrEmpty(guidItem) && temp[temp.Count - 1].TagName.Equals("item"))
                {
                    guidItem = temp[temp.Count - 1].GuidRef;
                }
                if (!parentBasicData.IsContainer)
                {
                    parentBasicData = automation.Core.SearchEngineGet.SearchItemContainsGuidRef(automation.Core.ListPrimaryItens, guidItem, false);
                    if (parentBasicData != null)
                    {
                        while ((string.IsNullOrEmpty(parentBasicData.Guid) && parentBasicData.Parent != null) || parentBasicData.GetType() == typeof(OtherData))
                        {
                            //neste momento temos a referencia do item correto
                            parentBasicData = parentBasicData.Parent;
                        }

                    }
                }
                //basicData = temp[temp.Count - 1];
                guidParent = parentBasicData.Guid;
                Action<IBasicData, bool> restoration = null;
                //core.FindItemContainsGuidRef(core.ListPrimaryItens, attribute.Value);
                if (parentBasicData.GetType() == typeof(CommandBarData))
                {
                    bool visible = true;
                    try
                    {

                        // string commandBarName = GetItemCaption(guidParent);
                        string commandBarName = automation.GetItemCaption(parentBasicData);
                        Debug.WriteLine("CommandBarName:{0}", commandBarName);
                        CommandBar commandBar = corelApp.FrameWork.CommandBars[commandBarName];
                        visible = commandBar.Visible;
                        bool buildIt = commandBar.BuiltIn;
                        //  ShowHideCommandBar(commandBarName, true);
                        automation.ShowHideCommandBar(parentBasicData, true);

                        restoration = new Action<IBasicData, bool>(automation.ShowHideCommandBar);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message, "showhilite");
                        automation.Core.DispactchNewMessage(e.Message, MsgType.Console);
                    }
                    ShowHighLightItem(guidItem, guidParent, restoration, parentBasicData, visible);
                    return;
                }
                if (parentBasicData.GetType() == typeof(DockerData))
                {
                    bool visible = corelApp.FrameWork.IsDockerVisible(guidParent);
                    corelApp.FrameWork.ShowDocker(guidParent);
                    restoration = new Action<IBasicData, bool>(automation.ShowHideDocker);
                    ShowHighLightItem(guidItem, guidParent, restoration, parentBasicData, visible);
                    return;
                }
                if (parentBasicData.GetType() == typeof(DialogData))
                {
#if !X7
                    corelApp.FrameWork.ShowDialog(guidParent);
                    
#endif
                }
                ShowHighLightItem(guidItem, guidParent);
            }

            catch (System.Exception erro)
            {
                automation.Core.DispactchNewMessage(erro.Message, MsgType.Console);
            }
        }
        //private void SetCommmandBarVisible(string guid,object visible)
        //{

        //}
        
        public void ShowHighLightItem(string itemGuid, string itemParentGuid, Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false, bool firstTime = true)
        {
            PrepareForm(itemGuid, itemParentGuid, false, restoration, restorationData, v, firstTime);
        }
        public void PrintScreenItem(string itemGuid, string itemParentGuid, Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false, bool firstTime = true)
        {
            PrepareForm(itemGuid, itemParentGuid, false, restoration, restorationData, v, firstTime);
        }
        private void PrepareCommandBar(IBasicData basicData, string guid)
        {

        }
        private void PrepareForm(string itemGuid, string itemParentGuid, bool printScreen, Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false, bool firstTime = true)
        {
            WinAPI.SetFocus(br.com.Bonus630DevToolsBar.ControlUI.corelHandle);
            WinAPI.SetForegroundWindow(br.com.Bonus630DevToolsBar.ControlUI.corelHandle);

            cdrWindowState state = corelApp.AppWindow.WindowState;


            if (state == cdrWindowState.cdrWindowMaximized || state == cdrWindowState.cdrWindowMinimized)
                corelApp.AppWindow.WindowState = cdrWindowState.cdrWindowMaximized;
            if (state == cdrWindowState.cdrWindowRestore)
                corelApp.AppWindow.WindowState = cdrWindowState.cdrWindowRestore;
            if (state == cdrWindowState.cdrWindowNormal)
                corelApp.AppWindow.WindowState = cdrWindowState.cdrWindowNormal;
            //corelApp.AppWindow.Activate();

            System.Windows.Rect rect = automation.GetItemRect(itemParentGuid, itemGuid);

            if (rect.IsZero())
            {
                restoration.Invoke(restorationData, false);
               // Thread.Sleep(100);
                if (restoration != null)
                {
                    restoration.Invoke(restorationData, true);
                    rect = automation.GetItemRect(itemParentGuid, itemGuid);
                }
                //    return;
                //rect = this.GetItemRect(itemParentGuid, itemGuid);
                //if (firstTime)
                //    ShowHighLightItem(itemGuid, itemParentGuid, restoration, restorationData, v, false);
                //else
                //{
                //core.DispactchNewMessage("Don't can find the control, maybe is does not visible", MsgType.Console);
                //return;
                //}
            }
            if (rect.IsZero())
            {
                if (restoration != null)
                    restoration.Invoke(restorationData, v);
                return;
            }
            automation.Core.SetUIVisible = false;
            automation.Core.DispactchNewMessage(DateTime.Now.ToLongTimeString(), MsgType.Console);


            //tagWindow.Visibility = win.Visibility.Collapsed;

            if (corelApp.AppWindow.WindowState == cdrWindowState.cdrWindowMaximized)
            {
                if (printScreen)
                    printScreenForm = new PrintScreenForm(rect);
                else
                    overlayForm = new OverlayForm(rect);
            }

            else
            {
                System.Windows.Rect rect2 = new System.Windows.Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                if (printScreen)
                    printScreenForm = new PrintScreenForm(rect, rect2);
                else
                    overlayForm = new OverlayForm(rect, rect2);
            }
            //Thread.Sleep(1000);
            if (printScreen)
            {
                
                PrintScreen();
            }
            else
            {
               // overlayForm = new OverlayForm(automation,itemParentGuid, itemGuid,restoration,restorationData,v);
                CallForm();
            }
        }
        private void CallForm(Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false)
        {
            if (overlayForm.InvokeRequired)
            {
                var d = new Action<Action<IBasicData, bool>, IBasicData, bool>(CallForm);
                overlayForm.Invoke(restoration, restorationData, v);
            }
            else
            {
                //overlayForm.Show();
                overlayForm.Show();
                overlayForm.FormClosed += (s, e) =>
                {
                    automation.Core.SetUIVisible = true;
                    if (restoration != null)
                        restoration.Invoke(restorationData, v);
                };
            }


        }
        private void PrintScreen(Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false)
        {
            if (printScreenForm.InvokeRequired)
            {
                var d = new Action<Action<IBasicData, bool>, IBasicData, bool>(PrintScreen);
                printScreenForm.Invoke(restoration, restorationData, v);
            }
            else
            {
                printScreenForm.Show();
                printScreenForm.FormClosed += (s, e) =>
                {
                    automation.Core.SetUIVisible = true;
                    if (restoration != null)
                        restoration.Invoke(restorationData, v);
                };
            }


        }
        //public void showHighLightItem(win.DependencyObject dependencyObject, List<IBasicData> temp)
        //{
        //    Views.XMLTagWindow w = Core.FindParentControl<Views.XMLTagWindow>(dependencyObject) as Views.XMLTagWindow;
        //    if (w == null)
        //    {
        //        core.DispactchNewMessage("Error tagWindow not found", MsgType.Console);
        //        return;
        //    }

        //    showHighLightItem(w, temp);
        //}

    }
}
