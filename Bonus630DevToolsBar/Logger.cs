using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar
{
    internal static class Logger
    {
        internal static void Log(string message,string trace = null)
        {
            Task.Run(() =>
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bonus630\\Bonus630DevToolsBar.log";
                if (!string.IsNullOrEmpty(trace))
                    trace = string.Format("Stack Trace: {0}\n", trace);
                string logMessage = string.Format("[{0}] Erro: {1}\n{2}\n", DateTime.Now, message, trace);
                File.AppendAllText(path, logMessage);
            });
        }
        internal static void Log(Exception e)
        {
            Logger.Log(e.Message, e.StackTrace);
        }   

    }
}
