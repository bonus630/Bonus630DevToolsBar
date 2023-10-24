using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public abstract class BasicData<T> : ViewModelBase, IComparable, IBasicData
    {
        public string Guid { get; set; }
        public string GuidRef { get; set; }
        public virtual string TagName { get; set; }
        public int PathIndex { get; private set; }

        public string TagValue { get { if (Childrens == null || Childrens.Count == 0) return TagName; else return string.Format("{0} [{1}]", TagName, Childrens.Count); } }
        public string Caption { get; set; }
        private Type type;
        private int xmlChildrenID;
        private int xmlChildrenParentID;
        private int treeLevel = 0;
        private bool isSelected = false;
        private bool isSpecialType = false;
        private string text;
        protected bool isContainer = false;
        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged(); }
        }
        //Teste getresource
        private string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; OnPropertyChanged(); }
        }
        public int XmlChildrenID { get { return this.xmlChildrenID; } }
        public int XmlChildrenParentID { get { return this.xmlChildrenParentID; } }
        public int TreeLevel { get { return this.treeLevel; } }

        private List<Attribute> attributes;
        public List<Attribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; OnPropertyChanged(); }
        }
        private ObservableCollection<IBasicData> childrens;
        public ObservableCollection<IBasicData> Childrens
        {
            get
            {
                return childrens;
            }
            set { childrens = value; OnPropertyChanged(); }
        }
        public bool IsSelected { get { return this.isSelected; } }
        public bool IsContainer { get { return this.isContainer; } }
        public bool IsSpecialType { get { return this.isSpecialType; } }
        public IBasicData Parent { get; set; }

        public event Action<bool, bool?, bool> SelectedEvent;


        public BasicData()
        {
            this.type = typeof(T);
            if (childrens == null)
                childrens = new ObservableCollection<IBasicData>();
            Attributes = new List<Attribute>();
            setSpecialType();
        }
        /// <summary>
        /// Está função procura um pai de um certo tipo ou no range do treeLevel
        /// </summary>
        /// <typeparam name="T">Tipo do parente para encontrar</typeparam>
        /// <param name="treeLevel">Passe um valor para encontrar nesse range</param>
        /// <returns>Retorna nulo se nenhum parente do tipo ou no range for encontrado</returns>
        public IBasicData GetParentByType<T>(int treeLevel = - 1)
        {
            bool searching = true;
            IBasicData parent = this.Parent;
            while(searching)
            {

                if (parent == null)
                {
                    searching = false;
                    break;
                }
                if (parent.GetType().Equals(typeof(T)) && treeLevel == -1)
                {
                    searching = false;
                    break;
                }
                if(treeLevel > -1 && treeLevel == parent.TreeLevel)
                {
                    searching = false;
                    break;
                }
               parent = parent.Parent;
            }
            return parent;
        }
        private void setSpecialType()
        {
            if (type == typeof(DockerData) || type == typeof(CommandBarData) || type == typeof(DialogData))
                this.isSpecialType = true;


        }

        public void SetTreeLevel(int parentLevel)
        {
            this.treeLevel = parentLevel + 1;
        }
        public void SetPathIndex(int index)
        {
            PathIndex = index;
        }
        public int CompareTo(object obj)
        {
            BasicData<T> basicData = (obj as BasicData<T>);
            if (obj == null)
                return -1;
            if (basicData.Guid == this.Guid)
                return 0;
            return basicData.Guid.CompareTo(basicData.Guid);


        }
        public Type GetType(IBasicData basicData)
        {
            return typeof(T);
        }

        public bool ContainsAttribute(string AttributeName)
        {
            if (this.Attributes == null)
                return false;
            foreach (Attribute item in Attributes)
            {
                if (item.Name == AttributeName)
                    return true;
            }
            return false;
        }

        public bool ContainsAttributeValue(string AttributeValue)
        {
            if (this.Attributes == null)
                return false;
            foreach (Attribute item in Attributes)
            {
                if (item.Value == AttributeValue)
                    return true;
            }
            return false;
        }
        public string GetAttribute(string AttributeName)
        {
            if (this.Attributes == null)
                return "";
            foreach (Attribute item in Attributes)
            {
                if (item.Name == AttributeName)
                    return item.Value;
            }
            return "";
        }
        public bool ContainsAttributeValuePartial(string AttributeValue)
        {
            if (this.Attributes == null)
                return false;
            foreach (Attribute item in Attributes)
            {
                if (item.Value.ToLower().Contains(AttributeValue.ToLower()))
                    return true;
            }
            return false;
        }
        public string GetAnyGuidAttribute()
        {
            if (this.Attributes == null)
                return null;
            if (!string.IsNullOrEmpty(this.Guid))
                return this.Guid;
            if (!string.IsNullOrEmpty(this.GuidRef))
                return this.GuidRef;
            foreach (Attribute item in Attributes)
            {
                if (item.IsGuid)
                    return item.Value;
            }
            return null;
        }
        public void SetXmlChildreID(int id)
        {
            this.xmlChildrenID = id;
        }
        public void SetXmlChildreParentID(int id = -1)
        {
            this.xmlChildrenParentID = id;
        }
        public void Add(IBasicData basicData)
        {
            if (childrens == null)
                childrens = new ObservableCollection<IBasicData>();
            this.childrens.Add(basicData);
            OnPropertyChanged("Childrens");
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            IBasicData basicData = obj as IBasicData;
            if (basicData == null)
                return false;
           
            //This comparison can't still work in merge case
            if (this.TagName == basicData.TagName && this.Guid == basicData.Guid && this.XmlChildrenID == basicData.XmlChildrenID )
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -1730927587;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Guid);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GuidRef);
            return hashCode;
        }

        public void SetSelected(bool isSelected, bool? isExpands, bool update, bool recursive = false)
        {
           
            this.isSelected = isSelected;

            
            if (SelectedEvent != null)
            {
                SelectedEvent(this.isSelected, isExpands, update);
            }
            OnPropertyChanged("IsSelected");
            if (recursive && this.Parent != null)
                this.Parent.SetSelected(isSelected, isExpands, update, recursive);
        }

        public bool IAmUniqueTag()
        {
            for (int i = 0; i < this.Parent.Childrens.Count; i++)
            {
                if (this != this.Parent.Childrens[i] && this.Parent.Childrens[i].TagName == this.TagName)
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.TagName, this.XmlChildrenID);
        }

        //public IEnumerator<T> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}
    }


}
