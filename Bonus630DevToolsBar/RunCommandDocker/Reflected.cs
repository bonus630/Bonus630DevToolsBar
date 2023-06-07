using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class Reflected : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if(PropertyChanged!=null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

         public Reflected Parent { get; set; }
        public bool IsIndexed { get; set; }
        public int Index { get; set; }

        private bool isValueType;

        public bool IsValueType
        {
            get { return isValueType; }
            set { isValueType = value;
                OnPropertyChanged("IsValueType");
            }
        }
        public bool Error { get; set; }

        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value;
                OnPropertyChanged("Value");
            }
        }
        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }


        private ObservableCollection<Reflected> childrens;

        public ObservableCollection<Reflected> Childrens
        {
            get { return childrens; }
            set
            {
                childrens = value;
                OnPropertyChanged("Childrens");
            }
        }
    
        public void Add(Reflected children)
        {
            if (childrens == null)
                childrens = new ObservableCollection<Reflected>();
            childrens.Add(children);
            OnPropertyChanged("Childrens");
        }

    }
}
