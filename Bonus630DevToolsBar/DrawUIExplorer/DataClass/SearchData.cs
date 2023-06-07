using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public class SearchData : BasicData<SearchData>
    {
        public System.Guid ID { get; protected set; }

        public override string TagName{ get { return "Search"; } set { } }

        public SearchData():base()
        {
            ID = System.Guid.NewGuid();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            IBasicData basicData = obj as IBasicData;
            if (basicData == null)
                return false;
            if (basicData.GetType() == typeof(DataClass.SearchData))
                return this.ID.Equals((basicData as SearchData).ID);
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
