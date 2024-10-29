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
        //Pen bluePen;
       // SolidBrush transparentBlueBrush;
        public bool NormalMode = true;
        public IntPtr OwnerHandle { get; set; }
        public OverlayForm(System.Windows.Rect rect)
        {
            start(rect);
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.Location = new Point(0, 0);
            this.rectWindow = new Rectangle(this.Location, this.Size);
 
        }
        public OverlayForm(System.Windows.Rect corelRect, System.Windows.Rect itemRect,bool normalMode = false)
        {
            NormalMode = normalMode;
            start(itemRect);
            this.rectWindow = new Rectangle((int)corelRect.X, (int)corelRect.Y, (int)corelRect.Width, (int)corelRect.Height);
           // transparentBlueBrush = new SolidBrush(Color.FromArgb(255, 226, 246, 239));
           // bluePen = new Pen(new SolidBrush(Color.FromArgb(255, 168, 221, 246)), 1);
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
                automation.Core.SetUIState = XMLTagWindowStates.Visible;
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
            if (NormalMode)
            {
                e.Graphics.FillRectangle(transparentBlackBrush, this.rectWindow);
                e.Graphics.FillRectangle(Brushes.White, this.rectOverlay);
                SetCursorPos(this.rectOverlay.X + (this.rectOverlay.Width / 2), this.rectOverlay.Y + (this.rectOverlay.Height / 2));
            }
            
            e.Graphics.DrawRectangle(redPen, rectOverlay);
            //else
            //{
            //    e.Graphics.FillRectangle(transparentBlueBrush, this.rectOverlay);
            //    e.Graphics.DrawLine(bluePen, this.rectWindow.Left, this.rectOverlay.Top, this.rectWindow.Right, this.rectOverlay.Top);
            //    e.Graphics.DrawLine(bluePen, this.rectWindow.Left, this.rectOverlay.Bottom, this.rectWindow.Right, this.rectOverlay.Bottom);
            //    e.Graphics.DrawLine(bluePen, this.rectOverlay.Left, this.rectWindow.Top,this.rectOverlay.Left, this.rectWindow.Bottom);
            //    e.Graphics.DrawLine(bluePen, this.rectOverlay.Right, this.rectWindow.Top, this.rectOverlay.Right, this.rectWindow.Bottom);
            //}
           
       
            base.OnPaint(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this.rectOverlay.Contains(e.Location) && NormalMode)
                this.Close();
            base.OnMouseMove(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if(NormalMode)
                this.Close();
        }
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        private void OverlayForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (NormalMode)
                this.Close();
        }

        private void OverlayForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (NormalMode)
                this.Close();
        }

        private void OverlayForm_Leave(object sender, EventArgs e)
        {
            if (NormalMode)
                this.Close();
        }

        internal void UpdateLayoutMode(System.Windows.Rect rect)
        {
            this.rectOverlay = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            Invalidate();
        }
        public void Show(bool noFocus)
        {
            base.Show(new WpfWrapper(this.Handle));
        }
    }
    public class WpfWrapper : IWin32Window
    {
        public WpfWrapper(IntPtr handle)
        {
            Handle = handle; 
        }
        public IntPtr Handle { get; set; }
    }
}
