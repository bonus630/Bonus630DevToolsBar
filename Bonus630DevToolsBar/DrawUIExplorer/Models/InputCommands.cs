using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


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
        public void TryHighLightItem(string strItemGuid, string strItemParentGuid)
        {
            core.HighLightItemHelper.ShowHighLightItem(strItemGuid, strItemParentGuid);
        }
        [InCorelAtt(true)]
        public void RunMacro(string strMacro) 
        {
            core.CorelAutomation.RunMacro(strMacro);
        }
        [InCorelAtt(false)]
        public void AddLabelToActiveTag(string strLabel)
        {
            core.CurrentBasicData.Label = strLabel;
        }
        [InCorelAtt(false)]
        public void AttachDisattachCorelDRW(string versionMajor)
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
                    core.DispactchNewMessage("Coreldraw {0} disattached", MsgType.Erro,version);
                }
            }
            else
            {
               
                Int32.TryParse(versionMajor, out version);
                if(version==0)
                {
                    core.DispactchNewMessage("Invalid version", MsgType.Erro);
                }
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        Type pia_type = Type.GetTypeFromProgID(string.Format("CorelDRAW.Application.{0}",version));
                        core.CorelApp = (Corel.Interop.VGCore.Application) Activator.CreateInstance(pia_type);

                        int i = 0;
                        while (core.CorelApp == null || i > 50)
                        {
                            if (core.CorelApp != null)
                            {
                                core.InCorel = true;
                                core.DispactchNewMessage("Sucess to connect!", MsgType.Result);
                            }
                            i++;
                            Thread.Sleep(1000);
                        }
                    }
                    catch
                    {
                        core.DispactchNewMessage("Failed to start CorelDraw",MsgType.Erro);
                        
                    }
                
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }
        [InCorelAtt(true)]
        public string CorelVersionMajor()
        {
            return core.CorelApp.VersionMajor.ToString();
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
                InCorelAtt corelAtt =  (InCorelAtt)m[i].GetCustomAttribute(typeof(InCorelAtt), false);

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
