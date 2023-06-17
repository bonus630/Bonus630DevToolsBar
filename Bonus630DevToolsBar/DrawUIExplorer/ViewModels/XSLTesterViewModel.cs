using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    class XSLTesterViewModel : ViewModelDataBase
    {
        XmlDocument xmlDoc;
        XslCompiledTransform xslCompiledTransform;
        SaveLoadConfig saveLoadConfig;
        string path;
        public string xslFile;
        string resultFile;
        public string xmlfile;
        private int fontSize = 9;
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value; OnPropertyChanged();
                if (saveLoadConfig != null)
                {
                    saveLoadConfig.XslTesterFontSize = fontSize;
                    saveLoadConfig.Save();
                }
            }
        }
        private int controlWidth;
        public int ControlWidth
        {
            get { return controlWidth; }
            set
            {
                controlWidth = value; OnPropertyChanged();
            }
        }
        private int controlHeight;
        public int ControlHeight
        {
            get { return controlHeight; }
            set
            {
                controlHeight = value; OnPropertyChanged();
            }
        }
        private string xmlText;
        public string XmlText
        {
            get { return xmlText; }
            set { xmlText = value; OnPropertyChanged(); }
        }
        private string xslText;
        public string XslText
        {
            get { return xslText; }
            set { xslText = value; OnPropertyChanged(); }
        }
        private string resultText;
        public string ResultText
        {
            get { return resultText; }
            set { resultText = value; OnPropertyChanged(); }
        }
        public SimpleCommand GenXmlCommand { get { return new SimpleCommand(GenXmlText); } }
        public SimpleCommand ProcessCommand { get { return new SimpleCommand(process); } }


        public SimpleCommand DecreaseFontSizeCommand { get { return new SimpleCommand(decreaseFontSize); } }
        public SimpleCommand IncreaseFontSizeCommand { get { return new SimpleCommand(increaseFontSize); } }
        public SimpleCommand XmlContainerHideShowCommand { get { return new SimpleCommand(hideShowXml); } }
        public SimpleCommand XsltContainerHideShowCommand { get { return new SimpleCommand(hideShowXslt); } }


        public XSLTesterViewModel(Core core) : base(core)
        {
            xmlDoc = new XmlDocument();
            xslCompiledTransform = new XslCompiledTransform(true);
            path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DrawUIExplorer");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            xslFile = path + "\\temp.xslt";
            resultFile = path + "\\result.txt";
            xmlfile = path + "\\temp.xml";
            saveLoadConfig = new SaveLoadConfig();
            FontSize = saveLoadConfig.XslTesterFontSize;
        }
        protected override void Update(IBasicData basicData)
        {
            CurrentBasicData = basicData;
        }

        //int level = 0;
        private void GenXmlText()
        {
            XmlText = core.GetXml(this.CurrentBasicData);
        }
        private void process()
        {
            try
            {
                string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DrawUIExplorer");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string xslFile = path + "\\temp.xslt";
                string resultFile = path + "\\result.txt";
                string xmlfile = path + "\\temp.xml";
                File.WriteAllText(xslFile, XslText);
                File.WriteAllText(xmlfile, XmlText);
                xslCompiledTransform.Load(xslFile);
                xslCompiledTransform.Transform(xmlfile, resultFile);
                ResultText = File.ReadAllText(resultFile);
                this.core.DispactchNewMessage("Xsl Transform Sucess, the result is in Result Box", MsgType.Console);
            }
            catch (XmlException erro) { this.core.DispactchNewMessage(erro.Message, MsgType.Console); }
            catch (XsltException erro) { this.core.DispactchNewMessage(erro.Message, MsgType.Console); }
            catch (Exception erro) { this.core.DispactchNewMessage(erro.Message, MsgType.Console); }
        }
        private XmlReader CreateXmlReader(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            //text = text.Replace(@"\", "");
            BufferedStream stream = new BufferedStream(new MemoryStream());
            stream.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(stream);
            XmlReader reader = XmlReader.Create(sr);

            stream.Close();
            return reader;
        }
        private void increaseFontSize()
        {
            if (this.FontSize < 30)
                this.FontSize++;
        }
        private void decreaseFontSize()
        {
            if (this.FontSize > 6)
                this.FontSize--;
        }
        private void hideShowXml()
        {

            XmlContainerVisible = !XmlContainerVisible;
            ChangeColumnWidth();
        }
        private void hideShowXslt()
        {

            XsltContainerVisible = !XsltContainerVisible;
            ChangeColumnWidth();
        }
        private void ChangeColumnWidth()
        {
           
          
            if (!XsltContainerVisible)
                XsltColumnWidth = "0";
            if (!XmlContainerVisible)
                XmlColumnWidth = "0";
            if (XsltContainerVisible)
                XsltColumnWidth = "Auto";
            if (XmlContainerVisible)
                XmlColumnWidth = "Auto";
            if (XsltContainerVisible && XmlContainerVisible)
            {
                XmlColumnWidth = "*";
                XsltColumnWidth = "*";
            }
        }  
        private bool xmlContainerVisible = true;

        public bool XmlContainerVisible
        {
            get { return xmlContainerVisible; }
            set { xmlContainerVisible = value; OnPropertyChanged(); }
        }
        private bool xsltContainerVisible = true;

        public bool XsltContainerVisible
        {
            get { return xsltContainerVisible; }
            set { xsltContainerVisible = value; OnPropertyChanged(); }
        }
        private string xmlColumnWidth = "*";

        public string XmlColumnWidth
        {
            get { return xmlColumnWidth; }
            set { xmlColumnWidth = value; OnPropertyChanged(); }
        }
        private string xsltColumnWidth = "*";

        public string XsltColumnWidth
        {
            get { return xsltColumnWidth; }
            set { xsltColumnWidth = value; OnPropertyChanged(); }
        }

    }
}
