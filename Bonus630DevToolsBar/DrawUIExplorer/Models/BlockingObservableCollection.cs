using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class BlockingObservableCollection<T> : BlockingCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public new  void Add(T item)
        {
            base.Add(item);
            OnNotifyPropertyChanged("Item[]");
            OnNotifyPropertyChanged("Count");
            OnCollectionChanged();
        }

        public void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void OnCollectionChanged()
        {
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
              
                CollectionChanged(this, args);
            }
        }
    }
}
