using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace br.com.Bonus630DevToolsBar.DataSource
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    //[ClassInterface(ClassInterfaceType.None)]
    public class BaseDataSource : INotifyPropertyChanged
    {
        protected DataSourceProxy m_AppProxy;
        protected Application CorelApp;
        public BaseDataSource(DataSourceProxy proxy, Application corelApp)
        {
            this.m_AppProxy = proxy;
            this.CorelApp = corelApp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                this.m_AppProxy.UpdateListeners(propertyName);
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            catch { }
        }
    }
}
