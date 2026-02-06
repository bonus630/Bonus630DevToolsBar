using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{

    public abstract class CommandBase : MarshalByRefObject, INotifyPropertyChanged
    {
        private string name;
        public virtual string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; OnPropertyChanged("Index"); }
        }



        //Lets uses this flag for now
        public bool MarkToDelete = false;
        protected bool isSelected;
        public virtual bool IsSelectedBase
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelectedBase");
             
            }
        }
        protected bool isExpanded;
        public virtual bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
    //public interface ICommandCollection
    //{
    //    void AddAndCheckRange(ICommandCollection range);

    //}
    //Apos mudar os parametros de um commando ele nao atualiza
    public abstract class CommandCollectionBase<T> : CommandBase where T : CommandBase
    {

        private ObservableCollection<T> items;

        public virtual ObservableCollection<T> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }
        //Precisamos mudar este métodos de ordenação, utilizando CollectionViewSource
        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-sort-and-group-data-using-a-view-in-xaml?view=netframeworkdesktop-4.8
        public virtual void Order()
        {
            this.Items = new ObservableCollection<T>(Items.OrderBy(r => r.Name));
            OnPropertyChanged("Items");
        }
        public virtual void OrderDesc()
        {
            this.Items = new ObservableCollection<T>(Items.OrderByDescending(r => r.Name));
            OnPropertyChanged("Items");
        }
        public virtual void OrderOriginal()
        {
            this.Items = new ObservableCollection<T>(Items.OrderBy(r => r.Index));
            OnPropertyChanged("Items");
        }
   
        public int Count
        {
            get
            {
                if (Items == null)
                    return 0;
                else
                    return Items.Count;
            }

        }



        public virtual void Add(T item)
        {
            if (Items == null)
                Items = new ObservableCollection<T>();
            Items.Add(item);
        }
        public virtual void AddRange(T[] range)
        {
            if (Items == null)
                Items = new ObservableCollection<T>();
            for (int i = 0; i < range.Length; i++)
            {
                Items.Add(range[i]);
            }
        }
        //public void AddAndCheckRange(ICommandCollection commandCollectionRange)
        //{
        //    if (Items == null)
        //        Items = new ObservableCollection<T>();

        //    foreach (var item in range)
        //    { 
        //        if (!Items.Contains(item))
        //            Items.Add(item as T);
        //    }

        //    foreach (var item in Items)
        //    {
        //        if (!range.Contains(item))
        //        {
        //            item.MarkToDelete = true;
        //        }
        //        else
        //        {
        //            T i = range.FirstOrDefault<T>(r => r.Equals(item));
        //            if (i != null)
        //                (item as ICommandCollection).AddAndCheckRange(i);
        //        }
        //    }
        //    int count = Items.Count;
        //    for (int i = 0; i < count;)
        //    {
        //        if (Items[i].MarkToDelete)
        //            Items.RemoveAt(i);
        //        else
        //            i++;
        //    }

        // }
        public bool Contains(T item)
        {
            return Items.Contains(item);
        }
        public virtual void Remove(T item)
        {
            if (Items != null)
                Items.Remove(item);
        }
        public virtual T this[int index] { set { Items[index] = value; } get { return Items[index]; } }
    }

    public class Project : CommandCollectionBase<Module>
    {
        public object Parent { get; set; }
        public string Path { get; set; }
        public override string ToString() { return Name; }

        private bool loaded = true;
        public bool Loaded { get { return loaded; }
            set { loaded = value;OnPropertyChanged("Loaded"); }
        }
        string projFilePath = "";
        public string ProjFilePath
        {
            get { return projFilePath; }
            set
            {
                projFilePath = value;
                OnPropertyChanged("ProjFilePath");
            }
        }
        public void SwitchLoaded()
        {
            string path;//string name;
            if(loaded)
            {
                path = System.IO.Path.ChangeExtension(this.Path, ".bak");
                System.IO.File.Move(this.Path, path);
                //name = string.Format("Unloaded {0}",this.Name);
            }
            else
            {
                path = System.IO.Path.ChangeExtension(this.Path, ".dll");
                System.IO.File.Move(this.Path, path);
               // name = this.Name.Replace("Unloaded ", "");
            }
            Loaded = !loaded;
            this.Path = path;
           // this.Name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Project)
            {
               // string.Compare(this.Path, (obj as Project).Path, true);
                return this.Path.ToLower().Equals((obj as Project).Path.ToLower());
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return 467214278 + EqualityComparer<string>.Default.GetHashCode(Path);
        }
  
        public void AddAndCheckRange(ObservableCollection<Module> moduleList)
        {
            if (Items == null)
                Items = new ObservableCollection<Module>();
            for (int i = 0; i < moduleList.Count; i++)
            {
                if (!Items.Contains(moduleList[i]))
                    Items.Add(moduleList[i]);
            }
            foreach (var module in Items)
            {
                if (!moduleList.Contains(module))
                {
                    module.MarkToDelete = true;
                }
                else
                {
                    var i = moduleList.FirstOrDefault(r => r.Equals(module));
                    if (i != null)
                        module.AddAndCheckRange(i.Items);
                }
            }
            int count = 0;
            while (count < Items.Count)
            {
                if (Items[count].MarkToDelete)
                {
                    Items.RemoveAt(count);

                }
                else
                    count++;
            }
        }

    }

    public class Module : CommandCollectionBase<Command>
    {
        public Project Parent { get; set; }
        public string FullName { get; set; }
        public string ModulePath { get; set; }
        public override string ToString() { return string.Format("{0}/{1}", Parent.Name, Name); }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Module)
            {
                return this.ToString().ToLower().Equals((obj as Module).ToString().ToLower());
            }
            else
                return false;
        }
        public override void Add(Command command)
        {
            if (Items == null)
                Items = new ObservableCollection<Command>();
            int count = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ToString().Equals(command.ToString()))
                    count++;
            }
            Items.Add(command);

            command.ID = count;
        }
        public void RemoveAt(int commandIndex)
        {
            Command toDelete = Items[commandIndex];
            if(toDelete.ID > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].ToString().Equals(toDelete.ToString()) && Items[i].ID > toDelete.ID)
                        Items[i].ID--;

                }

            }
            Items.RemoveAt(commandIndex);
        }
       
        public override int GetHashCode()
        {
            return 733961487 + EqualityComparer<string>.Default.GetHashCode(this.ToString());
        }
        public void AddAndCheckRange(ObservableCollection<Command> commandList)
        {
        //    if (Items == null)
        //        Items = new ObservableCollection<Command>();
            for (int i = 0; i < commandList.Count; i++)
            {
                if (!Items.Contains(commandList[i]))
                    this.Add(commandList[i]);
                //else
                //{
                //    this.Items[this.Items.IndexOf(commandList[i])].ReturnsType = commandList[i].ReturnsType;
                //}
            }
            foreach (var comand in Items)
            {
                if (!commandList.Contains(comand))
                {
                    comand.MarkToDelete = true;
                }
                //else
                //{
                //    var i = range.FirstOrDefault(r => r.Equals(item));
                //    if (i != null)
                //        item.AddAndCheckRange(i.Items);
                //}
            }
            int count = 0;
            while (count < Items.Count)
            {
                if (Items[count].MarkToDelete)
                {
                    this.RemoveAt(count);

                }
                else
                    count++;
            }
        }
    }

    public class Command : CommandCollectionBase<Argument>
    {
        public Module Parent { get; set; }
        private int recursionProtection = 0;
        private object[] argumentsCache;
        public event Action<bool> ArgumentsReady;
        public object[] ArgumentsCache
        {
            get
            {
                return this.argumentsCache;
            }
        }
        public int ID { get; set; }
        internal void PrepareArguments()
        {
            recursionProtection++;
            try
            {
                object[] objects = null;

                if (this.Items != null)
                {
                    int length = this.Items.Count;
                    if (length > 0)
                        objects = new object[length];
                    for (int i = 0; i < length; i++)
                    {
                        if (Items[i].Value != null && !(Items[i].Value is DBNull))
                        {
                            if (Items[i].ArgumentType.IsEnum)
                            {
                                objects[i] = Enum.Parse(Items[i].ArgumentType, Items[i].Value.ToString());
                                continue;
                            }
                            if (Items[i].Value.GetType().IsValueType || Items[i].Value is string)
                            {
                                try
                                {
                                    objects[i] = Convert.ChangeType(Items[i].Value, Items[i].ArgumentType);
                                }
                                catch (InvalidCastException e)
                                {
                                    objects[i] = null;
                                }
                                catch (Exception e)
                                {
                                    objects[i] = null;
                                }
                            }
                            else
                            {
                                if (recursionProtection > 100)
                                {
                                    objects[i] = null;
                                    return;
                                }

                                objects[i] = (Items[i].Value as FuncToParam).MyFunc.Invoke(this);
                            }
                        }
                        else
                            objects[i] = null;
                    }
                }
                this.argumentsCache = objects;
            }
            catch (StackOverflowException e)
            {
                onArgumentsReady(false);
                System.Windows.Forms.MessageBox.Show("UNBOUNDED RECURSION!");
            }
            onArgumentsReady(true);
        }
        private void onArgumentsReady(bool ready)
        {
            if (ArgumentsReady != null)
                ArgumentsReady(ready);
        }
        public string Method { get; set; }
        public override string ToString() { return string.Format("{0}/{1}/{2}", Parent.Parent.Name, Parent.Name, Method); }

        public event Action<Command> CommandSelectedEvent;

        private object returns;
        public object Returns
        {
            get { return returns; }
            set
            {
                returns = value;
                OnPropertyChanged("Returns");
                OnPropertyChanged("ReturnsType");
            }
        }

        private Type returnsType;
        public Type ReturnsType { 
            get { return returnsType; } 
            set { returnsType = value; OnPropertyChanged("ReturnsType"); } }

        private string consoleOut;
        public string ConsoleOut
        {
            get { return consoleOut; }
            set { consoleOut = value; OnPropertyChanged("ConsoleOut"); }
        }

        private Reflected reflected;

        public Reflected ReflectedProp
        {
            get { return reflected; }
            set
            {
                reflected = value;
                OnPropertyChanged("ReflectedProp");
            }
        }

        private bool hasParam = false;
        public bool HasParam { get { return hasParam; } set { hasParam = value; OnPropertyChanged("HasParam"); } }
        private void onCommandSelected()
        {
            recursionProtection = 0;
            if (CommandSelectedEvent != null)
                CommandSelectedEvent(this);
        }
        public override bool IsSelectedBase
        {
            get { return base.IsSelectedBase; }
            set
            {

                base.IsSelectedBase = value;

                onCommandSelected();

            }
        }
        private bool canStop = false;
        private bool lastRunFails;
        private AggregateException lastException = null;
        public bool CanStop
        {
            get { return canStop; }
            set
            {
                canStop = value;

                OnPropertyChanged("CanStop");

            }
        }
        #region Timer
 
        private string elapsedTime;
        public string ElapsedTime
        {
            get { return elapsedTime; }
            set
            {
                elapsedTime = value;
                OnPropertyChanged("ElapsedTime");
            }
        }
  
        private Stopwatch stopwatch;

        public void StartTimer()
        {
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            else
                stopwatch.Restart();
            ElapsedTime = "";
      
        }
        public void EndTimer()
        {
            ElapsedTime = stopwatch.Elapsed.ToString(@"h\:mm\:ss\.fff");
        }

        #endregion Timer
        public bool LastRunFails { get { return lastRunFails; }
            set { lastRunFails = value;
                OnPropertyChanged("LastRunFails");
            } }
        public AggregateException LastException { get { return lastException; }
            set { lastException = value;
                OnPropertyChanged("LastException");
            } }
        public bool CheckSelected()
        {
            if (IsSelectedBase)
            {
                OnPropertyChanged("Items");
            }
            return IsSelectedBase;
        }
        public void AddRange(object[] range)
        {
            if (Items == null && range.Length > 0)
            {
                Items = new ObservableCollection<Argument>();
                HasParam = true;
            }
            for (int i = 0; i < range.Length; i++)
            {
                Argument arguments = new Argument();
                if (range[i] is Argument)
                {
                    arguments.Name = (range[i] as Argument).Name;
                    //Tipos customizados não são carregador nessa etapa, sendo que são carregados corretamente na etapa anterior, no proxy, vamos lidar com a exception por enquanto
                    try
                    {
                        arguments.ArgumentType = (range[i] as Argument).ArgumentType;
                    }
                    catch(FileNotFoundException fnfe)
                    {
                        arguments.ArgumentType = typeof(object);
                    }
                    catch(ArgumentException e)
                    {
                        arguments.ArgumentType = typeof(object);
                    }
                    arguments.Value = (range[i] as Argument).Value;
                    arguments.Options = (range[i] as Argument).Options;
                }
                else
                {

                    arguments.Name = (range[i] as Tuple<string, Type>).Item1;
                    arguments.ArgumentType = (range[i] as Tuple<string, Type>).Item2;
                }
                arguments.Parent = this;
                Items.Add(arguments);

            }

        }
     
        public override bool Equals(object obj)
        {

            try
            {
                Command c = obj as Command;
                if (c == null)
                    return false;
                if (!c.ToString().Equals(this.ToString()))
                    return false;
                if (!c.returnsType.Equals(this.returnsType))
                    return false;
                if (c.Items == null && this.Items == null)
                    return true;
                if (c.Items != null && this.Items != null)
                {
                    if (!c.Items.Count.Equals(this.Items.Count))
                        return false;
                    for (int i = 0; i < c.Items.Count; i++)
                    {
                        if (!c.Items[i].Equals(this.Items[i]))
                            return false;
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            int hashCode = 1974103776;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(returnsType);
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode(Name);
            if (Items != null)
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Items.Count);
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Items);
            }
            else
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(0);

            return hashCode;
        }
    }
    public class Argument : CommandBase
    {
        public Command Parent { get; set; }

        private Type argumentType;

        public Type ArgumentType
        {
            get { return argumentType; }
            set
            {
                argumentType = value;
                OnPropertyChanged("ArgumentType");
            }
        }
        private object _value;
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }   
        //private bool incompatibleArgumentType;
        //public bool IncompatibleArgumentType
        //{
        //    get { return incompatibleArgumentType; }
        //    set
        //    {
        //        if (!value)
        //        {
        //            incompatibleArgumentType = value;
        //            OnPropertyChanged("CompatibleArgumentType");
        //        }
        //    }
        //}  
        //public void SetIncompatibleArgumentType()
        //{
        //    IncompatibleArgumentType = true;
        //    OnPropertyChanged("IncompatibleArgumentType");
        //}
        private ObservableCollection<object> options;
        public ObservableCollection<object> Options
        {
            get { return options; }
            set
            {
                options = value;
                OnPropertyChanged();
            }
        }
        public void OptionsAddRange(object[] range)
        {
            if (Options == null)
                Options = new ObservableCollection<object>();
            for (int i = 0; i < range.Length; i++)
            {
                Options.Add(range[i]);
            }
        }
        public override bool Equals(object obj)
        {
            bool equals = false;
            try
            {
                Argument argument = obj as Argument;
                if (argument != null)
                    equals = (argument.Name.Equals(this.Name)) && (argument.ArgumentType.Equals(this.ArgumentType));
            }
            catch
            {
                
            }
            return equals;
        }

        public override int GetHashCode()
        {
            int hashCode = 1974103776;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ArgumentType);
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
   
}