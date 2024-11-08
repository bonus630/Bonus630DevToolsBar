using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public abstract class XMLManager : IDisposable
    {

        protected XmlDocument xmlDoc;
        protected string projFilePath;
        protected string rootNamespace;
        protected XmlNamespaceManager namespaceManager;
        public XMLManager() { }
        /// <summary>
        /// Call OpenFile
        /// </summary>
        /// <param name="projFilePath"></param>
        public XMLManager(string projFilePath)
        {
            OpenFile(projFilePath);
        }


        protected XmlElement CreateElement(string name, string value = "", Dictionary<string,string> attributes = null)
        {
            XmlElement element = xmlDoc.CreateElement(name,this.rootNamespace);
            if (!string.IsNullOrEmpty(value))
                element.Value = value;
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    element.SetAttribute(attribute.Key, attribute.Value);
                }
            }
            return element;
        }

        protected string GetTagValue(string XPath,int index = 0)
        {
            XmlNode rootNode = xmlDoc.DocumentElement;
            var reference = rootNode.SelectNodes(XPath, namespaceManager);
            if (reference.Count > 0)
                return reference[index].Value;
            else
                return string.Empty;
        }
        protected string GetTagText(string XPath, int index = 0)
        {
            XmlNode rootNode = xmlDoc.DocumentElement;
            var reference = rootNode.SelectNodes(XPath, namespaceManager);
            if (reference.Count > 0)
                return reference[index].InnerText;
            else
                return string.Empty;
        }
        public void OpenFile(string projFilePath)
        {
            if (!File.Exists(projFilePath))
                throw new IOException("File not exists");
            this.projFilePath = projFilePath;
            xmlDoc = new XmlDocument();
            xmlDoc.Load(projFilePath);
            this.rootNamespace = xmlDoc.DocumentElement.NamespaceURI;
            namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("p", "http://schemas.microsoft.com/developer/msbuild/2003");
        }
        /// <summary>
        /// Save proj file
        /// </summary>
        public void CloseFile()
        {
            this.Dispose();
        }


        public void Dispose()
        {
            if (xmlDoc != null)
                xmlDoc.Save(this.projFilePath);
        }

    }
}
