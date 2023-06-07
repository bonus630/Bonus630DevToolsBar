using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Input;

namespace br.com.Bonus630DevToolsBar.RecentFiles
{
    public class KeyHook : IDisposable
    {

        public event Action<Key> KeyUpEvent;

        // Constantes do Windows
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        // Delegate do hook de teclado
        public  delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Importação da função SetWindowsHookEx
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        // Importação da função UnhookWindowsHookEx
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // Importação da função CallNextHookEx
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // Importação da função GetModuleHandle
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Hook de teclado
        public IntPtr hookId = IntPtr.Zero;

        public KeyHook()
        {
            // Registra o hook de teclado
            hookId = SetKeyboardHook();
        }

        private IntPtr SetKeyboardHook()
        {
            // Cria o delegate do hook de teclado
            LowLevelKeyboardProc proc = HookCallback;

            // Obtém o módulo atual
            IntPtr moduleHandle = GetModuleHandle(null);

            // Registra o hook de teclado
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, moduleHandle, 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Verifica se a tecla foi pressionada
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                // Obtém a estrutura do evento de teclado
                KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                // Captura a tecla pressionada
                Key key = KeyInterop.KeyFromVirtualKey(hookStruct.vkCode);

                if (KeyUpEvent != null)
                    KeyUpEvent(key);
            }

            // Chama o próximo hook na cadeia
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            // Remove o hook de teclado ao fechar a janela
            UnhookWindowsHookEx(hookId);
        }
    }

    // Estrutura do evento de teclado
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

}
