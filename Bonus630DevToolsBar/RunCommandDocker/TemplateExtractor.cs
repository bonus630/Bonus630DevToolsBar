using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class TemplateExtractor : IDisposable
    {

        private ZipArchive zipArchive;
        private FileStream fileStream;

        public TemplateExtractor() { }
        /// <summary>
        /// Call OpenFile
        /// </summary>
        /// <param name="zipPath"></param>
        public TemplateExtractor(string zipPath)
        {
            OpenFile(zipPath);
        }

        private StreamReader GetStreamFromEntry(string entry)
        {
            ZipArchiveEntry textEntry = zipArchive.GetEntry(entry);
            if (textEntry == null)
            {
                throw new Exception(string.Format("The {0} not found in this file!", entry));
            }
            Stream stEntry = textEntry.Open();
            return new StreamReader(stEntry);
        }
        public void OpenFile(string zipPath)
        {
            if (!File.Exists(zipPath))
                throw new IOException("File not exists");
            fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);

        }
        public void CloseFile()
        {
            this.Dispose();
        }
        public void ExtractFile(string pathToSave, string zipEntry)
        {
            var entry = zipArchive.GetEntry(zipEntry);
            using (MemoryStream ms = new MemoryStream())
            {
                entry.Open().CopyTo(ms);
                File.WriteAllBytes(pathToSave, ms.ToArray());
            }
        }
        public void Extract(string dirToSave)
        {
            if (!Directory.Exists(dirToSave))
                Directory.CreateDirectory(dirToSave);
            var entries = zipArchive.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                string tempDir = dirToSave;
                string[] folders = entries[i].FullName.Split('/');
                for (int f = 0; f < folders.Length; f++)
                {
                    if (folders[f] != entries[i].Name)
                    {
                        tempDir = Path.Combine(dirToSave, folders[f]);
                        Directory.CreateDirectory(tempDir);
                    }
                }
                string fileName = Path.Combine(tempDir, entries[i].Name);
                ExtractFile(fileName, entries[i].FullName);
            }
        }
        public void Dispose()
        {
            if (zipArchive != null)
                zipArchive.Dispose();
            if (fileStream != null)
                fileStream.Dispose();
        }

    }
}
