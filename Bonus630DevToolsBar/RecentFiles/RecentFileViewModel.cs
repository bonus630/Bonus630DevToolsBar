﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;

namespace br.com.Bonus630DevToolsBar.RecentFiles
{
    public class RecentFileViewModel : ViewModelBase

    {

        public RecentFileViewModel(int id)
        {
            this.ID = id;
        }

        private int id;

        public int ID
        {
            get { return id; }
            private set { id = value; OnPropertyChanged(); }
        }
        private int fileIndex;

        public int Index
        {
            get { return fileIndex; }
            set { fileIndex = value; OnPropertyChanged(); }
        }
        private string fileName;

        public string Name
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged(); }
        }
        private string filePath;

        public string FullName
        {
            get { return filePath; }
            set { filePath = value; OnPropertyChanged(); }
        }
        private string absName;

        public string AbsName
        {
            get { return absName; }
            private set { absName = value; OnPropertyChanged(); }
        }

        private BitmapSource fileThumb;

        public BitmapSource Thumb
        {
            get { return fileThumb; }
            set { fileThumb = value; OnPropertyChanged(); }
        }

        private int fileOpenTimes;

        public int OpenTimes
        {
            get { return fileOpenTimes; }
            set { fileOpenTimes = value; OnPropertyChanged(); }
        }
        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; OnPropertyChanged(); }
        }
        private long fileOpenedTime;

        public long OpenedTime
        {
            get { return fileOpenedTime; }
            set { fileOpenedTime = value; OnPropertyChanged(); }
        }
        private DateTime fileLastOpenDate;

        public DateTime OpenDate
        {
            get { return fileLastOpenDate; }
            set { fileLastOpenDate = value; OnPropertyChanged(); }
        }
        private bool autoLoad;

        public bool AutoLoad
        {
            get { return autoLoad; }
            set { autoLoad = value; OnPropertyChanged(); }
        }
        private bool isOpened = false;

        public bool IsOpened
        {
            get { return isOpened; }
            set { isOpened = value; OnPropertyChanged(); }
        }
        private bool isPinned = false;

        public bool IsPinned
        {
            get { return isPinned; }
            set { isPinned = value; OnPropertyChanged(); }
        }
        private int pinned = -1;

        public int Pinned
        {
            get { return pinned; }
            set { pinned = value; OnPropertyChanged(); }
        }
        public void SetAbsName()
        {
            //index = 0,name = -1,thumb = 1
            if (Properties.Settings.Default.UseIndex == 0)
                this.AbsName = this.Index.ToString();
            if (Properties.Settings.Default.UseIndex == -1)
                this.AbsName = this.Name.Substring(0, 2);
            if (Properties.Settings.Default.UseIndex == 1)
                this.AbsName = "";
        }

    }
}
