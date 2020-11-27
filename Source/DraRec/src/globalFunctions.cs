/*
 * Author : RTU(keroroxzz)
 * Last modify : 2020/8/11
 * 
 * Global functions
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DRnamespace
{
    static class GlobalFunctions
    {
        static double 
            wpfDpi = 96.0,
            dpi_scaling = 1.0;

        static GlobalFunctions()
        {
            RefreshDpiScaling();
        }

        public static double RefreshDpiScaling()
        {
            dpi_scaling = Graphics.FromImage(new Bitmap(1, 1)).DpiX / wpfDpi;
            return dpi_scaling;
        }

        public static double DpiScaling()
        {
            return dpi_scaling;
        }

        public static Point MousePosition()
        {
            var mp = Control.MousePosition;
            return new Point((int)( mp.X / dpi_scaling ), (int)( mp.Y / dpi_scaling ));
        }

        public static MouseButtons GetMouseButtons()
        {
            return Control.MouseButtons;
        }

        public static bool IsMousePressed()
        {
            return Control.MouseButtons != MouseButtons.None;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        public static void DrawCursor(Graphics g, int x, int y)
        {
            CURSORINFO ci;
            ci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            if (GetCursorInfo(out ci) && ci.flags == 1)
            {
                IntPtr h = g.GetHdc();
                DrawIcon(h, ci.ptScreenPos.x - x, ci.ptScreenPos.y - y, ci.hCursor);
                g.ReleaseHdc();
            }    
        }
    }
}
