using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar
{
    [Serializable]
    public class WorkspaceObjectTranporter
    {
        public string WorkspacePath = "";
        public string WorkspaceName = "";
        public string IconFolder = "";
        public List<string> Items = new List<string>();
    }
}
