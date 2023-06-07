using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace br.com.Bonus630DevToolsBar.RecentFiles
{
    public class RecentFilesViewModel : ViewModelBase
    {
    
        private ObservableCollection<RecentFileViewModel> files;

        public ObservableCollection<RecentFileViewModel> Files
        {
            get { return files; }
            set { files = value; OnPropertyChanged(); }
        }
        private int itemsWidth = 20;

        public int ItemsWidth
        {
            get { return itemsWidth * files.Count; }
            set { itemsWidth = value;OnPropertyChanged(); }
        }

        public int Count { get { return files.Count; } }

        public RecentFileViewModel this[int i] 
        { get { return files[i]; }set { Files[i] = value; OnPropertyChanged("Files"); } }
        public RecentFileViewModel this[string fullName]
        { 
            get { 
                return files.SingleOrDefault(r => r.Path.Equals(fullName, StringComparison.InvariantCultureIgnoreCase)); 
            }  }

        /// <summary>
        /// Retorna -1 se falhar
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>id</returns>
        public int GetId(string fullName) 
        {
            int id = -1;
            RecentFileViewModel rfm = this[fullName];
            if (rfm != null)
                id = rfm.ID;
            return id;
        }
        public RecentFileViewModel GetItem(int id)
        {
            return files.SingleOrDefault(r => r.ID == id);
        }

        public void Add(RecentFileViewModel file)
        {
            this.files.Add(file);
            OnPropertyChanged("Files");
            OnPropertyChanged("IteMsWidth");
        }
        public void Remove(RecentFileViewModel file)
        {
            this.files.RemoveAt(file.ID);
            OnPropertyChanged("Files");
            OnPropertyChanged("ItemsWidth");
        }
    }
}
