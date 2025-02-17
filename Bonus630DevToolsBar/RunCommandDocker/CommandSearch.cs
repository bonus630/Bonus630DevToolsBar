using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class CommandSearch : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Command> commandsSearchCache = new List<Command>();
        public ObservableCollection<Project> projects;

        private int currentIndex = 0;

        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                currentIndex = value;
                OnPropertyChanged("CurrentIndex");
            }
        }

        private int foundedsCount;

        public int FoundedsCount
        {
            get { return foundedsCount; }
            set
            {
                foundedsCount = value;
                OnPropertyChanged("FoundedsCount");
            }
        }

        private string currentAndFounds;

        public string CurrentAndFounds
        {
            get { return currentAndFounds; }
            set { currentAndFounds = value;
                OnPropertyChanged("CurrentAndFounds");
            }
        }


        public string searchTerm;
        internal int termPosition;
        internal bool wholeWord;
        internal bool matchCase;

        //public string SearchTerm
        //{
        //    get { return searchTerm; }
        //    set { searchTerm = value; }
        //}

        private void changeCurrentAndFounds(int currentIndex, int foundedsCount)
        {
            this.currentIndex = currentIndex;
            if (this.currentIndex > foundedsCount - 1)
                this.currentIndex = 0;
            if (this.currentIndex < 0)
                this.currentIndex = this.foundedsCount - 1;
            this.foundedsCount = foundedsCount;
            if(this.foundedsCount ==0)
                CurrentAndFounds = string.Format("{0}/{1}", 0, this.foundedsCount);
            else
                CurrentAndFounds =  string.Format("{0}/{1}", this.currentIndex+1, this.foundedsCount);

        }
   
        private IEnumerable<Command> FindItem(Module item,string searchTerm)
        {
            if (matchCase)
            {
                if(wholeWord)
                    return item.Items.Where(r => r.Name.Equals(searchTerm));
                else
                {
                    switch(termPosition)
                    {
                        case 0:
                            return item.Items.Where(r => r.Name.StartsWith(searchTerm));
                        case 1:
                            return item.Items.Where(r => r.Name.Contains(searchTerm));
                        default:
                            return item.Items.Where(r => r.Name.EndsWith(searchTerm));
                    }
                }
            }
            else
            {
                searchTerm = searchTerm.ToLower();
                if (wholeWord)
                    return item.Items.Where(r => r.Name.ToLower().Equals(searchTerm));
                else
                {
                    switch (termPosition)
                    {
                        case 0:
                            return item.Items.Where(r => r.Name.ToLower().StartsWith(searchTerm));
                        case 1:
                            return item.Items.Where(r => r.Name.ToLower().Contains(searchTerm));
                        default:
                            return item.Items.Where(r => r.Name.ToLower().EndsWith(searchTerm));
                    }
                }
            }
        }
        public void Search(string searchTerm,bool restart = false)
        {
            if(searchTerm != this.searchTerm || restart)
            {
                this.searchTerm = searchTerm;
                commandsSearchCache.Clear();
                for (int i = 0; i < projects.Count; i++)
                {
                    for (int j = 0; j < projects[i].Count; j++)
                    {
                        var items = FindItem(projects[i][j], searchTerm);
                        commandsSearchCache.AddRange(items);
                    }
                }
                if(restart)
                    currentIndex = 0;
                changeCurrentAndFounds(currentIndex, commandsSearchCache.Count) ;
                if (commandsSearchCache.Count > 0)
                    activeItem();

            }
         
        }
        public void Search(string searchTerm,Module module)
        {
            if (searchTerm != this.searchTerm )
            {
                this.searchTerm = searchTerm;
                commandsSearchCache.Clear();
                for (int i = 0; i < projects.Count; i++)
                {
                        var items = FindItem(module, searchTerm);
                        commandsSearchCache.AddRange(items);
                }
                currentIndex = 0;
                changeCurrentAndFounds(currentIndex, commandsSearchCache.Count);
                if (commandsSearchCache.Count > 0)
                    activeItem();

            }

        }
        public void Navegate(int next)
        {
            if (this.foundedsCount > 0)
            {
                changeCurrentAndFounds(currentIndex + next, this.foundedsCount);
                activeItem();
            }
        }
        private void activeItem()
        {
            Command item = commandsSearchCache[currentIndex];
            item.Parent.Parent.IsExpanded = true;
            item.Parent.IsExpanded = true;
            item.IsSelectedBase = true;
           
            
        }

    }
}
