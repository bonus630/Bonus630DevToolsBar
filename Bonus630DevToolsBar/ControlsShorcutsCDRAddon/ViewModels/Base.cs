using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace br.com.Bonus630DevToolsBar.ControlsShorcutsCDRAddon.ViewModels
{
    public class Base : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if(PropertyChanged!=null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
