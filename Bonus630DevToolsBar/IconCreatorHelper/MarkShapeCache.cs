using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.IconCreatorHelper
{
    public class MarkShapeCache
    {
        private Corel.Interop.VGCore.Application corelApp;

        public MarkShapeCache(Corel.Interop.VGCore.Application corelApp)
        {
                this.corelApp = corelApp;
        }

        private List<int> staticIdCache = new List<int>();
        public int PageSize{get;private set;}  
        public double Left { get; private set; }
        public double Bottom { get; private set; }
        public void StoreStaticIdCache(ShapeRange sr,int pageSize)
        {
            staticIdCache.Clear();
            this.PageSize = pageSize;
            this.Left = sr.LeftX;
            this.Bottom = sr.BottomY;
            try
            {

                for (int i = 1; i <= sr.Count; i++)
                {
                    int id = sr[i].StaticID;
                    if (!staticIdCache.Contains(id))
                        staticIdCache.Add(id);
                }
                staticIdCache.Sort();
            }
            catch { }
        }
        public ShapeRange RecoverStaticIdCache(Page page)
        {
            ShapeRange sr = corelApp.CreateShapeRange();
            try
            {
                for (int i = 0; i < staticIdCache.Count; i++)
                {
                    Shape n = page.Shapes.FindShape(StaticID: staticIdCache[i]);
                    if (n != null)
                        sr.Add(n);
                }
            }
            catch { }
            
            return sr;
        }
    }
}
