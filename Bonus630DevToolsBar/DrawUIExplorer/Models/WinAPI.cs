using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer
{
    public static class WinAPI
    {
        public const int WM_CLOSE = 16;
        public const int BN_CLICKED = 245;

        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

        [DllImport("User32.dll")]
        public static extern System.IntPtr SetFocus(System.IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(System.IntPtr hWnd);
    }
}
