using br.com.Bonus630DevToolsBar.RunCommandDocker;
using br.com.Bonus630DevToolsBar.RunCommandDocker.Styles;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using form = System.Windows.Forms;

namespace br.com.Bonus630DevToolsBar.Folders
{
    /// <summary>
    /// Interaction logic for Folders.xaml
    /// </summary>
    public partial class Folders : UserControl
    {

        public BindingCommand<string> PastFolderCommand { get; set; }
        public BindingCommand<Folder> OpenFolderCommand { get; set; }
        public BindingCommand<Folder> PastIconeCommand { get; set; }
        public BindingCommand<Folder> RemoveCommand { get; set; }
        public ObservableCollection<Folder> _Folders { get; set; }

        private StylesController stylesController;
        private FolderModel folderModel;

        public Folders(object app)
        {
            InitializeComponent();

            PastFolderCommand = new BindingCommand<string>(PastFolder);
            OpenFolderCommand = new BindingCommand<Folder>(OpenFolder);
            PastIconeCommand = new BindingCommand<Folder>(PastIcone);
            RemoveCommand = new BindingCommand<Folder>(Remove);

            folderModel = new FolderModel((app as Corel.Interop.VGCore.Application).VersionMajor);
            _Folders = new ObservableCollection<Folder>();
            StartFolderShortcuts();
           
            DataContext = this;
            try
            {
                stylesController = new StylesController(this.Resources, app as Corel.Interop.VGCore.Application);
                stylesController.LoadThemeFromPreference();
            }
            catch { }
        }

