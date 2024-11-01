using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class InputCommands
    {
        Core core;
        public InputCommands(Core core)
        {
            this.core = core;
        }
        [InCorelAtt(false)]
        public string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }
        [InCorelAtt(true)]
        public string CQL(string strCQL)
        {
            return core.CorelApp.Evaluate(strCQL).ToString();
        }
        [InCorelAtt(true)]
        public void InvokeItem(string strItemGuid)
        {
            core.CorelAutomation.InvokeItem(strItemGuid);
        }
        [InCorelAtt(true)]
        public void InvokeDialogItem(string strDialogGuid, string strItemGuid)
        {
            core.CorelAutomation.InvokeDialogItem(strDialogGuid, strItemGuid);
        }
        [InCorelAtt(true)]
        public int GetHwnd(string strItemGuid, string strItemParentGuid)
        {
           return core.CorelAutomation.GetHwnd(strItemParentGuid, strItemGuid);

        }
        [InCorelAtt(true)]
        public void TryHighLightItem(string strItemGuid, string strItemParentGuid)
        {
            core.HighLightItemHelper.ShowHighLightItem(strItemGuid, strItemParentGuid);

        }
        [InCorelAtt(true)]
        public string TryGetAnyCaption(string strItemGuid)
        {
            string result = string.Empty;
            result = core.TryGetAnyCaption(strItemGuid);
            if (result.Equals(string.Empty))
                result = "Caption not founded!";
            return result;
        }
        [InCorelAtt(true)]
        public void RunMacro(string strMacro)
        {
            core.CorelAutomation.RunMacro(strMacro);
        }
        [InCorelAtt(true)]
        public void RunJavascript(string strMacro)
        {
            core.CorelAutomation.RunJavascript(strMacro);
        }
        [InCorelAtt(true)]
        public void RunVSTA(string strMacro)
        {
            core.CorelAutomation.RunVSTA(strMacro);
        }
        [InCorelAtt(false)]
        public void AddLabelToActiveTag(string strLabel)
        {
            core.CurrentBasicData.Label = strLabel;
        }
        [InCorelAtt(false)]
        public void AttachDisattachCorelDRW(string intVersionMajor)
        {
            int version = 0;
            if (core.InCorel)
            {

                try
                {
                    version = core.CorelApp.VersionMajor;
                    Marshal.ReleaseComObject(core.CorelApp);
                    core.InCorel = false;
                    core.CorelApp.ToString();
                }
                catch
                {
                    core.DispactchNewMessage("Coreldraw {0} disattached", MsgType.Erro, version);
                }
            }
            else
            {
                core.DispactchNewMessage("Starting connection!", MsgType.Console);
                Int32.TryParse(intVersionMajor, out version);
                if (version == 0)
                {
                    core.DispactchNewMessage("Invalid version", MsgType.Erro);
                }
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        Type pia_type = Type.GetTypeFromProgID(string.Format("CorelDRAW.Application.{0}", version));
                        var app = (Corel.Interop.VGCore.Application)Activator.CreateInstance(pia_type);
                        core.CorelApp = app;

                        int i = 0;
                        while (core.CorelApp == null && i < 60)
                        {
                            i++;
                            Thread.Sleep(1000);
                            core.DispactchNewMessage("Waiting connection. Time out: {0}s", MsgType.Console, i);
                        }
                        if (core.CorelApp != null)
                        {
                            core.InCorel = true;
                            core.CorelApp.Visible = true;
                            core.DispactchNewMessage("Sucess to connect!", MsgType.Result);
                        }
                    }
                    catch
                    {
                        core.DispactchNewMessage("Failed to start CorelDraw", MsgType.Erro);

                    }

                });
                thread.IsBackground = true;
                thread.Start();
            }
        }

        [InCorelAtt(false)]
        public void Reattach()
        {
            if(core.CorelLastVersion > 0)
            {
                core.DispactchNewMessage("Starting connection!", MsgType.Console);
                try
                {
                   
                   // var app = (Corel.Interop.VGCore.Application)Marshal.GetActiveObject(string.Format("CorelDRAW.Application.{0}", core.CorelLastVersion));
                    Type pia_type = Type.GetTypeFromProgID(string.Format("CorelDRAW.Application.{0}", core.CorelLastVersion));
                    var app = (Corel.Interop.VGCore.Application)Activator.CreateInstance(pia_type);
                    core.CorelApp = app;
                    if (core.CorelApp != null)
                    {
                        core.InCorel = true;
                        core.DispactchNewMessage("Sucess to connect!", MsgType.Result);
                    }

                }
                catch(Exception e)
                {
                    core.DispactchNewMessage(e.Message, MsgType.Erro);
                }
            }
            else
            {
                core.DispactchNewMessage("No version to reattach!", MsgType.Erro);
            }
        }
        
        [InCorelAtt(true)]
        public string CorelVersionMajor()
        {
            return core.CorelApp.VersionMajor.ToString();
        }
        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint uCode, uint uMapType);

        [InCorelAtt(false)]
        public string MapKeyCode(string uintVirtualKeyCode)
        {
            UInt32 keyCode = 0;
            if (UInt32.TryParse(uintVirtualKeyCode, out keyCode))
            {

                int scanCode = MapVirtualKey(keyCode, 0);
                Key key = KeyInterop.KeyFromVirtualKey((int)keyCode);
                return string.Format("KeyCode: {0} | ScanCode: {1} | Key: {2}", keyCode, scanCode, key);
            }
            else
            {
                core.DispactchNewMessage("Param format invalid!", MsgType.Erro);
                return "";
            }
        }

        [InCorelAtt(true)]
        public void SetWindowSize(string intWidth, string intHeight)
        {
            int w = 0, h = 0;
            if (Int32.TryParse(intWidth, out w) && Int32.TryParse(intHeight, out h))
            {
                core.CorelApp.AppWindow.Width = w;
                core.CorelApp.AppWindow.Height = h;
            }
            else
            {
                core.DispactchNewMessage("Param format invalid!", MsgType.Erro);

            }
        }
        [InCorelAtt(true)]
        public void SetWindowPosition(string intX, string intHY)
        {
            int x = 0, y = 0;
            if (Int32.TryParse(intX, out x) && Int32.TryParse(intHY, out y))
            {
                core.CorelApp.AppWindow.Left = x;
                core.CorelApp.AppWindow.Top = y;
            }
            else
            {
                core.DispactchNewMessage("Param format invalid!", MsgType.Erro);

            }
        }

        [InCorelAtt(false)]
        public string Help()
        {
            string result = "";
            List<MethodInfo> m = new List<MethodInfo>();
            m.AddRange((typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            m = m.OrderBy(r => r.Name).ToList();
            for (int i = 0; i < m.Count; i++)
            {
                InCorelAtt corelAtt = (InCorelAtt)m[i].GetCustomAttribute(typeof(InCorelAtt), false);

                if (corelAtt.Value && core.InCorel || !corelAtt.Value)
                {
                    string input = m[i].Name;

                    ParameterInfo[] parameters = m[i].GetParameters();
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        input += " [" + parameters[j].Name + "]";
                    }
                    result += input;

                    //if (attributes.)
                    if (i < m.Count - 1)
                        result += Environment.NewLine;
                }
            }
            return result;
        }

    }
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class InCorelAtt : System.Attribute
    {
        private bool _value;

        public InCorelAtt(bool value)
        {
            _value = value;
        }

        public bool Value
        {
            get { return _value; }
        }
    };
}
