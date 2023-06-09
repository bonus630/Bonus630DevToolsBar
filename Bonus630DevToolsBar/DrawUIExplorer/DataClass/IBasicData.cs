﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass
{
    public interface IBasicData
    {
        string Guid { get; set; }
        string GuidRef { get; set; }
        string TagName { get; set; }
        string Text { get; set; }
        //teste getresource
        string Icon { get; set; }
        bool IsSelected { get;  }
        bool IsContainer { get; }
        void SetSelected(bool isSelected,bool? isExpands, bool update, bool recursive = false);
        string Caption { get; set; }
        int XmlChildreID { get; }
        int PathIndex { get; }
        int XmlChildreParentID { get; }
        int TreeLevel {  get; }
        List<Attribute> Attributes { get; set; }
        IBasicData Parent { get; set; }
        ObservableCollection<IBasicData> Childrens { get; set; }

        Type GetType(IBasicData basicData);
         bool ContainsAttribute(string AttributeName);
         bool ContainsAttributeValue(string AttributeValue);
        bool ContainsAttributeValuePartial(string AttributeValue);
        string GetAttribute(string AttributeName);
        void SetXmlChildreID(int id);

        string GetAnyGuidAttribute();

        void SetPathIndex(int index);

         void SetXmlChildreParentID(int id = -1);
        void SetTreeLevel(int parentLevel);
         event Action<bool,bool?,bool> SelectedEvent;
        void Add(IBasicData basicData);
        bool IAmUniqueTag();
        bool IsSpecialType { get; }
    }
}
