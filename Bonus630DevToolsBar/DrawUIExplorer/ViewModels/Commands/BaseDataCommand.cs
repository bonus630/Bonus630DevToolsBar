using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels.Commands
{
    public class BaseDataCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Action<IBasicData> basicDataContentExec;
        public Predicate<IBasicData> basicDataContentCanExec;

        public BaseDataCommand(Action<IBasicData> basicDataContentExec, Predicate<IBasicData> basicDataContentCanExec)
        {
            this.basicDataContentExec = basicDataContentExec;
            this.basicDataContentCanExec = basicDataContentCanExec;
        }

        public bool CanExecute(object parameter)
        {
            return basicDataContentCanExec == null ? true : basicDataContentCanExec(parameter as IBasicData);

        }

        public void Execute(object parameter)
        {
            basicDataContentExec.Invoke(parameter as IBasicData);
        }
    }
}
