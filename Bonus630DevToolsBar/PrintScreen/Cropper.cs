using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace br.com.Bonus630DevToolsBar.PrintScreen
{
    public partial class Cropper : Form
    {
        private Rectangle mainRect;
        private string folderPath = "";
        private string fileName = "";
        private Corel.Interop.VGCore.Application corelApp;
        public Cropper(Corel.Interop.VGCore.Application corelApp)
        {
            InitializeComponent();
            this.corelApp = corelApp;
            SetLocationSize(this.corelApp.ActiveWindow.Left, this.corelApp.ActiveWindow.Top, this.corelApp.ActiveWindow.Width, this.corelApp.ActiveWindow.Height);
           
        
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Process();

            this.Close();
        }
        public void SetLocationSize(double x, double y, double width, double height)
        {
            this.Location = new Point((int)x, (int)y);
            this.Size = new Size((int)width + 1, (int)height + 1);
            this.mainRect = new Rectangle(this.Location, this.Size);
        }
        private void Process()
        {
            try
            {
                if (string.IsNullOrEmpty(this.folderPath))
                    SetFolderPath();
                string filePath = string.Format("{0}\\P-{1}-{2}.jpg", this.folderPath, this.fileName, DateTime.Now.ToString("yyyyMMddHHmmss"));
                Cursor.Hide();
                Bitmap bitmap = new Bitmap(mainRect.Width, mainRect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, mainRect.Size, CopyPixelOperation.SourceCopy);
                bitmap.Save(filePath);
                Clipboard.SetImage(bitmap);
            }
            catch { }
            finally
            {
                Cursor.Show();
            }
        }
        public void SetFolderPath(string fileName = "", string folderPath = "")
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                FolderBrowserDialog fd = new FolderBrowserDialog();
                fd.ShowNewFolderButton = true;
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo dr = new DirectoryInfo(fd.SelectedPath);
                    if (dr.Exists)
                        this.folderPath = dr.FullName;
                }

            }
            else
            {
                this.folderPath = folderPath;
                this.fileName = fileName;
            }
        }
    }
}
