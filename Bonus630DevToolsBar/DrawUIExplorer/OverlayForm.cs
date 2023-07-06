using br.com.Bonus630DevToolsBar.DrawUIExplorer.DataClass;
using br.com.Bonus630DevToolsBar.DrawUIExplorer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer
{
    public partial class OverlayForm : Form
    {

        Rectangle rectOverlay;
        Rectangle rectWindow;
        Pen redPen;
        SolidBrush transparentBlackBrush;

        public OverlayForm(System.Windows.Rect rect)
        {
            start(rect);
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.Location = new Point(0, 0);
            this.rectWindow = new Rectangle(this.Location, this.Size);
        }

        public OverlayForm(System.Windows.Rect rect, System.Windows.Rect rect2)
        {
            start(rect);
            this.rectWindow = new Rectangle((int)rect2.X, (int)rect2.Y, (int)rect2.Width, (int)rect2.Height);
        }
        public OverlayForm(CorelAutomation automation, string itemParentGuid, string itemGuid, Action<IBasicData, bool> restoration, IBasicData restorationData, bool v)
        {
            System.Windows.Rect rect = automation.GetItemRect(itemParentGuid, itemGuid);

            if (rect.IsZero())
            {
                automation.Core.SetUIVisible = true;
                if (restoration != null)
                    restoration.Invoke(restorationData, v);
                this.Close();
            }

            if (automation.Core.CorelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
            {
                start(rect);
                this.Size = Screen.PrimaryScreen.Bounds.Size;
                this.Location = new Point(0, 0);
                this.rectWindow = new Rectangle(this.Location, this.Size);
            }

            else
            {
                System.Windows.Rect rect2 = new System.Windows.Rect(automation.Core.CorelApp.AppWindow.Left,
                    automation.Core.CorelApp.AppWindow.Top, automation.Core.CorelApp.AppWindow.Width,
                    automation.Core.CorelApp.AppWindow.Height);
                start(rect);
                this.rectWindow = new Rectangle((int)rect2.X, (int)rect2.Y, (int)rect2.Width, (int)rect2.Height);
            }
        }
        private void start(System.Windows.Rect rect)
        {
            InitializeComponent();
            this.rectOverlay = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            redPen = new Pen(Brushes.Red);
            redPen.Width = 2;
            Color black20 = Color.FromArgb(210, Color.Black);
            transparentBlackBrush = new SolidBrush(black20);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(transparentBlackBrush, this.rectWindow);
            e.Graphics.FillRectangle(Brushes.White, this.rectOverlay);
            e.Graphics.DrawRectangle(redPen, rectOverlay);
            SetCursorPos(this.rectOverlay.X + (this.rectOverlay.Width / 2), this.rectOverlay.Y + (this.rectOverlay.Height / 2));
            base.OnPaint(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this.rectOverlay.Contains(e.Location))
                this.Close();
            base.OnMouseMove(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Close();
        }
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        private void OverlayForm_KeyUp(object sender, KeyEventArgs e)
        {
            this.Close();
        }

        private void OverlayForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void OverlayForm_Leave(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
