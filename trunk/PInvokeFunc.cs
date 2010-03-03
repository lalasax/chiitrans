using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace ChiiTrans
{
    class PInvokeFunc
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(int key);

        public static bool IsKeyDown(Keys key)
        {
            short value = GetAsyncKeyState((int)key);
            return ((value & -2) != 0);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        public const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        public const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
        public const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;
        public const uint LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040;

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop,
            EnumDesktopWindowsDelegate lpfn, IntPtr lParam);

        public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, int lParam);
        
        public static IntPtr[] GetDesktopWindowHandles(IntPtr hDesktop)
        {
            handlesBuf.Clear();
            EnumDesktopWindows(hDesktop, new EnumDesktopWindowsDelegate(EnumDesktopWindowsProc), IntPtr.Zero);
            IntPtr[] res = handlesBuf.ToArray();
            handlesBuf.Clear();
            return res;
        }

        private static List<IntPtr> handlesBuf = new List<IntPtr>();
        private static bool EnumDesktopWindowsProc(IntPtr hWnd, int notused)
        {
            handlesBuf.Add(hWnd);
            return true;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT r = new RECT();
            if (!GetWindowRect(hWnd, out r))
                return new Rectangle();
            else
                return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        unsafe public static byte[] ByteArrayFromPtr(IntPtr ptr)
        {
            byte* start = (byte*)ptr;
            byte* cur = start;
            while (*cur != 0)
                ++cur;
            int size = (int)(cur - start);
            byte[] res = new byte[size];
            Marshal.Copy(ptr, res, 0, size);
            return res;
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        /*const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_NORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;
        const int SW_MAXIMIZE = 3;*/
        public const int SW_SHOWNOACTIVATE = 4;
        /*const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;
        const int SW_SHOWMINNOACTIVE = 7;
        const int SW_SHOWNA = 8;
        const int SW_RESTORE = 9;
        const int SW_SHOWDEFAULT = 10;
        const int SW_FORCEMINIMIZE = 11;
        const int SW_MAX = 11;*/
    }
}
