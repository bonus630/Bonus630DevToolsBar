using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;


namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public interface IBasicData
    {
        string Guid { get; set; }
        string GuidRef { get; set; }
        string TagName { get; set; }
        string Text { get; set; }
        string Label { get; set; }
        //teste getresource
        string Icon { get; set; }
        string Caption { get; set; }
        bool IsSelected { get; }
        bool IsContainer { get; }
        bool ChildrenSelected { get; set; }
        bool Marked { get; set; }
        SolidColorBrush MarkColor { get; set; }
        void SetSelected(bool isSelected, bool? isExpands, bool update, bool recursive = false);
        int XmlChildrenID { get; }
        int PathIndex { get; }
        int XmlChildrenParentID { get; }
        int TreeLevel { get; }
        List<Attribute> Attributes { get; set; }
        IBasicData Parent { get; set; }
        ObservableCollection<IBasicData> Childrens { get; set; }

        void SetPathIndex(int index);
        void SetXmlChildreParentID(int id = -1);
        void SetTreeLevel(int parentLevel);
        void Add(IBasicData basicData);
        void SetXmlChildreID(int id);
        bool ContainsAttribute(string AttributeName);
        bool ContainsAttributeValue(string AttributeValue);
        bool ContainsAttributeValuePartial(string AttributeValue);
        bool IAmUniqueTag();
        bool IsSpecialType { get; }
        string GetAttribute(string AttributeName);
        string GetAnyGuidAttribute();
        IBasicData GetParentByType<T>(int treeLevel = -1);

        Type GetType(IBasicData basicData);

        event Action<bool, bool?, bool> SelectedEvent;
    }
}
