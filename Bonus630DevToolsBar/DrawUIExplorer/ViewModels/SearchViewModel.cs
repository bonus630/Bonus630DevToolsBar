using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels
{
    public class SearchViewModel : ViewModelDataBase
    {
        private SearchEngine searchEngine;
       

        private ObservableCollection<SearchAdvancedParamsViewModel> advancedSearchListAction = new ObservableCollection<SearchAdvancedParamsViewModel>();
        public ObservableCollection<SearchAdvancedParamsViewModel> AdvancedSearchListAction { get { return advancedSearchListAction; } set { this.advancedSearchListAction = value;OnPropertyChanged();  } }

        private ObservableCollection<object> tags = new ObservableCollection<object>();

        public ObservableCollection<object> Tags
        {
            get { return tags; }
            set {
             
                OnPropertyChanged(); }
        }
        private ObservableCollection<object> attributesName = new ObservableCollection<object>();

        public ObservableCollection<object> AttributesName
        {
            get { return attributesName; }
            set {
                //if (attributesName != null)
                //    attributesName.Clear();
                //attributesName = value;
                //if (attributesName.Count > 0)
                //    AttributeName = attributesName[0].ToString(); 
                OnPropertyChanged(); }
        }
        private ObservableCollection<object> attributesValue = new ObservableCollection<object>();

        public ObservableCollection<object> AttributesValue
        {
            get { return attributesValue; }
            set {
                //if (attributesValue != null)
                //    attributesValue.Clear();
                //attributesValue = value;
                //if (attributesValue.Count > 0)
                //    AttributeValue = attributesValue[0].ToString();
                OnPropertyChanged();
            }
        }

        public RoutedCommand<IBasicData> Search { get {return new RoutedCommand<IBasicData>(search); } }
        public RoutedCommand<string> AddParam { get { return new RoutedCommand<string>(addParam); } }
        public RoutedCommand<string> GetParam { get { return new RoutedCommand<string>(getParam); } }
        public RoutedCommand<bool> SetLocalData { get { return new RoutedCommand<bool>(setLocalData); } }
        public RoutedCommand<bool> SetGlobalData { get { return new RoutedCommand<bool>(setGlocalData); } }
        public SimpleCommand CopyGuid { get { return new SimpleCommand(menuItemCopyGuid); } }
        public SimpleCommand PastGuid { get { return new SimpleCommand(menuItemPastGuid); } }
        public SimpleCommand ClearSearchs { get { return new SimpleCommand(clearSearchs); } }
        public RoutedCommand<bool> SetAttributeTag { get { return new RoutedCommand<bool>(setAttributeTag); } }

        public SimpleCommand PastGuid2 { get { return new SimpleCommand(menuItemPastGuid2); } }
        public SimpleCommand Copy { get { return new SimpleCommand(menuItemCopy); } }
        public SimpleCommand Past { get { return new SimpleCommand(menuItemPast); } }
        public SimpleCommand Cut { get { return new SimpleCommand(menuItemCut); } }
        public SimpleCommand Clear { get { return new SimpleCommand(menuItemClear); } }

        protected IBasicData sBasicData;
        public IBasicData SearchBasicData
        {
            get { return sBasicData; }
            set { sBasicData = value; OnPropertyChanged(); }
        }
        public SearchViewModel(Core core): base(core)
        {
            this.searchEngine = core.SearchEngineGet;
            searchEngine.GenericSearchResultEvent += SearchEngine_GenericSearchResultEvent;
        }
        private void SearchEngine_GenericSearchResultEvent(List<object> obj)
        {
            AdvancedSearchListAction.Clear();
            for (int i = 0; i < obj.Count; i++)
            {
                AdvancedSearchListAction.Add(obj[i] as SearchAdvancedParamsViewModel);
            }
            
        }

        private void search(IBasicData sBasicData)
        {
            if (sBasicData != null)
            {
                for (int i = 0; i < this.AdvancedSearchListAction.Count; i++)
                {
                    (this.AdvancedSearchListAction[i] as SearchAdvancedParamsViewModel).SearchBasicData = sBasicData;
                }
                //searchEngine.NewSearch();
                ////searchEngine.SearchItemFromGuidRef(currentBasicData.Childrens, txt_guid.Text);

                //searchEngine.SearchAllAttributes(currentBasicData,SearchOrderResult.ASC);

                searchEngine.SearchAdvanced(this.advancedSearchListAction.ToList());
            }
        }
        protected override void Update(IBasicData data)
        {
            //if (!data.Equals(this.CurrentBasicData))
            //{
                CurrentBasicData = data;
                if (localData)
                    SearchBasicData = this.CurrentBasicData;
                LocalDataName = this.CurrentBasicData.TagName;
                GlobalDataName = core.ListPrimaryItens.TagName;
            //}
        }
        protected override void UpdateNoAttached(IBasicData basicData)
        {
            try
            {
                Update(basicData);
            }
            catch { }
        }
        private bool isUnique;

        public bool IsUnique
        {
            get { return isUnique; }
            set { isUnique = value;OnPropertyChanged(); }
        }
        private bool uniqueName;

        public bool UniqueName
        {
            get { return uniqueName; }
            set { uniqueName = value; OnPropertyChanged(); }
        }
        private bool uniqueValue;

        public bool UniqueValue
        {
            get { return uniqueValue; }
            set { uniqueValue = value; OnPropertyChanged(); }
        }
        private string tag;

        public string Tag
        {
            get { return tag; }
            set { tag = value;OnPropertyChanged(); }
        }
        private string attributeName;

        public string AttributeName
        {
            get { return attributeName; }
            set { attributeName = value; OnPropertyChanged(); }
        }
        private string attributeValue;

        public string AttributeValue
        {
            get { return attributeValue; }
            set { attributeValue = value; OnPropertyChanged(); }
        }
        private string localDataName;
        public string LocalDataName
        {
            get { return localDataName; }
            set { localDataName = value; OnPropertyChanged(); }
        }
        private string globalDataName;
        public string GlobalDataName
        {
            get { return globalDataName; }
            set { globalDataName = value; OnPropertyChanged(); }
        }
        private void addParam(string tag)
        {
            try
            {
                SearchAdvancedParamsViewModel sap = new SearchAdvancedParamsViewModel();

                sap.SearchBasicData = this.CurrentBasicData;
               // string tag = (sender as Button).Tag.ToString();

                switch (tag)
                {
                    case "TagName":
                        sap.SearchParam = Tag;
                        sap.SearchAction = searchEngine.GetDataByTagName;
                        sap.Condition = "Tag Name = ";
                        break;
                    case "AttributeName":
                        sap.SearchParam = AttributeName;
                        sap.SearchAction = searchEngine.GetDataByAttributeName;
                        sap.IsUnique = UniqueName;
                        sap.Condition = "Attribute Name = ";
                        break;
                    case "AttributeValue":
                        sap.SearchParam = AttributeValue;
                        sap.SearchAction = searchEngine.GetDataByAttributeValue;
                        sap.IsUnique = UniqueValue;
                        sap.Condition = "Attribute Value = ";
                        break;
                    case "Guid":
                        sap.SearchParam = Guid;
                        sap.SearchAction = searchEngine.GetDataByGuid;
                        sap.Condition = "Guid = ";
                        break;
                    case "AttributeValuePartial":
                        sap.SearchParam = AttributeValue;
                        sap.SearchAction = searchEngine.GetDataByAttributeValuePartial;
                        sap.IsUnique = UniqueValue;
                        sap.Condition = "Attribute Value % ";
                        break;
                }

                this.AdvancedSearchListAction.Add(sap);
                //listView_tags.ItemsSource = null;
                //listView_tags.ItemsSource = this.AdvancedSearchListAction;
            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }

        }





        private void getParam(string tag)
        {
            //string tag = (sender as Button).Tag.ToString();

            switch (tag)
            {
                case "TagName":
                        Tags  = setCollection(tags,searchEngine.SearchAllTags(this.CurrentBasicData));
                    break;
                case "AttributeName":
                        AttributesName = setCollection(attributesName,searchEngine.SearchAllAttributesName(this.CurrentBasicData, SearchOrderResult.ASC));
                    break;
                case "AttributeValue":
                        AttributesValue = setCollection(attributesValue,searchEngine.SearchAllAttributesValue(this.CurrentBasicData, SearchOrderResult.ASC));
                    break;
            }

        }
        private ObservableCollection<object> setCollection(ObservableCollection<object> collection, List<object> itens)

        {
            for (int i = 0; i < itens.Count; i++)
            {
                if(!collection.Contains(itens[i]))
                {
                    collection.Add(itens[i]);
                }
            }
            return collection;
        }
        private string attributeValueTag = "AttributeValue";

        public string AttributeValueTag
        {
            get { return attributeValueTag; }
            set { attributeValueTag = value; OnPropertyChanged(); }
        }


        private void setAttributeTag(bool like)
        {
            if (like)
            {
                AttributeValueTag = "AttributeValuePartial";
            }
            else
            {
                AttributeValueTag = "AttributeValue";
            }
        }
        public string GetGuid(string text)
        {
            Regex reg = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match math = reg.Match(text);
            return math.Value;

        }

        private string guid;

        public string Guid
        {
            get { return guid; }
            set { guid = value;
                OnPropertyChanged();
            }
        }
        private bool localData = true;

        private void menuItemCopyGuid()
        {
            Clipboard.SetText(GetGuid(Guid));
        }

        private void menuItemPastGuid()
        {
            Guid = GetGuid(Clipboard.GetText());
        }
        private void menuItemPastGuid2()
        {
            AttributeValue = GetGuid(Clipboard.GetText());
        }
        private void menuItemPast()
        {
            AttributeValue = Clipboard.GetText();
        }
        private void menuItemCopy()
        {
             Clipboard.SetText(AttributeValue);
        }
        private void menuItemCut()
        {
            Clipboard.SetText(AttributeValue);
            AttributeValue = "";
        }
        private void menuItemClear()
        {
            AttributeValue = "";
        }
       
        private void clearSearchs()
        {

        }
        private void setLocalData(bool s)
        {
            if (s)
            {
                localData = true;
                SearchBasicData = CurrentBasicData;
            }
        }
        private void setGlocalData(bool s)
        {

            if (s)
            {
                localData = false;
                SearchBasicData = core.ListPrimaryItens;
            }
        }
    }
}
