using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    public abstract class ViewModelDataBase : ViewModelBase
    {
        private IBasicData basicData;
   
        public IBasicData CurrentBasicData
        {
            get { return basicData; }
            set { basicData = value; OnPropertyChanged(); }
        }
        public ViewModelDataBase(Core core)
        {
            this.Core = core;
            if (core.InCorel)
                core.CurrentBasicDataChanged += Update;
            else
                core.CurrentBasicDataChanged += UpdateNoAttached;


        }
        public  Core Core { get; set; }

        protected abstract void Update(IBasicData basicData);

        protected abstract void UpdateNoAttached(IBasicData basicData);


    }
}
