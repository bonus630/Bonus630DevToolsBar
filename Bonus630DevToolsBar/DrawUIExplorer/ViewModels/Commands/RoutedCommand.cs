using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands
{
    public class RoutedCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Action<T> action;
        public Predicate<object> predicate;

        public RoutedCommand(Action<T> execute, Predicate<object> canExecute = null)
        {
            this.action = execute;
            this.predicate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return predicate != null ? predicate.Invoke(parameter) : true;
        }

        public void Execute(object parameter)
        {
            action.Invoke((T)parameter);
        }
    }
}
