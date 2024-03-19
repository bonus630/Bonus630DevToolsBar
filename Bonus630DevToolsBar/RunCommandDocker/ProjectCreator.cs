﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Build.Execution;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Corel.Interop.VGCore;
using System.Runtime.Remoting.Messaging;
using Microsoft.Build.Exceptions;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class ProjectCreator
    {
        private string safeProjectName;
        private string projectName;
        private string loggerVariable;

        private readonly string[] toReplace = { "$safeprojectname$", "$vgcoredll$", "$assembliesFolder$", "$projectname$", "$guid2$", "$ModulePath$" };

        public string VgCore { get; set; }
        public string AssembliesFolder { get; set; }

        public string ProjectFolder { get; set; }
        public string LastProject { get; set; }
        public string AddonFolder { get; set; }
        public int Index { get; set; }
        //$safeprojectname$
        //$vgcoredll$
        //$assembliesFolder$
        //$projectname$
        //$guid2$
        public string RemoveInvalidChars(string text)
        {
            char[] chars = Path.GetInvalidFileNameChars();
            string nText = "";
            for (int i = 0; i < chars.Length; i++)
            {
                nText = text.Replace(chars[i].ToString(), string.Empty);
            }
            chars = Path.GetInvalidPathChars();
            for (int i = 0; i < chars.Length; i++)
            {
                nText = nText.Replace(chars[i].ToString(), "_");
            }
            chars = new char[] { ' ', '@', '#', '$' };
            for (int i = 0; i < chars.Length; i++)
            {
                nText = nText.Replace(chars[i].ToString(), "_");
            }
            return nText;
        }
        public void SetProjectName(string projectName)
        {
            safeProjectName = RemoveInvalidChars(projectName);
            this.projectName = projectName;
        }
        public void ReplaceFiles()
        {
            string[] fileList = { };
            switch (Index)
            {
                case 0:
                    fileList = GetCSFiles();
                    break;
                case 1:
                    fileList = GetVBFiles();
                    break;
            }

            for (int i = 0; i < fileList.Length; i++)
            {
                string path = Path.Combine(ProjectFolder, fileList[i]);
                try
                {
                    string text = File.ReadAllText(path);
                    text = text.Replace(toReplace[0], safeProjectName);
                    text = text.Replace(toReplace[1], VgCore);
                    text = text.Replace(toReplace[2], AssembliesFolder);
                    text = text.Replace(toReplace[3], projectName);
                    text = text.Replace(toReplace[4], Guid.NewGuid().ToString());
                    text = text.Replace(toReplace[5], path);
                    File.WriteAllText(path, text);
                    if (path.Contains("ProjFile"))
                    {
                        string npath = path.Replace("ProjFile", safeProjectName);
                        File.Move(path, npath);
                    }


                }
                catch (IOException eio)
                {
                    System.Windows.Forms.MessageBox.Show(eio.Message);
                }
                catch (Exception e) { System.Windows.Forms.MessageBox.Show(e.Message); }
            }
            LastProject = GetProjFullPath(ProjectFolder);
        }
        public string[] GetCSFiles()
        {
            string[] fileList = { "Main.cs", "CdrCustomAttributes.cs", "ProjFile.csproj", "PROPERTIES\\AssemblyInfo.cs" };
            return fileList;
        }
        public string[] GetVBFiles()
        {
            string[] fileList = { "Main.vb", "ProjFile.vbproj", "My Project\\AssemblyInfo.vb" };
            return fileList;
        }
        public void ExtractTemplate()
        {
            string templatePath = "";
            string template = "";
            switch (Index)
            {
                case 0:
                    template = "MacroClassLibraryCS.zip";
                    break;
                case 1:
                    template = "MacroClassLibraryVB.zip";
                    break;
            }
            templatePath = Path.Combine(AddonFolder, "RunCommandDocker\\Templates", template);
            string tempPath = Path.Combine(Path.GetTempPath(), template);
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch { }
            }
            try
            {
                File.Copy(templatePath, tempPath);
            }
            catch { }
            ExtractFiles(tempPath, ProjectFolder);
        }
        public void ExtractFiles(string zipPath, string destFolder)
        {
            try
            {
                using (FileStream fs = new FileStream(zipPath, FileMode.Open))
                {
                    ZipArchive z = new ZipArchive(fs);
                    var entries = z.Entries;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        string[] folders = entries[i].FullName.Split('/');
                        for (int f = 0; f < folders.Length; f++)
                        {
                            if (folders[f] != entries[i].Name)
                            {
                                destFolder = Path.Combine(destFolder, folders[f]);
                                Directory.CreateDirectory(destFolder);
                            }

                        }
                        string fileName = Path.Combine(destFolder, entries[i].Name);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            entries[i].Open().CopyTo(ms);
                            File.WriteAllBytes(fileName, ms.ToArray());

                        }
                    }
                }
            }
            catch (IOException eio)
            {
                System.Windows.Forms.MessageBox.Show(eio.Message);
            }
            catch (Exception e) { System.Windows.Forms.MessageBox.Show(e.Message); }
        }
        private string GetProjFullPath(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileInfo[] files = dirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension == ".csproj" || files[i].Extension == ".vbproj")
                    return files[i].FullName;
            }

            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                GetProjFullPath(dirs[i].FullName);
            }
            return "";
        }
        public void MSBuild()
        {
            if (!string.IsNullOrEmpty(LastProject))
                StartMSBuild(string.Format("\"{0}\" /p:Configuration={1} /m", LastProject, config));
            //StartMSBuild(string.Format("\"{0}\" /p:Configuration=\"{1}\" /v:d /nologo /noconsolelogger{2}{3}"
            // , LastProject, config, "", loggerVariable));
        }
        //private bool IsProjectInCollection(Microsoft.Build.Evaluation.Project project, IDictionary<string, string> globalProperties)
        //{
        //    foreach (ProjectInstance instance in BuildManager.DefaultBuildManager.GetProjectInstanceForBuild() .Keys)
        //    {
        //        if (instance.FullPath.Equals(project.FullPath, StringComparison.OrdinalIgnoreCase))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        Microsoft.Build.Evaluation.Project project;
        private string prevProject = string.Empty;
        public void DirectBuild()
        {
            if (!string.IsNullOrEmpty(LastProject))
            {

                try
                {
                    if(!LastProject.Equals(prevProject))
                        project = new Microsoft.Build.Evaluation.Project(LastProject);
                    prevProject = LastProject;
           
                    BuildRequestData requestData = null;
                

                    ErrorLogger eLog = new ErrorLogger();
                    eLog.Verbosity = LoggerVerbosity.Minimal;
                    eLog.BuildErrorEvent += (s, e) =>
                    {
                        OnErroReceived(e.Message);
                    };
                    eLog.BuildMessageEvent += (msg) =>
                    {
                        OnDataReceived(msg);
                    };
                    
                    BuildParameters buildParameters = new BuildParameters
                    {
                        Loggers = new[] { eLog }

                    };
                    List<string> properties = new List<string>();
                    properties.Add(config);

                    Task.Run(() =>
                    { 
                        requestData = new BuildRequestData(project.CreateProjectInstance(), new string[] { "Build" }, null, BuildRequestDataFlags.ReplaceExistingProjectInstance, properties);
                        BuildManager.DefaultBuildManager.Build(buildParameters, requestData);
                        // Compilação
                        //BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

                        //// Verifica se o projeto já está na coleção
                        ////if (!IsProjectInCollection(project, BuildManager.DefaultBuildManager.ProjectInstanceGlobalProperties))
                        //// {
                        //requestData = new BuildRequestData(project.CreateProjectInstance(),new string[] {"Build" }, null, BuildRequestDataFlags.ReplaceExistingProjectInstance, properties);

                        //BuildSubmission submission = BuildManager.DefaultBuildManager.PendBuildRequest(requestData);
                        //submission.ExecuteAsync(null, null);
                        //submission.WaitHandle.WaitOne();
                        //////}

                        //BuildManager.DefaultBuildManager.EndBuild();
                    });
                    ////try
                    //{
                   
                    //}
                    ////catch(InvalidProjectFileException fe)
                    ////{
                    ////    OnDataReceived(fe.Message);
                    ////}
                    //catch (Exception e)
                    //{
                    //    OnDataReceived(e.Message);
                    //}





                    // Compilação
                   // BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, requestData);

                    //if (buildResult.OverallResult == BuildResultCode.Success)
                    //{
                    //    OnFinish();
                    //}
                    //else
                    //{
                    //    OnDataReceived("Compile erro!");
                    //}
                }
                catch (Exception e)
                {
                    OnErroReceived(string.Format("Buid Erro:{0}", e.Message));
                }
                   
              
            }
        }
        protected string msbuildPath;
        private string config = "Release";

        public event Action<string> DataReceived;
        public event Action Finish;
        public event Action<string> ErroReceived;

        protected void SetMsBuildPath()
        {
            var ver = System.Environment.Version;
            string win = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows);
            string path = string.Format("{0}\\microsoft.net", win);
            string frame = "Framework64";
            if (!Directory.Exists(string.Format("{0}\\{1}", path, frame)))
                frame = "Framework";
            else if (!Directory.Exists(string.Format("{0}\\{1}", path, frame)))
                throw new Exception(".Net Framework not found");

            path = string.Format("{0}\\{1}\\v{2}.{3}.{4}", path, frame, ver.Major, ver.Minor, ver.Build);


            if (!File.Exists(string.Format("{0}\\MSBuild.exe", path)))
            {
                ExtractMsBuild(path);
            }

            if (!File.Exists(string.Format("{0}\\MSBuild.exe", path)))
                throw new Exception("MSBuild not found");
            else
                msbuildPath = string.Format("{0}\\MSBuild.exe", path);

            //loggerDll = Path.Combine(Application.StartupPath, "MSBuildLogger.dll");
            //if (File.Exists(loggerDll))
            //    loggerVariable = string.Format(" /logger:\"{0}\"", loggerDll);

            loggerVariable = string.Format(" /logger:\"{0}\"", Path.Combine(AddonFolder, "MSBuildLogger.dll"));



        }
        private void ExtractMsBuild(string path)
        {
            try
            {
                string zip = string.Format("{0}\\MSBuild.zip", AddonFolder);
                ExtractFiles(zip, path);
            }
            catch { }
        }
        public void StartMSBuild(string arguments)
        {
            if (string.IsNullOrEmpty(msbuildPath))
                SetMsBuildPath();

            Process psi = new Process();

            psi.StartInfo.CreateNoWindow = true;
            psi.StartInfo.UseShellExecute = false;
            psi.EnableRaisingEvents = true;
            psi.StartInfo.FileName = msbuildPath;
            psi.StartInfo.Arguments = arguments;
            psi.StartInfo.RedirectStandardOutput = true;
            psi.OutputDataReceived += R_OutputDataReceived;
            psi.Exited += Psi_Exited;
            psi.Start();

            psi.BeginOutputReadLine();
        }

        protected void Psi_Exited(object sender, EventArgs e)
        {
            OnFinish();
        }

        private void R_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
           OnDataReceived(e.Data);
        }
        protected void OnFinish()
        {
            if (Finish != null)
                Finish();
        }
        private void OnDataReceived(string msg)
        {
            if (DataReceived != null)
                DataReceived(msg);
        }
        private void OnErroReceived(string msg)
        {
            if (ErroReceived != null)
                ErroReceived(msg);
        }
    }

    // Implementação de um Logger personalizado para capturar erros
    class ErrorLogger : ILogger
    {
        public BuildErrorEventHandler BuildErrorEvent;

        public Action<string> BuildMessageEvent;

        public Action BuildStartEvent;

        public string Parameters { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.ErrorRaised += (sender, e) =>
            {
                if (BuildErrorEvent != null)
                    BuildErrorEvent(sender, e);
            };
            eventSource.AnyEventRaised += (sender, e) =>
            {
                if (BuildMessageEvent != null)
                    BuildMessageEvent(e.Message);
            };
            eventSource.BuildStarted += (sender, e) =>
            {
                if (BuildStartEvent != null)
                    BuildStartEvent();
            };
        }

      


        public void Shutdown()
        {
        }

        public LoggerVerbosity Verbosity { 
            get; 
            set; }
    }

}
