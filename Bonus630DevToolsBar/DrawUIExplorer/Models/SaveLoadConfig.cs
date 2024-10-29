using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class SaveLoadConfig
    {
        private bool consoleCounter;
        public bool ConsoleCounter { 
            get { consoleCounter = Properties.Settings.Default.ConsoleCounter; return consoleCounter; } 
            set{ consoleCounter = value; } }
        private int xslTesterFontSize;
        public int XslTesterFontSize
        {
            get { xslTesterFontSize = Properties.Settings.Default.XslTesterFontSize; return xslTesterFontSize; }
            set { xslTesterFontSize = value; }
        }
        private bool autoOpenLastFile;
        public bool AutoOpenLastFile
        {
            get { autoOpenLastFile = Properties.Settings.Default.AutoOpenLastFile; return autoOpenLastFile; }
            set { autoOpenLastFile = value; }
        }
        private string lastFilePath;
        public string LastFilePath
        {
            get { lastFilePath = Properties.Settings.Default.LastFilePath; return lastFilePath; }
            set { lastFilePath = value; }
        }
        private string editorArguments;
        public string EditorArguments
        {
            get { return editorArguments; }
            set { editorArguments = value;  }
        }
        private string editor;
        public string Editor
        {
            get { return editor; }
            set { editor = value; }
        }
        private bool closesIfHostClose = true;

        public bool ClosesIfHostClose
        {
            get { return closesIfHostClose; }
            set { closesIfHostClose = value;  }
        }
        public SaveLoadConfig()
        {
            load();
        }
        public void Save()
        {
            Properties.Settings.Default.ConsoleCounter = consoleCounter;
            Properties.Settings.Default.XslTesterFontSize = xslTesterFontSize;
            Properties.Settings.Default.LastFilePath = lastFilePath;
            Properties.Settings.Default.AutoOpenLastFile = autoOpenLastFile;
            Properties.Settings.Default.ClosesIfHostClose = closesIfHostClose;
            Properties.Settings.Default.Editor = editor;
            Properties.Settings.Default.EditorArguments = editorArguments;
            Properties.Settings.Default.Save();
        }
        private void load()
        {
            ConsoleCounter = Properties.Settings.Default.ConsoleCounter;
            XslTesterFontSize = Properties.Settings.Default.XslTesterFontSize;
            AutoOpenLastFile = Properties.Settings.Default.AutoOpenLastFile;
            ClosesIfHostClose = Properties.Settings.Default.ClosesIfHostClose;
            LastFilePath = Properties.Settings.Default.LastFilePath;
            Editor = Properties.Settings.Default.Editor;
            EditorArguments = Properties.Settings.Default.EditorArguments;
        }
    }
}
