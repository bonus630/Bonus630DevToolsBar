using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public class KeyData:BasicData<KeyData>
    {
        private bool shift = false;
        public bool Shift { get { Boolean.TryParse(GetAttribute("shift"), out shift); return shift; } }
        private bool ctrl = false;
        public bool Ctrl { get { Boolean.TryParse(GetAttribute("ctrl"), out ctrl); return ctrl; }  }
        private bool alt = false;
        public bool Alt { get { Boolean.TryParse(GetAttribute("alt"), out alt); return alt; } }
        private Key key = Key.None;
        public Key Key { get
            {
                if (this.Childrens.Count > 0)
                {
                    string s = "VK";
                    if (this.Childrens[0].Text.Contains("_")) ;
                    s += "_";
                    Enum.TryParse<Key>(this.Childrens[0].Text.Replace(s, "").UCFirst(), out key);
                } 
                return key; } }

       
    }
}
