using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class SimpleObject
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
