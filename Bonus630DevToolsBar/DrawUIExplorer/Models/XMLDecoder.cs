using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class XMLDecoder
    {
        private string xmlString;
        private XmlDocument xmlDocument;
        private bool isBusy = false;
        public bool IsBusy { get { return isBusy; } protected set { isBusy = value; } }
        public string XmlString { get { return this.xmlString; } set { this.xmlString = value; } }

        public event Action LoadFinish;
        private string xPath;
        public IBasicData FirstItens;

        public List<string> FilePath { get; set; }

        public void Process(string filePath)
        {
            isBusy = true;
            if (FilePath == null)
                FilePath = new List<string>();
            FilePath.Add(filePath);
            xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(this.xmlString);
            }
            catch (XmlException erro)
            {
                throw erro;
            }
            IBasicData rootNodeData = new OtherData();
            rootNodeData.SetXmlChildreParentID(-1);
            rootNodeData.SetXmlChildreID(0);
            rootNodeData.TagName = "uiConfig";
            FirstItens = rootNodeData;
            loadXmlNodes(rootNodeData, xmlDocument.ChildNodes.Item(1));
            if (LoadFinish != null)
            {
                isBusy = false;
                LoadFinish();
            }
        }
        public void ExtractTagsToNewXml(string filePath, string xmlSavePath, List<string> listTags)
        {
            bool anyTagNotFound = true;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (!File.Exists(xmlSavePath))
                {
                    using (FileStream fsCreate = File.Create(xmlSavePath)) { }
                }
                using (FileStream xmlStream = new FileStream(xmlSavePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (XmlReader reader = XmlReader.Create(fs))
                    {

                        byte[] xmlHeader = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
                        xmlStream.Write(xmlHeader, 0, xmlHeader.Length);

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && listTags.Contains(reader.Name))
                            {
                                anyTagNotFound = false;
                                string t = reader.ReadOuterXml();
                                byte[] buffer = Encoding.UTF8.GetBytes(t);
                                xmlStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
            }
            if(anyTagNotFound)
                throw new Exception("no tags found");
        }
        public void MergeXmlFilesForTags(List<string> tagNames, List<string> filePaths, string outputFilePath)
        {
            XDocument combinedDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes")
                
            );

            foreach (string tagName in tagNames)
            {
                XElement combinedTagElement = new XElement(tagName);

                foreach (string filePath in filePaths)
                {
                    XDocument doc = XDocument.Load(filePath);

                    XElement tagElement = doc.Element(tagName);

                    if (tagElement != null)
                    {
                        combinedTagElement.Add(tagElement.Elements());
                    }
                }

                combinedDoc.Add(combinedTagElement);
            }

            combinedDoc.Save(outputFilePath);
        }
        //private int count = 0;
        private void loadXmlNodes(IBasicData parentBasicData, XmlNode xmlNode)
        {
            // este simbolo faz ocorrer um erro no xml:  --&gt;
            //Debug.WriteLine(count++);
            int index = 1;
            XmlNodeList FirstsNodes = xmlNode.ChildNodes;
            IBasicData tempBasicData = null;
            foreach (XmlNode item in FirstsNodes)
            {
                string tagName = item.Name;
                // Debug.WriteLine(tagName);

                switch (tagName)
                {
                    case "userData":
                        tempBasicData = new UserData();
                        break;
                    case "commandBarData":
                        tempBasicData = new CommandBarData();
                        break;
                    case "state":
                        tempBasicData = new StateData();
                        break;
                    case "itemData":
                        tempBasicData = new ItemData();
                        break;
                    case "dialog":
                        tempBasicData = new DialogData();
                        break;
                    case "viewTemplate":
                        tempBasicData = new ViewTemplate();
                        break;
                    case "dockerData":
                        tempBasicData = new DockerData();
                        break;
                    case "container":
                        tempBasicData = new ContainerData();
                        break;
                    case "frame":
                        tempBasicData = new FrameData();
                        break;
                    case "key":
                        tempBasicData = new KeyData();
                        break;
                    case "table":
                        tempBasicData = new TableData();
                        break;
                    default:
                        tempBasicData = new OtherData();
                        break;
                }

                if (tempBasicData != null && parentBasicData != null)
                {
                    tempBasicData.TagName = tagName;
                    tempBasicData.SetXmlChildreID(index);
                    tempBasicData.SetXmlChildreParentID(parentBasicData.XmlChildrenID);
                    tempBasicData.Parent = parentBasicData;
                    index++;
                    try
                    {
                        if (item.Attributes != null)
                        {
                            foreach (XmlAttribute att in item.Attributes)
                            {
                                if (att.Name == "guid")
                                {
                                    tempBasicData.Guid = att.Value;
                                    //break;
                                }
                                if (att.Name == "guidRef")
                                {
                                    tempBasicData.GuidRef = att.Value;
                                    //break;
                                }
                                tempBasicData.Attributes.Add(new DataClass.Attribute(att.Name, att.Value));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Attribute load failed in tag \"" + tagName + "\"");
                    }
                    tempBasicData.SetTreeLevel(parentBasicData.TreeLevel);
                    parentBasicData.Add(tempBasicData);
                    if (item.ChildNodes.Count > 0)
                        loadXmlNodes(tempBasicData, item);
                    else
                        tempBasicData.Text = item.Value;
                }

            }

        }
        private void GetXPath(IBasicData basicData, bool intern)
        {

            if (basicData.XmlChildrenID > 0 && !basicData.IAmUniqueTag())
            {
                if (!string.IsNullOrEmpty(basicData.Guid))
                    xPath = string.Format("/{0}[@guid='{1}']{2}", basicData.TagName, basicData.Guid, xPath);
                else
                    xPath = string.Format("/{0}[{1}]{2}", basicData.TagName, basicData.XmlChildrenID, xPath);
            }
            else
                xPath = string.Format("/{0}{1}", basicData.TagName, xPath);
            if (basicData.Parent != null)
            {
                ;
                GetXPath(basicData.Parent, true);
            }

        }
        public string GetXPath(IBasicData basicData)
        {
            this.xPath = "";
            GetXPath(basicData, true);
            return this.xPath;
        }
        public string GetXml(IBasicData basicData, bool xmlHeader = true)
        {
            GetXPath(basicData);
            string text = "";
            XmlDocument temp = new XmlDocument();
            temp.LoadXml(xmlDocument.DocumentElement.SelectNodes(xPath).Item(0).OuterXml);
            XmlAttributeCollection xmlAttributeCollection = temp.FirstChild.Attributes;
            for (int i = 0; i < xmlAttributeCollection.Count; i++)
            {
                if (xmlAttributeCollection[i].Name == "encoding")
                {
                    temp.FirstChild.Attributes.Remove(xmlAttributeCollection[i]);
                    break;
                }
            }
            //temp.DocumentElement.Attributes.Remove(temp.DocumentElement.GetAttribute("encoding").);
            try
            {
                text = Beautify(temp);
                if (!xmlHeader)
                    text = text.Remove(0, 22);
            }
            catch (XmlException e)
            {
                throw new Exception("Many root elements!");
            }
            return text;
        }
        public string Beautify(string xmlString)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);
            return Beautify(xmlDocument);
        }
        private string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = Encoding.GetEncoding("ISO-8859-1")
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString().Replace(" encoding=\"utf-16\"", " ");
        }

        internal bool IsXMLString(string text)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicData"></param>
        /// <returns>Return -1 if fails!</returns>
        internal int GetLineNumber(IBasicData basicData)
        {
            try
            {
                string path = this.GetXPath(basicData);
                XPathDocument xpDco = new XPathDocument(FilePath[0]);
                XPathNavigator navigator = xpDco.CreateNavigator();
                var element = navigator.SelectSingleNode(path);
                return (IXmlLineInfo)element != null ? ((IXmlLineInfo)element).LineNumber : -1;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }
    }

}
