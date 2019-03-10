using System;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //graph
        int px, py, width, height;
        Bitmap bmp, bmp_old;
        Graphics bitGraph;

        //dynamic link libs
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr a, IntPtr b, long count);

        void intializeGraph()
        {
            px = (int)Left;
            py = (int)Top;
            width = (int)Width;
            height = (int)Height;
            width = (width % 2 == 0) ? width : width - 1;
            height = (height % 2 == 0) ? height : height - 1;

            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp_old = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bitGraph = Graphics.FromImage(bmp);
        }

        Boolean IsBitmapsDiff(Bitmap a, Bitmap b)
        {
            var a_b = a.LockBits(new Rectangle(new System.Drawing.Point(0, 0), a.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var b_b = b.LockBits(new Rectangle(new System.Drawing.Point(0, 0), b.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            IntPtr
                a_ptr = a_b.Scan0,
                b_ptr = b_b.Scan0;

            int res = memcmp(a_ptr, b_ptr, a_b.Height * a_b.Stride);


            a.UnlockBits(a_b);
            b.UnlockBits(b_b);

            return res != 0;
        }
    }
}