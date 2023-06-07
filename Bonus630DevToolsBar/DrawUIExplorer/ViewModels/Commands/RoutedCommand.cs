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
        

        public RoutedCommand(Action<T> action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action.Invoke((T)parameter);
        }
    }
}
