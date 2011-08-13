using System;
using System.Runtime.InteropServices;

namespace SimpleMVVM
{
    public static partial class Storage
    {
        private static class Win32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Point
            {
                public int X;
                public int Y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WindowPlacement
            {
                public int length;
                public int flags;
                public int showCmd;
                public Point minPosition;
                public Point maxPosition;
                public Rect normalPosition;
            }

            public const int ShowNormal = 1;
            public const int ShowMinimized = 2;

            private const string User32 = "user32.dll";

            [DllImport(User32)]
            public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

            [DllImport(User32)]
            public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);
        }
    }
}
