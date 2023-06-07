using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands
{
    public class AttributeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Action<DataClass.Attribute> attributeContentExec;
        public Predicate<DataClass.Attribute> attributeContentCanExec;

        public AttributeCommand(Action<DataClass.Attribute> attributeContentExec, Predicate<DataClass.Attribute> attributeContentCanExec)
        {
            this.attributeContentExec = attributeContentExec;
            this.attributeContentCanExec = attributeContentCanExec;
        }

        public bool CanExecute(object parameter)
        {
            return attributeContentCanExec == null ? true : attributeContentCanExec(parameter as DataClass.Attribute);
           
        }

        public void Execute(object parameter)
        {
            attributeContentExec.Invoke(parameter as DataClass.Attribute);
        }
    }
}
