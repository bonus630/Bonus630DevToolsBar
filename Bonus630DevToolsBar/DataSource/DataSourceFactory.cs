﻿using Corel.Interop.VGCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace br.com.Bonus630DevToolsBar.DataSource
{
    public class DataSourceFactory : ICUIDataSourceFactory
    {
        private Dictionary<string, Type> DataSourceList = new Dictionary<string, Type>();

        private Application corelApp;

        public DataSourceFactory(Application corelApp)
        {
            this.corelApp = corelApp;
        }

        public void AddDataSource(string name, Type dataSource)
        {
            DataSourceList.Add(name, dataSource);
        }

        public void Register()
        {
            foreach (string name in DataSourceList.Keys)
            {
                bool registred = ControlUI.corelApp.FrameWork.Application.RegisterDataSource(name, this);
            }
        }
        public void CreateDataSource(string DataSourceName, DataSourceProxy Proxy, out object ppVal)
        {
            if (DataSourceList.ContainsKey(DataSourceName))
            {
                Type type = DataSourceList[DataSourceName];
                ppVal = type.Assembly.CreateInstance(type.FullName, true, System.Reflection.BindingFlags.CreateInstance, null, new object[] { Proxy,corelApp }, null, null);
                return;
            }
            ppVal = null;
        }

        public object CreateDataSource(string DataSourceName, DataSourceProxy Proxy)
        {
            object ppVal = null;
            if (DataSourceList.ContainsKey(DataSourceName))
            {
                Type type = DataSourceList[DataSourceName];
                ppVal = type.Assembly.CreateInstance(type.FullName, true, System.Reflection.BindingFlags.CreateInstance, null, new object[] { Proxy, corelApp }, null, null);

            }
            return ppVal;
        }
    }
}
