using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        private bool consoleCounter;
        public event Action CloseEvent;
        private SaveLoadConfig saveLoad;
        public bool ConsoleCounter
        {
            get { return consoleCounter; }
            set { consoleCounter = value;OnPropertyChanged(); }
        }
        private int xslTesterFontSize;
        public int XslTesterFontSize
        {
            get { xslTesterFontSize = Properties.Settings.Default.XslTesterFontSize; return xslTesterFontSize; }
            set { xslTesterFontSize = value;OnPropertyChanged(); }
        }
        private string section = "General";

        public string Section
        {
            get { return section ; }
            set { section  = value; OnPropertyChanged(); }
        }

        private bool autoOpenLastFile;

        public bool AutoOpenLastFile
        {
            get { return autoOpenLastFile; }
            set { autoOpenLastFile = value; OnPropertyChanged(); }
        }
        private string lastFilePath;
        public string LastFilePath
        {
            get {  return lastFilePath; }
            set { lastFilePath = value; OnPropertyChanged(); }
        }

        private string editorArguments;
        public string EditorArguments
        {
            get { return editorArguments; }
            set { editorArguments = value; OnPropertyChanged(); }
        }
        private string editor;
        public string Editor
        {
            get { return editor; }
            set { editor = value; OnPropertyChanged(); }
        }
        private SimpleCommand saveCommmand;
        private SimpleCommand closeCommand;
        
        public ConfigViewModel()
        {
            saveCommmand = new SimpleCommand(save);
            closeCommand =new  SimpleCommand(close);
            saveLoad = new SaveLoadConfig();
            load();
        }

        public SimpleCommand SaveCommand { get { return saveCommmand; } }
        public SimpleCommand CloseCommand { get { return closeCommand; } }
        private void save()
        {
            saveLoad.ConsoleCounter = consoleCounter;
            saveLoad.XslTesterFontSize = xslTesterFontSize;
            saveLoad.AutoOpenLastFile = autoOpenLastFile;
            saveLoad.LastFilePath = lastFilePath;
            saveLoad.Editor = editor;
            saveLoad.EditorArguments = editorArguments;
            saveLoad.Save();
            close();
        }
        private void load()
        {
            ConsoleCounter = saveLoad.ConsoleCounter;
            XslTesterFontSize = saveLoad.XslTesterFontSize;
            AutoOpenLastFile = saveLoad.AutoOpenLastFile;
            LastFilePath = saveLoad.LastFilePath;
            Editor = saveLoad.Editor;
            EditorArguments = saveLoad.EditorArguments;
        }
        private void close()
        {
            if (CloseEvent != null)
                CloseEvent();
        }

    }
}
