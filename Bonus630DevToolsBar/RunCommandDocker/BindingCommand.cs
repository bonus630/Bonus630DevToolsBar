using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class BindingCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        Action<T> RunPath;
        Predicate<T> canRun;

        public BindingCommand(Action<T> action, Predicate<T> canRun = null)
        {
            this.RunPath = action;
            this.canRun = canRun;
        }
        public bool CanExecute(object parameter)
        {
            return canRun == null ? true : canRun((T)parameter);
        }
        public void Execute(object parameter)
        {
            RunPath.Invoke((T)parameter);
            
        }
    }
    public class SimpleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action RunPath;
      
        public SimpleCommand(Action action)
        {
            this.RunPath = action;
        }
      
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            RunPath.Invoke();
        }
    }
}
