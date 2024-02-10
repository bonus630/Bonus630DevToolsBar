using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using Corel.Interop.VGCore;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Controls;


namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class CorelAutomation
    {
        private Application corelApp;
        private Core core;
        public Core Core { get { return this.core; } }
        public CorelAutomation(Application app, Core core)
        {
            this.corelApp = app;
            this.core = core;
        }

        public string GetCaptionDocker(string guid)
        {
            corelApp.FrameWork.ShowDocker(guid);
            string caption = corelApp.FrameWork.Automation.GetCaptionText(guid);
            corelApp.FrameWork.HideDocker(guid);
            return caption;
        }

        public string GetActiveMenuItemGuid(int index)
        {
#if X7
            return "";
#else
            return corelApp.FrameWork.Automation.GetActiveMenuItemGuid(index);
#endif
        }
        public string a(string guid, string propertieName)
        {
            ICUIControlData data = corelApp.FrameWork.Automation.GetControlData(guid);
            return data.GetValue(propertieName).ToString();
        }
        public string CommandBarModeName()
        {
            CommandBarModes modes = corelApp.FrameWork.CommandBars[0].Modes;
            return modes[0].Name;
        }
        public System.Windows.Rect GetItemRect(string guidParent, string guidItem)
        {
            int left, top, width, height = 0;
            bool data = corelApp.FrameWork.Automation.GetItemScreenRect(guidParent, guidItem, out left, out top, out width, out height);
            core.DispactchNewMessage(string.Format("X:{0},Y:{1},W:{2},H:{3}", left, top, width, height), MsgType.Result);
            return new System.Windows.Rect() { X = left, Y = top, Width = width, Height = height };

        }
        internal System.Windows.Rect GetCorelRect()
        {
            ScreenRect sr = corelApp.FrameWork.MainFrameWindow.Position;
            core.DispactchNewMessage(string.Format("X:{0},Y:{1},W:{2},H:{3}", sr.Left, sr.Top, sr.Width, sr.Height), MsgType.Result);
            return new System.Windows.Rect() { X = sr.Left,Y = sr.Top, Width = sr.Width, Height = sr.Height };
        }
        public string GetActiveGuidTool()
        {
            string guid = string.Empty;
            try
            {
                guid = corelApp.ActiveToolStateGuid;
                core.DispactchNewMessage(string.Format("The current tool guid: {0}", guid),MsgType.Result);
               
            }
            catch(Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }
            return guid;
        }
        public string GetCaption(string guid, bool removeAmpersand = false)
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
        public void InvokeItem(IBasicData basicData)
        {
            try
            {
                string guid;
                if (!string.IsNullOrEmpty(basicData.Guid))
                    guid = basicData.Guid;
                else
                    guid = basicData.GuidRef;
                this.corelApp.FrameWork.Automation.InvokeItem(guid);
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }
        }

        public void ShowHideCommandBar(IBasicData basicData, bool show = true)
        {
            try
            {
                corelApp.EventsEnabled = false;
                string commandBarCaption = GetItemCaption(basicData);
                this.ShowHideCommandBar(commandBarCaption, show);
            }
            catch { }
            finally
            {
                corelApp.EventsEnabled = true;
            }
        }
        public void ShowHideCommandBar(string commandBarName, bool show = true)
        {
            //How close a commandBar in a popup
            try
            {
                if (!string.IsNullOrEmpty(commandBarName))
                    this.corelApp.FrameWork.CommandBars[commandBarName].Visible = show;

            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }

        }
        public void ShowHideDocker(IBasicData basicData, bool show = true)
        {
            try
            {
                corelApp.EventsEnabled = false;
                if (show)
                    ShowDocker(basicData.Guid);
                else
                    HideDocker(basicData.Guid);
            }
            catch { }
            finally
            {
                corelApp.EventsEnabled = true;
            }
        }

        public void ShowHideDialog(IBasicData basicData, bool show = true)
        {
            try
            {
                corelApp.EventsEnabled = false;
                if (show)
                    ShowDialog(basicData.Guid);
                else
                    HideDialog(basicData.Guid);
            }
            catch { }
            finally
            {
                corelApp.EventsEnabled = true;
            }
        }

        public void ShowBar(string guid)
        {
            try
            {
                this.corelApp.FrameWork.Automation.ShowBar(guid);
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }

        }
        public void CommandBarMode(IBasicData basicData, bool show = true)
        {
            string commandBarCaption = GetItemCaption(basicData);
            this.CommandBarMode(commandBarCaption, true);
        }
        public void CommandBarMode(string commandBarName, bool show = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(commandBarName))
                {
                    CommandBar commandBar = this.corelApp.FrameWork.CommandBars[commandBarName];

                    // var ctr = this.app.FrameWork.Automation.GetControlData("");
                    // commandBar.Controls.Count
                    CommandBarModes modes = commandBar.Modes;
                    // string guid = corelApp.FrameWork.Automation.GetActiveMenuItemGuid(0);
                    //corelApp.FrameWork.Automation.InvokeDialogItem(GuidDialog, guiditem);
                    core.DispactchNewMessage("CommandBarMode: " + commandBar.Controls.Count.ToString(), MsgType.Result);
                }
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }
        }
        public string GetItemCaption(string guid)
        {
            try
            {
                string caption = "";
                caption = this.corelApp.FrameWork.Automation.GetCaptionText(guid);
                return caption;
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }
            return "";
        }
        public string GetItemCaption(IBasicData basicData)
        {
            try
            {
                string commandBarCaption = "";
                commandBarCaption = this.corelApp.FrameWork.Automation.GetCaptionText(basicData.Guid);
                if (string.IsNullOrEmpty(commandBarCaption))
                    commandBarCaption = core.TryGetAnyCaption(basicData);
                return commandBarCaption;
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);
            }
            return "";
        }
        public void InvokeItem(string itemGuid)
        {
            corelApp.FrameWork.Automation.InvokeItem(itemGuid);
        }
        public void InvokeDialogItem(string dialogGuid, string itemGuid)
        {
            corelApp.FrameWork.Automation.InvokeDialogItem(dialogGuid, itemGuid);
        }
        public void RunBindDataSource(string value, bool invoke = false,string param = "")
        {
            string o = "";
            Type type = null;
            try
            {
                string pattern = @"(DataSource=(?<datasource>[a-zA-Z0-9]+);Path=(?<path>[0-9a-zA-Z]{0,}))";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(value);
                string datasource = match.Groups["datasource"].Value;
                string path = match.Groups["path"].Value;
                DataSourceProxy dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(datasource);
                if (invoke)
                {
                    dsp.InvokeMethod(path);
                    
                }
                else
                {
                    if (!String.IsNullOrEmpty(param))
                        dsp.SetProperty(path, param);
                    else
                    {
                        object j = dsp.GetProperty(path);
                        if(j==null)
                        {
                            core.DispactchNewMessage("Path:{0} returns null", MsgType.Result,path);
                        }
                        type = j.GetType();
                        o = j.ToString();
                        if (type == typeof(string) && core.IsXMLString(o))
                        {
                            core.DispactchNewMessage("Type: Xml String, Check in \"Xml Viewer\" tab", MsgType.Console);
                            core.DispactchNewMessage(core.FormatXml(o), MsgType.Xml);
                        }
                        else if (Marshal.IsComObject(j))
                        {
                            List<Type> typer = GetIUnknown(j);
                            string r = type.Name;
                            for (int i = 0; i < typer.Count; i++)
                            {
                                r += "," + typer[i].Name;
                            }
                            o = string.Format("Type:{0}", r);
                            core.DispactchNewMessage(o, MsgType.Result);
                        }
                        else
                        {
                            o = string.Format("Type:{0} Value:{1}", type, o);
                            core.DispactchNewMessage(o, MsgType.Result);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                o = erro.Message;
                core.DispactchNewMessage(o, MsgType.Erro);
            }
        }
        public void RunBindWithParamDataSource(string value, bool invoke = false, string param = "")
        {
            string result = string.Empty;
            try
            {
                
#if !X7
                result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif

            }
            catch { }
            Views.ParamBox pb = new Views.ParamBox();
            pb.ChangeTheme(result);
            if ((bool) pb.ShowDialog())
            {
                 param = pb.Param;
                if (!String.IsNullOrEmpty(param))
                    RunBindDataSource(value, invoke, param);
            }
        }
        private List<Type> GetIUnknown(object dataSourceObject)
        {
            IntPtr iu = Marshal.GetIUnknownForObject(dataSourceObject);
            return GetIUnknown(iu);
        }
        public List<Type> GetIUnknown(string guidParent, string guidItem)
        {
            //int itemHwnd = corelApp.FrameWork.Automation.GetItemInstanceHwnd(guidParent, guidItem);
            //corelApp.FrameWork.Automation.GetControlData
            //IntPtr iu = new IntPtr(itemHwnd);
            dynamic data = corelApp.FrameWork.Automation.GetControlData(guidItem);
            IntPtr iu = Marshal.GetIUnknownForObject(data);
            
            return GetIUnknown(iu);
        }
        private List<Type> GetIUnknown(IntPtr iu)
        {
            Type anyCDRComType = typeof(ICUIImageList);
            List<Type> iTypeList = new List<Type>();
            Assembly asm = Assembly.GetAssembly(anyCDRComType);
            Type[] eTypes = asm.GetTypes();
            foreach (Type item in eTypes)
            {
                
                Guid iuGuid = item.GUID;
                if (!item.IsInterface || iuGuid == Guid.Empty)
                    continue;
                IntPtr hModule;
                Marshal.QueryInterface(iu, ref iuGuid, out hModule);
                if (hModule != IntPtr.Zero)
                {
                    iTypeList.Add(item);
                }
            }
            return iTypeList;
        }
       
        public void ShowDialog(string guid)
        {
            try
            {
#if !X7
                this.corelApp.FrameWork.ShowDialog(guid);
#endif
            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Erro);
            }


        }
        public void HideDialog(string guid)
        {

            try
            {
#if !X7
                this.corelApp.FrameWork.HideDialog(guid);
#endif
            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Erro);
            }


        }
        public void ShowDocker(string guid)
        {
            try
            {

                this.corelApp.FrameWork.ShowDocker(guid);

            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Erro);
            }


        }
        public void HideDocker(string guid)
        {

            try
            {

                this.corelApp.FrameWork.HideDocker(guid);

            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Erro);
            }


        }
        //public int GetCommandBarIndexByGuid(string guid)
        //{
        //    for (int i = 1; i <= corelApp.FrameWork.CommandBars.Count; i++)
        //    {
        //        CommandBar commandBar = corelApp.FrameWork.CommandBars[i];
        //        if (commandBar.Controls.Count == 0 || commandBar.Controls.Count != this.CommandBarMode()
        //            continue;
        //        string controlId = commandBar.Controls[1].ID;
        //        //var b = core.SearchItemContainsGuidRef(controlId);
        //        var b = core.SearchEngineGet.SearchItemFromGuid(core.ListPrimaryItens, guid, false);
        //        if (b.Childrens.Count == 1)
        //            b = b.Childrens[0];
        //        else
        //            continue;
        //        while (b.GetType() != typeof(CommandBarData))
        //        {
        //            b = b.Parent;
        //        }
        //        if (b.Guid == guid)
        //            return i;
        //    }
        //    return 0;
        //}
        public void RunMacro(string value,params object[] args)
        {
            try
            {
                string module = value.Substring(0, value.LastIndexOf("."));
                string macro = value.Substring(value.LastIndexOf(".") + 1, value.Length - (module.Length + 1));
                object o = this.corelApp.GMSManager.RunMacro(module, macro, args);
                core.DispactchNewMessage("{0}", MsgType.Console, o);
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Erro);

            }
        }

       
    }
}
