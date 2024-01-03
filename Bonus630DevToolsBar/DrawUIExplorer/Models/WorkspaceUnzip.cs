using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Drawing;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class WorkspaceUnzip :IDisposable
    {
        private StreamReader xmlStreamReader;
        public StreamReader XmlStreamReader { get { return this.xmlStreamReader; } }
        private ZipArchive zipArchive;
        private Stream mainStream;
        public WorkspaceUnzip(Stream fileStream)
        {
            processZip(fileStream);
        }
        public WorkspaceUnzip(FileInfo file)
        {
            mainStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            processZip(mainStream);
           
           
        }
        private void processZip(Stream fileStream)
        {
           
                zipArchive = new ZipArchive(fileStream);
                xmlStreamReader = GetStreamFromEntry("content/workspace.xml", zipArchive);
          
           
        }
        private StreamReader GetStreamFromEntry(string entry, ZipArchive zipFile)
        {
            
            ZipArchiveEntry textEntry = zipFile.GetEntry(entry);
            if(textEntry == null)
            {
                throw new Exception(string.Format("The {0} not found in this file!", entry));
            }
            Stream stEntry = textEntry.Open();
            return new StreamReader(stEntry);
        
 
        }
        private string GetStringFromEntry(string entry, ZipArchive zipFile)
        {
            //string text = "";
            //ZipArchiveEntry textEntry = zipFile.GetEntry(entry);
            //using (Stream stEntry = textEntry.Open())
            //{
            //    using (StreamReader sr = new StreamReader(stEntry))
            //    {
            //        text = sr.ReadToEnd();
            //    }
            //}
            //return text;
            return GetStreamFromEntry(entry, zipFile).ReadToEnd();
        }
        private Bitmap GetBitmapFromEntry(string entry, ZipArchive zipFile)
        {
            ZipArchiveEntry thumbEntry = zipFile.GetEntry(entry);
            Bitmap bitmap;
            using (Stream thumbStream = thumbEntry.Open())
            {
                bitmap = new Bitmap(thumbStream);

            }
            return bitmap;
        }

        public void Dispose()
        {
            xmlStreamReader.Close();
            xmlStreamReader.Dispose();
            zipArchive.Dispose();
            mainStream.Close();
            mainStream.Dispose();
        }
    }
}
