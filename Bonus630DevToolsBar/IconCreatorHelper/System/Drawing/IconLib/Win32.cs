//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Drawing.IconLib
{
    
    internal class Win32
    {
        #region DELEGATES
        public delegate Int32 EnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam);
        public delegate bool EnumResNameProc(IntPtr hModule, IntPtr pType, IntPtr pName, IntPtr param);
        #endregion

        #region KERNEL32
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("KERNEL32.DLL")]
		public unsafe static extern void CopyMemory(void* dest, void* src, int length);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError =true)] 
		public static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", SetLastError=true)] 
		public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", SetLastError=true)] 
		public static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, ref string pName, ushort wLanguage, byte[] lpData, uint cbData);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Ansi)] 
		public static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr pName, ushort wLanguage, byte[] lpData, uint cbData);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", SetLastError=true)] 
		public unsafe static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, byte[] pName, ushort wLanguage, byte[] lpData, uint cbData);
        //[DllImport("kernel32.dll", SetLastError=true)] 
        //public static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, uint lpName, ushort wLanguage, byte[] lpData, uint cbData);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", EntryPoint="RtlMoveMemory")]
        public unsafe static extern void CopyMemory(RGBQUAD* dest, byte* src, int cb);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int SizeofResource(IntPtr hModule, IntPtr hResource);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int FreeLibrary(IntPtr hModule);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr LockResource(IntPtr hGlobalResource);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResource);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("Kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern bool EnumResourceNames(IntPtr hModule, IntPtr pType, EnumResNameProc callback, IntPtr param);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool EnumResourceTypes(IntPtr hModule, EnumResTypeProc callback, IntPtr lParam);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindResource(IntPtr hModule, string resourceID, IntPtr type);
        //[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr FindResource(IntPtr hModule, Int32 resourceID, IntPtr type);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, IntPtr type);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, string resourceName);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadLibrary(string libraryName);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadLibraryEx(string path, IntPtr hFile, LoadLibraryFlags flags);
        #endregion

        #region GDI32
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll")]
        public static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask,
           int yMask, uint dwRop);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll")] 
        public static extern int SetBkColor(IntPtr hDC, uint crColor);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)] 
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, PatBltTypes dwRop);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool DeleteDC(IntPtr hDC);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool DeleteObject(IntPtr hObject);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("gdi32.dll")]
		static public extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int Width, int Heigth);
        #endregion

        #region USER32
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("user32.dll", CharSet=CharSet.Auto)] 
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr GetDC(IntPtr hWnd);
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        #endregion

        #region MACROS
        public static bool IS_INTRESOURCE(IntPtr value)
        {
            if (((uint) value) > ushort.MaxValue)
                return false;
            return true;
        }

        public static bool IS_INTRESOURCE(string value)
        {
            int iResult;
            return int.TryParse(value, out iResult);
        }

		public static int MAKEINTRESOURCE(int resource)
		{
			return 0x0000FFFF & resource;
		}
        #endregion
    }
}
