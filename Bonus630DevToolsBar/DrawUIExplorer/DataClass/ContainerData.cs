using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public class ContainerData : BasicData<ContainerData>
    {
        public ContainerData() : base()
        {
            isContainer = true;
        }
    }
}