        private void PastFolder(string folder)
        {
            folder = Clipboard.GetText();
            folder = folder.Trim('"', ' ');
            Folder f = null;
            int id = folderModel.CheckExits("folderPath", folder);
            if (id > -1)
            {
                for (int i = 0; i < _Folders.Count; i++)
                {
                    if (_Folders[i].Index == id)
                        f = _Folders[i];
                }
            }
            else
            {
                f = new Folder(this);
                _Folders.Add(f);

            }

            f.Path = folder;
            string icon = TryDefaultIcon(folder);
            if (!string.IsNullOrEmpty(icon))
                f.SetIcone(icon);

            if (id > -1)
                folderModel.UpdateFile(f);
            else
                folderModel.InsertData(f);
            //SaveOrUpdate(f);

        }
        private string TryDefaultIcon(string folder)
        {
            string result = string.Empty;
            string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bonus630", "Folders");
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            // Possível expressão para contornar bugs de hosts com dots        
            //Possível ER para encontrar host
            //^(?:http|https):\/\/(?:www.)?(?<host>(?:[a-zA-Z0-9-_]+\.)?([a-zA-Z0-9-_\.]+))(?:\.\w+)?(?:\.\w{2,4})(?:.+)
            //^(?:http|https):\/\/(?:www.)?(?:[a-zA-Z0-9-_]+\.)?(?<host>([a-zA-Z0-9-_\.]+))(?:\.\w{2,4})(?:.+)
            //Regex exp1 = new Regex(@"^(?:http|https):\/\/(?:www.)?(?<host>(?:[a-zA-Z0-9-_]+\.)?([a-zA-Z0-9-_\.]+))(?:\.\w+)?(?:\.\w{2,4})(?:.+)",options);
            //Regex exp1 = new Regex(@"^(?:http|https):\/\/(?:www.)?(?:[a-zA-Z0-9-_]+\.)?(?<host>([a-zA-Z0-9-_\.]+))(?:\.\w{2,4})(?:\.\w+)?(?:.+)",options);
            //Regex exp1 = new Regex(@"^(?:http|https):\/\/(?:www([0-9]{0,2}).)?(?:[a-zA-Z0-9-_]+\.)?(?<host>([a-zA-Z0-9-_\.]+))(?:\.\w{2,4})(?:\.\w+)?(?:.+)", options);
            //'/^(([\w]+:)?\/\/)?(([\d\w]|%[a-fA-f\d]{2,2})+(:([\d\w]|%[a-fA-f\d]{2,2})+)?@)?([\d\w][-\d\w]{0,253}[\d\w]\.)+[\w]{2,4}(:[\d]+)?(\/([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)*(\?(&?([-+_~.\d\w]|%[a-fA-f\d]{2,2})=?)*)?(#([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)?$/'
            // string urlTeste = "http://www.uol.com.br";

            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled;
            Regex exp1 = new Regex(@"^(?:http|https):\/\/(?:www([0-9]{0,2})\.)?(?:[a-zA-Z0-9-_]+\.)?(?<host>([a-zA-Z0-9-_\.]+))\.(com|biz|co|se|sx|art|in|tv|org|ind|info|net|ws|gl|ly|to|us|sex|xxx|eu|tk|tt|cr|cz|io|li|cc)(?:\.\w{2,4})?(?:\.\w+)?(?:.+)", options);
            string host = "";
            if (!string.IsNullOrEmpty(folder))
            {
                try
                {
                    if (exp1.IsMatch(folder))
                    {
                        Match match = exp1.Match(folder);
                        try
                        {
                            host = match.Result("${host}");
                        }
                        catch
                        {

                        }
                        if (string.IsNullOrEmpty(host))
                            result = string.Format("{0}\\{1}.ico", Guid.NewGuid().ToString());
                        else
                            result = string.Format("{0}\\{1}.ico", dest, host);
                        var client = new System.Net.WebClient();

                        client.DownloadFile(
                            @"https://www.google.com/s2/favicons?sz=32&domain_url=" + folder,
                            result);
                        return result;
                    }
                }
                catch { }


            }



            if (Directory.Exists(folder))
            {
                try
                {
                    string abs = Path.GetFileName(folder).Substring(0, 2).ToUpper();
                    result = string.Format("{0}\\{1}.png", dest, Path.GetFileName(folder));
                    System.Drawing.Bitmap bitmap = GenIcone(abs, br.com.Bonus630DevToolsBar.Properties.Resources.folderBase);
                    bitmap.Save(result, ImageFormat.Png);
                    return result;
                }
                catch { }
            }


            if (File.Exists(folder))
            {
                try
                {
                    System.Drawing.Bitmap icon = Icon.ExtractAssociatedIcon(folder).ToBitmap();
                    result = string.Format("{0}\\{1}.ico", dest, Path.GetFileName(folder));
                    icon.Save(result);
                    return result;
                }
                catch { }
            }
            return result;
        }
        private void OpenFolder(Folder folder)
        {
            try
            {
                System.Diagnostics.Process.Start(folder.Path);
            }
            catch { }
        }
        private System.Drawing.Bitmap GenIcone(string text, System.Drawing.Bitmap imageBase)
        {

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(32, 32);
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(Brushes.Transparent, new System.Drawing.Rectangle(0, 0, 32, 32));
            g.DrawImage(imageBase, new System.Drawing.Rectangle(0, 0, 32, 32), new System.Drawing.Rectangle(0, 0, imageBase.Width, imageBase.Height), GraphicsUnit.Pixel);


            System.Drawing.Font f = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            SizeF textSize = g.MeasureString(text, f);
            g.DrawString(text, f, Brushes.WhiteSmoke, GetPoint(bitmap.Size, textSize));
            f = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
            textSize = g.MeasureString(text, f);
            g.DrawString(text, f, Brushes.Red, GetPoint(bitmap.Size, textSize));

            bitmap.MakeTransparent(System.Drawing.Color.Transparent);

            return bitmap;
        }
        private System.Drawing.Point GetPoint(System.Drawing.Size bitmap, SizeF textSize)
        {
            return new System.Drawing.Point((int)(bitmap.Width / 2 - (textSize.Width / 2) + 1), (int)(bitmap.Height - textSize.Height - 2));
        }
        private void PastIcone(Folder folder)
        {
            try
            {
                form.OpenFileDialog of = new form.OpenFileDialog();
                of.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;";
                of.Multiselect = false;
                of.DefaultExt = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (of.ShowDialog().Equals(form.DialogResult.OK))
                {
                    folder.SetIcone(of.FileName);
                    folderModel.UpdateFile(folder);
                }
            }
            catch
            {
                
            }
        }
     
        private void Remove(Folder folder)
        {
            folderModel.DeleteFile(folder.Index);
            _Folders.Remove(folder);

        }
        private void StartFolderShortcuts()
        {
            folderModel.Fill(_Folders, this);
        
        }
    }
}
