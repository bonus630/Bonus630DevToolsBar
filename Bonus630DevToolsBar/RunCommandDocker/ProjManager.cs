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
    public class ProjManager : XMLManager
    {

        public ProjManager() { }
        /// <summary>
        /// Call OpenFile
        /// </summary>
        /// <param name="projFilePath"></param>
        public ProjManager(string projFilePath) : base(projFilePath)
        {
            OpenFile(projFilePath);
        }

        public bool ChangeAssembliesPathInProj(string assembliesPath)
        {
            try
            {
                var element = xmlDoc.DocumentElement;
                var na = new XmlNamespaceManager(xmlDoc.NameTable);
                na.AddNamespace("p", "http://schemas.microsoft.com/developer/msbuild/2003");
                XmlNodeList list = element.SelectNodes("p:Target[@Name=\"CopyFiles\"]", na);
                if (list.Count > 0)
                {
                    list[0].ChildNodes[1].Attributes["Directories"].Value = assembliesPath;
                    list[0].ChildNodes[2].Attributes["Condition"].Value = string.Format("Exists('{0}')", assembliesPath);
                    list[0].ChildNodes[2].Attributes["Command"].Value = string.Format("xcopy \"$(TargetDir)$(TargetFileName)\" \"{0}\" /y /d /e /c", assembliesPath);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public void AddCopyFilesCommand(string AssembliesFolder)
        {
            XmlNode rootNode = xmlDoc.DocumentElement;
            XmlElement targetNode = CreateElement("Target", "", new Dictionary<string, string>() { { "Name", "CopyFiles" }, { "AfterTargets", "Build" } });
            XmlElement msgNode = CreateElement("Message", "", new Dictionary<string, string> { { "Text", "CopyFiles" } });
            XmlElement makeDirNode = CreateElement("MakeDir", "", new Dictionary<string, string> { { "Directories", AssembliesFolder } });
            XmlElement execNode = CreateElement("Exec", "", new Dictionary<string, string>() { {"Condition", string.Format("Exists('{0}')", AssembliesFolder)},
                {"Command", string.Format("xcopy \"$(TargetDir)$(TargetFileName)\" \"{0}\" /y /d /e /c", AssembliesFolder)} });
            targetNode.AppendChild(makeDirNode);
            targetNode.AppendChild(msgNode);
            targetNode.AppendChild(execNode);
            xmlDoc.LastChild.AppendChild(targetNode);
        }
        public void ChangeReference(string referenceName, string referenceValue)
        {
            XmlNode rootNode = xmlDoc.DocumentElement;
            var references = rootNode.SelectNodes("//p:ItemGroup//p:Reference", namespaceManager);
            foreach (XmlNode referenceNode in references)
            {
                var att = referenceNode.Attributes["Include"];
                if (att.Value.Contains(referenceName))
                {
                    att.Value = referenceValue;
                    XmlElement el = xmlDoc.CreateElement("Name", rootNode.NamespaceURI);
                    el.InnerText = referenceName;
                    referenceNode.AppendChild(el);
                    break;
                }
            }
        }
        public string GetProjNameSpace()
        {
            return this.GetTagText("//p:PropertyGroup//p:RootNamespace");
        }
        public string GetAssemblyName()
        {
            return this.GetTagText("//p:PropertyGroup//p:AssemblyName");
        }
        public bool AddCompileItem(string itemName)
        {
            XmlNode rootNode = xmlDoc.DocumentElement;
            var references = rootNode.SelectNodes("//p:ItemGroup//p:Compile", namespaceManager);
            foreach (XmlNode referenceNode in references)
            {
                if (referenceNode.Attributes["Include"].Value.Equals(itemName))
                    return false;
            }
            XmlElement item = CreateElement("Compile", "", new Dictionary<string, string>() { { "Include", itemName } });
            references[0].ParentNode.AppendChild(item);
            return true;
        }
        public bool RemoveCompileItem(string itemName)
        {
            XmlNode rootNode = xmlDoc.DocumentElement;
            var references = rootNode.SelectNodes("//p:ItemGroup//p:Compile", namespaceManager);
            foreach (XmlNode referenceNode in references)
            {
                if (referenceNode.Attributes["Include"].Value.Equals(itemName))
                {
                    referenceNode.ParentNode.RemoveChild(referenceNode);
                    return true;
                }
            }
            return false;
        }

    }
}
