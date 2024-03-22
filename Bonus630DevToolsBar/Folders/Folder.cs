using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.ViewModels;

namespace br.com.Bonus630DevToolsBar.Folders
{
    public class Folder : ViewModelBase
    {
        public Folder()
        {
				
        }
        public Folder(Folders folders)
        {
			this.Parent = folders;
        }

		public Folders Parent { get; set; }
        private int index;

		public int Index
		{
			get { return index; }
			set { index = value; }
		}

		private string path;

		public string Path
		{
			get { return path; }
			set { path = value; OnPropertyChanged(); }
        }

        private ImageSource icone;

        public ImageSource Icone
        {
            get { return icone; }
            set { icone = value; OnPropertyChanged(); }
        }

		private string iconePath = string.Empty;

		public string IconePath { get { return iconePath; } }

		public void SetIcone(string iconePath)
		{
			this.iconePath = iconePath;
			if(System.IO.File.Exists(iconePath))
				Icone = new BitmapImage(new Uri(iconePath,UriKind.Absolute));
			OnPropertyChanged("Icone");
		}

		public string GetIcone()
		{
			return iconePath;
		}

    }
}
