using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            //this.KeyDown += Cropper_KeyDown;
            SetLocationSize(this.corelApp.ActiveWindow.Left, this.corelApp.ActiveWindow.Top, this.corelApp.ActiveWindow.Width, this.corelApp.ActiveWindow.Height);
           
        
        }

        //private void Cropper_KeyDown(object sender, KeyEventArgs e)
        //{
        //    Debug.WriteLine(e.KeyCode);
            //corel.Window cdrWin = this.corelApp.ActiveWindow;
            //if (cdrWin == null)
            //    return;
            ////int cdrWinTop = cdrWin.Top, cdrWinLeft = cdrWin.Left, cdrWinWidth = cdrWin.Width, cdrWinHeight = cdrWin.Height;
            //System.Drawing.Rectangle cdrWinRect = new System.Drawing.Rectangle(new System.Drawing.Point(cdrWin.Left, cdrWin.Top), new System.Drawing.Size(cdrWin.Width, cdrWin.Height));
            //int left = 0, top = 0, right = 0, bottom = 0;
            //switch (obj)
            //{
            //    case 0:
            //        mForm.SetLocationSize(0, 0, 0, 0);
            //        mForm.SetFitMessage("Default position");
            //        break;
            //    case 1:

            //        if (this.corelApp.ActiveSelection.Shapes.Count > 0)
            //        {
            //            this.corelApp.ActiveWindow.DocumentToScreen(corelApp.ActiveSelection.LeftX, corelApp.ActiveSelection.TopY, out left, out top);
            //            this.corelApp.ActiveWindow.DocumentToScreen(corelApp.ActiveSelection.RightX, corelApp.ActiveSelection.BottomY, out right, out bottom);
            //            if (right < cdrWinRect.Left || left > cdrWinRect.Right || top > cdrWinRect.Bottom || bottom < cdrWinRect.Top)
            //            {
            //                mForm.SetLocationSize(0, 0, 0, 0);
            //                mForm.SetFitMessage("Selection is off screen");
            //                break;
            //            }
            //            if (left < cdrWinRect.Left)
            //                left = cdrWinRect.Left;
            //            if (top < cdrWinRect.Top)
            //                top = cdrWinRect.Top;
            //            if (right > cdrWinRect.Right)
            //                right = cdrWinRect.Right;

            //            if (bottom > cdrWinRect.Bottom)
            //                bottom = cdrWinRect.Bottom;


            //            mForm.SetLocationSize(left, top, right - left, bottom - top);
            //            mForm.SetFitMessage("Fit to \"Selection\"");
            //            this.corelApp.ActiveSelection.Shapes.All().RemoveFromSelection();
            //        }
            //        break;
            //    case 2:

            //        if (this.corelApp.ActivePage != null)
            //        {
            //            this.corelApp.ActiveWindow.DocumentToScreen(corelApp.ActivePage.LeftX, corelApp.ActivePage.TopY, out left, out top);
            //            this.corelApp.ActiveWindow.DocumentToScreen(corelApp.ActivePage.RightX, corelApp.ActivePage.BottomY, out right, out bottom);
            //            if (right < cdrWinRect.Left || left > cdrWinRect.Right || top > cdrWinRect.Bottom || bottom < cdrWinRect.Top)
            //            {
            //                mForm.SetLocationSize(0, 0, 0, 0);
            //                mForm.SetFitMessage("Page is off screen");
            //                break;
            //            }
            //            if (left < cdrWinRect.Left)
            //                left = cdrWinRect.Left;
            //            if (top < cdrWinRect.Top)
            //                top = cdrWinRect.Top;
            //            if (right > cdrWinRect.Right)
            //                right = cdrWinRect.Right;

            //            if (bottom > cdrWinRect.Bottom)
            //                bottom = cdrWinRect.Bottom;

            //            mForm.SetLocationSize(left, top, right - left, bottom - top);
            //            mForm.SetFitMessage("Fit to \"Page\"");
            //        }
            //        break;
            //    case 3:
            //        mForm.SetLocationSize(this.corelApp.ActiveWindow.Left, this.corelApp.ActiveWindow.Top, this.corelApp.ActiveWindow.Width, this.corelApp.ActiveWindow.Height);
            //        mForm.SetFitMessage("Fit to \"Window\"");
            //        break;
            //    case 4:
            //        mForm.SetLocationSize(0, 0, 100, 100);
            //        mForm.SetFitMessage("Fit to \"FullScreen\"");
            //        break;

           // }
       // }

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
