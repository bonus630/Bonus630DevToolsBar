using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using br.com.Bonus630DevToolsBar.CustomControls;
using System.Threading;
using System.ComponentModel;


namespace br.com.Bonus630DevToolsBar.GMSDragger
{
    public class WinRarManager
    {
        string rarExePath = string.Empty;
        string tempPath = string.Empty;
        private ConcurrentQueue<string> queue;
        public event Action<string> UnRarFinish;
        private int filesToProcess = 0;
        private int currentFile = 0;
        private bool rarRunning = false;
        public void EnqueueFiles(string[] rarFiles)
        {
            if (queue == null)
                queue = new ConcurrentQueue<string>();
            for (int i = 0; i < rarFiles.Length; i++)
            {
                queue.Enqueue(rarFiles[i]);
            }
            filesToProcess += rarFiles.Length;
        }
        public void EnqueueFile(string file)
        {
            if (queue == null)
                queue = new ConcurrentQueue<string>();
            queue.Enqueue(file);
            filesToProcess++;
        }
        //public string ProcessRarFiles()
        //{

        //}
        public void ProcessFiles()
        {
            tempPath = GetTempDir();
            RunThread();
        }
        private void RunThread()
        {
            string path = string.Empty;
            if (queue.TryDequeue(out path))
            {
                processRar(path);
               // Thread tasks = new Thread(new ParameterizedThreadStart(processRar));
              //  tasks.IsBackground = true;
             //   tasks.Start(path);
                //tasks.Join();
                filesToProcess--;
                Debug.WriteLine("Rar " + path);
            }
            if (UnRarFinish != null && filesToProcess == 0)
                UnRarFinish(tempPath);
        }
        private void processRar(object rarFile)
        {
            try
            {
                if (string.IsNullOrEmpty(rarExePath))
                    rarExePath = FindWinrarPath();
                if (string.IsNullOrEmpty(rarExePath))
                {
                    System.Windows.Forms.MessageBox.Show("Winrar not founded!");
                    return;
                }
                rarExePath = Path.Combine(rarExePath, "Rar.exe");
                using (Process processo = new Process())
                {
                    processo.StartInfo.FileName = rarExePath;
                    processo.StartInfo.Arguments = string.Format(" x \"{0}\" \"{1}\"", rarFile, tempPath);
                    processo.StartInfo.UseShellExecute = true;
                    processo.StartInfo.RedirectStandardOutput = false;
                    processo.StartInfo.CreateNoWindow = false;
                    processo.Disposed += Processo_Disposed;
                    processo.Exited += Processo_Exited;
                    Debug.WriteLine("Rar directory exist:" + Directory.Exists(tempPath));
                    processo.Start();
                    rarRunning = true;
                    processo.WaitForExit();
                    Debug.WriteLine("Rar exitCode:" + processo.ExitCode);
                    processo.Close();
                }

            }
            catch(Win32Exception ex)
            {
                Debug.WriteLine(ex.ErrorCode);
            }

        }

        private void Processo_Exited(object sender, EventArgs e)
        {
            rarRunning = false;
        }

        private void Processo_Disposed(object sender, EventArgs e)
        {
            currentFile++;
            rarRunning = false;
            RunThread();
        }

        private string FindWinrarPath()
        {
            string[] paths = { @"C:\Program Files\WinRAR", @"C:\Program Files (x86)\WinRAR" };

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            string chaveRegistro = @"SOFTWARE\WinRAR";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(chaveRegistro))
            {
                if (key != null)
                {
                    object valor = key.GetValue("Path");
                    if (valor != null)
                    {
                        return valor.ToString();
                    }
                }
            }
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "Select WinRAR.exe";
            openFileDialog.Filter = "Rar.exe (Rar.exe)|*.exe";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;


            }
            return null;
        }
        private string GetTempDir()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
