using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Shell;

namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    public class SharpCompressManager
    {
        private string tempPath = string.Empty;
        public string TempPath { get { return tempPath; } protected set { tempPath = value; } }
        List<string> queue;
        public SharpCompressManager()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                if (args.Name.Contains("System.Runtime.CompilerServices.Unsafe"))
                {
                    string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Addons\\Bonus630DevToolsBar", "System.Runtime.CompilerServices.Unsafe.dll");
                    if (File.Exists(assemblyPath))
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                }
                if (args.Name.Contains("System.Runtime"))
                {
                    string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Addons\\Bonus630DevToolsBar", "System.Runtime.dll");
                    if (File.Exists(assemblyPath))
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                }
                return args.RequestingAssembly;
            };
        }
        public void ProcessFiles()
        {
            tempPath = GetTempDir();
            for (int i = 0; i < queue.Count; i++)
            {
                processRar(queue[i]);
            }
        }
        public void EnqueueFile(string file)
        {
            if (queue == null)
                queue = new List<string>();
            queue.Add(file);
           
        }
        private void processRar(string rarFile)
        {
            try
            {
                using (var archive = RarArchive.Open(rarFile))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        entry.WriteToDirectory(tempPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private string GetTempDir()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
    }

}
