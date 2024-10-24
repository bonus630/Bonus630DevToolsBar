using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.Models
{
    public class Shortcut
    {
        public Shortcut()
        {
            Key = new string[] {
            String.Empty };
        }

        public string Name { get; set; }
        public string Guid { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public string[] Key { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return Guid.Equals((obj as Shortcut).Guid);   
        }
        public override int GetHashCode()
        {
            return Guid.GetHashCode();  
        }

    }
}
