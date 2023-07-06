using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.Models
{
    public class ComparerKey : IComparer<Shortcut>

    {
        public int Compare(Shortcut x, Shortcut y)
        {
            return string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase);
        }
    }
}
