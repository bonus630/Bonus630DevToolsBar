using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public class DynamicData : BasicData<DynamicData>
    {
        public string DynamicCommand { get; set; }
        public string DynamicCategory { get; set; }
    }
}
