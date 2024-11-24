using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer
{
    public partial class PrintScreenForm : Form
    {
        //Thread t;
        bool close = false;
        public PrintScreenForm(System.Windows.Rect rect)
        {
            start(rect);
        }
        public PrintScreenForm(System.Windows.Rect rect, System.Windows.Rect rect2)
        {
            start(rect);
        }
        private void start(System.Windows.Rect rect)
        {
            

            InitializeComponent();
            this.Location = new Point((int)rect.X, (int)rect.Y);
            this.Size = new Size((int)rect.Width, (int)rect.Height);
            SaveFileDialog of = new SaveFileDialog();
            of.Filter = "(Image)|*.png";
            
            if (of.ShowDialog() == DialogResult.OK)
            {
                //t = new Thread(new ThreadStart(AutoClose));
                //t.IsBackground = true;
                //t.Start();
                using (Bitmap bitmap = new Bitmap((int)rect.Width, (int)rect.Height))
                {

                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.CopyFromScreen(this.Location, new Point(0, 0), this.Size);


                    bitmap.Save(of.FileName, ImageFormat.Png);
                }
            }
           // close = true;
            this.Close();
        }
        private void AutoClose()
        {
            int counter = 0;
            while (true)
            {
                if (counter > 5)
                {
                    this.Invoke(new Action(() => { this.Close(); }));
                }
                if (close)
                    counter++;
                Thread.Sleep(100);
            }
        }
    }
}
