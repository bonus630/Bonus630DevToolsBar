using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Diagnostics;
using System.Reflection;

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
        public void ExtractFiles()
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
            templatePath = Path.Combine(AddonFolder, "Templates", template);
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
            try
            {
                using (FileStream fs = new FileStream(tempPath, FileMode.Open))
                {
                    ZipArchive z = new ZipArchive(fs);
                    var entries = z.Entries;
                    for (int i = 0; i < entries.Count; i++)
                    {

                        string folder = ProjectFolder;

                        string[] folders = entries[i].FullName.Split('/');
                        for (int f = 0; f < folders.Length; f++)
                        {
                            if (folders[f] != entries[i].Name)
                            {
                                folder = Path.Combine(folder, folders[f]);
                                Directory.CreateDirectory(folder);
                            }

                        }
                        string fileName = Path.Combine(folder, entries[i].Name);
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
        public void Build()
        {
            if (!string.IsNullOrEmpty(LastProject))
                StartMSBuild(string.Format("\"{0}\" /p:Configuration={1} /m", LastProject, config));
            //StartMSBuild(string.Format("\"{0}\" /p:Configuration=\"{1}\" /v:d /nologo /noconsolelogger{2}{3}"
            // , LastProject, config, "", loggerVariable));
        }

        protected string msbuildPath;
        private string config = "Release";

        public event Action<string> DataReceived;
        public event Action Finish;

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
                throw new Exception("MSBuild not found");
            else
                msbuildPath = string.Format("{0}\\MSBuild.exe", path);

            //loggerDll = Path.Combine(Application.StartupPath, "MSBuildLogger.dll");
            //if (File.Exists(loggerDll))
            //    loggerVariable = string.Format(" /logger:\"{0}\"", loggerDll);

            loggerVariable = string.Format(" /logger:\"{0}\"", Path.Combine(AddonFolder, "MSBuildLogger.dll"));



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
            if (DataReceived != null)
                DataReceived(e.Data);
        }
        protected void OnFinish()
        {
            if (Finish != null)
                Finish();
        }

    }
}
