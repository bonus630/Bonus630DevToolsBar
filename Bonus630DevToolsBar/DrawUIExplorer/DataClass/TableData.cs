using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public class TableData : BasicData<TableData>
    {
        public string TableID { get; set; }
        public override string GetAnyGuidAttribute()
        {
            return GetAttribute("tableID");
        }
    }
}
