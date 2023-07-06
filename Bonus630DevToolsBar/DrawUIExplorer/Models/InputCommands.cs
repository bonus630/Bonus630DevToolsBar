using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }
        public string CQL(string cql)
        {
           return core.CorelApp.Evaluate(cql).ToString();
        }
        public void InvokeItem(string itemGuid)
        {
            core.CorelAutomation.InvokeItem(itemGuid);
        }
        public void InvokeDialogItem(string dialogGuid, string itemGuid)
        {
            core.CorelAutomation.InvokeDialogItem(dialogGuid, itemGuid);
        }
        public void TryHighLightItem(string itemGuid, string itemParentGuid)
        {
            core.HighLightItemHelper.ShowHighLightItem(itemGuid, itemParentGuid);
        }
        public void RunMacro(string macro) 
        {
            core.CorelAutomation.RunMacro(macro);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Help()
        {
            string result = "";
            MethodInfo[] m = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < m.Length; i++)
            {
                string input = m[i].Name;
                
                ParameterInfo[] parameters = m[i].GetParameters();
                for (int j = 0; j < parameters.Length; j++)
                {
                    input += " [" + parameters[j].Name+"]";
                }
                result += input;
                
                //if (attributes.)
                if (i < m.Length - 1)
                    result += Environment.NewLine;
            }
            return result;
        }
       
    }
}
