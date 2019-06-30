using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DRnamespace
{
    public class Graph
    {
        private int px;
        private int py;
        private int width;
        private int height;
        private double scaler;

        private bool DetectDifference = true;

        private Bitmap bmp_new, bmp_last;
        private IntPtr bmp_intptr;
        private Graphics bitGraph;
        
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr a, IntPtr b, long count);

        [DllImport("gdi32.dll")]
        private static extern void DeleteObject(IntPtr obj);

        public Graph(bool detect)
        {
            DetectDifference = detect;
            bitGraph = Graphics.FromImage(new Bitmap(1, 1));
        }

        public void Initial(double Left, double Top, double Width, double Height)
        {
            scaler = GetDpiScaleProportion();

            Left *= scaler;
            Top *= scaler;
            Width *= scaler;
            Height *= scaler;

            px = (int)Left;
            py = (int)Top;
            width = (int)Width;
            height = (int)Height;
            width = width % 2 == 0 ? width : width - 1;
            height = height % 2 == 0 ? height : height - 1;

            bmp_new = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp_last = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bitGraph = Graphics.FromImage(bmp_new);
            bmp_intptr = IntPtr.Zero;
        }

        public void Shift(double Left, double Top)
        {
            Left *= scaler;
            Top *= scaler;
            px = (int)Left;
            py = (int)Top;
        }

        public Boolean IsScreenChanged()
        {
            //fast comparison : https://stackoverflow.com/questions/2031217/what-is-the-fastest-way-i-can-compare-two-equal-size-bitmaps-to-determine-whethe

            if (DetectDifference)
            {
                var a_b = bmp_new.LockBits(new Rectangle(new System.Drawing.Point(0, 0), bmp_new.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var b_b = bmp_last.LockBits(new Rectangle(new System.Drawing.Point(0, 0), bmp_last.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr
                    a_ptr = a_b.Scan0,
                    b_ptr = b_b.Scan0;

                int res = memcmp(a_ptr, b_ptr, a_b.Height * a_b.Stride);

                bmp_new.UnlockBits(a_b);
                bmp_last.UnlockBits(b_b);

                return res != 0;
            }
            else
                return true;
        }

        public void ActiveDectect()
        {
            DetectDifference = true;
        }

        public void StopDectect()
        {
            DetectDifference = false;
        }

        public void Capture()
        {
            bitGraph.CopyFromScreen(px, py, 0, 0, new System.Drawing.Size(width, height));
        }

        public void UpdatePrevious()
        {
            bmp_last.Dispose();
            bmp_last = (Bitmap)bmp_new.Clone();
        }

        public byte[] BmpToByte()
        {
            var bmpStream = new MemoryStream();
            bmp_new.Save(bmpStream, ImageFormat.Jpeg);
            return bmpStream.ToArray();
        }

        public void UpdateIntPtr()
        {
            if (bmp_intptr == IntPtr.Zero)
                bmp_intptr = bmp_new.GetHbitmap();
        }

        public void DeleteIntPtr()
        {
            DeleteObject(bmp_intptr);
            bmp_intptr = IntPtr.Zero;
        }

        public IntPtr GetIntPtr()
        {
            return bmp_intptr;
        }

        public int h()
        {
            return height;
        }

        public int w()
        {
            return width;
        }

        public double GetDpiScaleProportion()
        {
            return bitGraph.DpiX / 96.0;
        }
    }
}