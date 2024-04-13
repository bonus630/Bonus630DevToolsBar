using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;


namespace br.com.Bonus630DevToolsBar.RunCommandDocker.MyPopup
{
    public class MyPopup : Popup
    {
        public static readonly DependencyProperty ToReflectProperty = DependencyProperty.Register("ToReflect",
                                                    typeof(object), typeof(MyPopup), new FrameworkPropertyMetadata(null));
        public object ToReflect
        {
            get { return GetValue(ToReflectProperty); }
            set { SetValue(ToReflectProperty, value); }
        }

        public event Action PopupCloseEvent;

        private Reflected reflected;
        private Thread getChildrenThread;

        public MyPopup()
        {
            PopupContent popupContent = new PopupContent();
            popupContent.ReflectedIsExpandedEvent += (reflected) => { GetChildrens(reflected); };
            this.Child = popupContent;
        }
        private bool IsValueType(object propertyValue)
        {
            return propertyValue == null || propertyValue.GetType().IsValueType || propertyValue.GetType().Equals(typeof(string));
        }
        protected override void OnOpened(EventArgs e)
        {
            if (ToReflect != null && !ToReflect.GetType().IsValueType)
            {
                closeCounter = 0;
                runToClose = false;
                base.OnOpened(e);

                reflected = new Reflected();
                object obj = ToReflect;
                reflected.Value = obj;
                try
                {
                    GetChildrens(reflected);
                    (this.DataContext as ProjectsManager).SelectedCommand.ReflectedProp = reflected;

                }
                catch { }
            }
            else
            {
                RequestClose();
            }

        }
        public void GetChildrens(Reflected parent)
        {

            //For get a item in um generic IEnumerable collection
            //1º lets check if type IsGenericType
            //2º Use the main type and GetInterfaces
            //3º Check which interfaces are implementeds
            //IList,ICollection,IEnumerable,...
            //4º Get the generic type GetGenericArguments()
            //5º Use the implemented interface and generic argument to cast reflected collection
            //6º Go through all items and reflect them
            //Can we put these items in the item property?
            //Is required a checking for enum types, enum types can throw exceptions if yours values cames wrong
            object obj = parent.Value;
            Type mainType = parent.Value.GetType();
            bool isValueType = mainType.IsValueType || mainType.Equals(typeof(string));
            Reflected item = null;
            if (string.IsNullOrEmpty(parent.Name))
                parent.Name = obj.GetType().FullName;

            if (parent.Name.Equals("Item"))
            {
                mainType = parent.Parent.Value.GetType();
                Type[] interfaces = mainType.GetInterfaces();
                // Type genericType = mainType.GetGenericTypeDefinition();

                Type _interface = interfaces.FirstOrDefault(r => r.Name.Equals("ICollection") || r.Name.Equals("IList") || r.Name.Equals("IEnumerable"));
                if (_interface == null)
                    return;
                //var generics = parent.Parent.Value as dynamic;
                //mainType = parent.Parent.GetType();
                var property = mainType.GetProperties().FirstOrDefault(p => p.Name.Equals("Item"));
                int counter = (int)mainType.GetProperties().FirstOrDefault(p => (p.Name.Equals("Count") || p.Name.Equals("Length"))).GetValue(obj, null);
                if (property != null)
                {
                    object v = null;
                    isValueType = false;
                    try
                    {
                        ParameterInfo[] parameters = property.GetIndexParameters();
                        int index = 0;
                        while (index <= counter)
                        {
                            try
                            {
                                v = property.GetValue(obj, new object[] { index });
                                if (v == null)
                                    isValueType = false;
                                else
                                {
                                    Type vType = v.GetType();
                                    isValueType = vType.IsValueType || vType.Equals(typeof(string));
                                }
                                item = new Reflected() { Name = string.Format("{0}[{1}]", property.Name, index), Value = v, IsValueType = isValueType, Parent = parent };
                                if (index == 0)
                                    parent.Childrens[0] = item;
                                else
                                    parent.Childrens.Add(item);
                                if (!isValueType && v != null)
                                {
                                    //Here can use recursivity to fill all treeview nodes
                                    item.Childrens = new ObservableCollection<Reflected>();
                                    item.Childrens.Add(null);
                                }
                            }
                            catch { }
                            // Childrens.Add(item);
                            index++;
                        }


                    }
                    catch { }
                }
                //foreach (var generic in generics)
                //{
                //    bool isValueType = false;
                //    Type itemType = generic.GetType();
                //    try
                //    {
                //        isValueType = itemType.IsValueType || itemType.Equals(typeof(string));
                //    }
                //    catch { }
                //    item = new Reflected() { Name = itemType.Name, Value = generic, IsValueType = isValueType, Parent = parent };
                //    if (!isValueType)
                //    {
                //        //Here can use recursivity to fill all treeview nodes
                //        item.Childrens = new ObservableCollection<Reflected>();
                //        item.Childrens.Add(null);
                //    }
                //    Childrens.Add(item);
                //}
            }
            else if (parent.Name.Equals("SyncRoot"))
            {

                if (parent.Value is Array && (parent.Value as Array).Length > 0)
                {
                    bool vType = IsValueType((parent.Value as Array).GetValue(0));
                    object _value;
                    for (int i = 0; i < (parent.Value as Array).Length; i++)
                    {
                        _value = (parent.Value as Array).GetValue(i);
                        parent.Childrens.Add(new Reflected() { Name = string.Format("[{0}]", i), Value = _value, IsValueType = IsValueType(_value), Parent = parent });
                    }

                }
            }
            else if (isValueType)
            {
                Reflected children = new Reflected() { Name = mainType.Name, Value = parent.Value, IsValueType = isValueType, Parent = parent };
                parent.Add(children);
            }
            else
            {
                FillProperties(parent);
            }
            //Test for a value type arrays


        }
        private void FillProperties(Reflected parent)
        {
            object obj = parent.Value;
            if (obj == null)
                return;
            Type mainType;
            Reflected children = null;
            ObservableCollection<Reflected> Childrens = new ObservableCollection<Reflected>();
            mainType = obj.GetType();
            var properties = mainType.GetProperties();
            bool isCollection = false;
            Type _interface = mainType.GetInterfaces().FirstOrDefault(r => r.Name.Equals("ICollection") || r.Name.Equals("IList") || r.Name.Equals("IEnumerable"));
            var item = properties.FirstOrDefault(p => p.Name.Equals("Item"));
            if (mainType.IsArray || _interface != null)
            {
                isCollection = true;
            }
            foreach (var property in properties)
            {
                object propertyValue = null;
                bool isValueType = false;
                try
                {
                    ParameterInfo[] parameters = property.GetIndexParameters();
                    if (parameters.Length == 0)
                    {
                        propertyValue = property.GetValue(obj, null);
                        isValueType = IsValueType(propertyValue);
                        if (isValueType)
                            children = new Reflected() { Name = property.Name, Value = propertyValue, IsValueType = isValueType, Parent = parent };
                        else
                            children = CreateNoValueTypeChildren(parent, property.Name, propertyValue);
                    }
                    else
                    {
                        children = CreateNoValueTypeChildren(parent, property.Name, obj);
                    }
                }
                catch (TargetInvocationException tie) { children = CreateExceptionChildren(parent, property.Name, tie.Message); }
                catch (AccessViolationException ave) { children = CreateExceptionChildren(parent, property.Name, ave.Message); }
                catch (Exception ex) { children = CreateExceptionChildren(parent, property.Name, ex.Message); }
                //if (!isValueType && propertyValue != null)
                //{
                //    Type _interface = mainType.GetInterfaces().FirstOrDefault(r => r.Name.Equals("ICollection") || r.Name.Equals("IList") || r.Name.Equals("IEnumerable"));

                //    if (mainType.IsArray || _interface != null)
                //    {

                //        children = CreateEnumerableChildren(parent, obj);

                //    }
                //    else
                //    {
                //        //Here can use recursivity to fill all treeview nodes
                //        children.Childrens = new ObservableCollection<Reflected>();
                //        children.Childrens.Add(null);
                //    }
                //}
                Childrens.Add(children);
            }



            parent.Childrens = Childrens;
        }
        private Reflected CreateNoValueTypeChildren(Reflected parent, string name, object obj)
        {
            Reflected item = new Reflected() { Name = name, Value = obj, IsValueType = false, Parent = parent };
            item.Childrens = new ObservableCollection<Reflected>();
            item.Childrens.Add(null);
            return item;

        }
        private Reflected CreateExceptionChildren(Reflected parent, string name, string msg)
        {
            Reflected item = new Reflected() { Name = name, Value = msg, IsValueType = true, Parent = parent, Error = true };
            return item;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Close();
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            closeCounter = 0;
            runToClose = false;
            if (counterThread != null)
                counterThread.Abort();
        }

        int closeCounter = 0;
        int ellapseTime = 100;
        int loops = 10;
        bool runToClose = false;
        Thread counterThread;


        private void Close()
        {
            runToClose = true;
            counterThread = new Thread(new ThreadStart(() =>
            {

                while (runToClose)
                {

                    Thread.Sleep(ellapseTime);
                    closeCounter++;
                    if (closeCounter > loops)
                    {
                        try
                        {
                            if (getChildrenThread != null)
                            {
                                getChildrenThread.Abort();
                                getChildrenThread = null;
                            }
                        }
                        catch
                        {

                        }
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            RequestClose();
                        }));
                    }
                }

            }));
            counterThread.IsBackground = true;
            counterThread.Start();

        }
        private void RequestClose()
        {
            if (PopupCloseEvent != null)
                PopupCloseEvent();
        }
    }

}
