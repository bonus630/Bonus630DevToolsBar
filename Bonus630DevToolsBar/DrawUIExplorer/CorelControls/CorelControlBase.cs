using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.CorelControls
{
    public class CorelControlBase
    {
        public string Name { get; set; }
        public string Guid { get; set; }

        public CorelControlBase(string name, string guid)
        {
            Name = name;
            Guid = guid;
        }
    }
}
