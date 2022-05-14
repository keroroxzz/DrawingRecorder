/*
 * Author : RTU(keroroxzz)
 * Last modify : 2020/8/11
 * 
 * Graph is for stuff like screenshot, graphics, and bitmap.
 * 
 * improve the stream process
 * Grip resize add when proportion unset
 * Exceptiom handle for capture
 */

using System;
using System.Drawing;
using System.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using static DRnamespace.GlobalFunctions;

using Rect = System.Windows.Rect;
using Size = System.Drawing.Size;
using System.Windows.Media.Animation;
using System.Net.Http.Headers;

namespace DRnamespace
{
    public class Graph
    {
        private int px;
        private int py;
        private int width;
        private int height;
        private double scaler;

        private Bitmap bmp;
        private Graphics graphic;
        private IntPtr bmp_intptr;

         [DllImport("gdi32.dll")]
        private static extern void DeleteObject(IntPtr obj);

        public Graph()
        {
            bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        }

        public void Initial(Rect area, bool isFull)
        {
            Trace.WriteLine("Initializing a Graph...");

            scaler = DpiScaling();

            area.X *= scaler;
            area.Y *= scaler;
            if (!isFull)
            {
                area.Width *= scaler;
                area.Height *= scaler;
            }

            px = (int)area.X;
            py = (int)area.Y;
            width = (int)area.Width;
            height = (int)area.Height;
            width = width % 2 == 0 ? width : width - 1;
            height = height % 2 == 0 ? height : height - 1;

            bmp_intptr = IntPtr.Zero;
            lock (bmp)
            {
                bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                graphic = Graphics.FromImage(bmp);
            }
        }

        public void Shift(double Left, double Top)
        {
            lock (bmp)
            {
                px = (int)( Left * scaler );
                py = (int)( Top * scaler );
            }
        }

        public void Capture(bool rm)
        {
            lock (bmp)
                for (int i=1; ;i++)
                {
                    try
                    {
                        graphic.CopyFromScreen(px, py, 0, 0, new Size(width, height));
                        if (rm)
                            DrawCursor(graphic, px, py);

                        break;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("CopyFromScreen Failed!" + e.Message);
                        Trace.TraceError("Retry for "+ i+" time.");

                        if (i > 10)
                        {
                            MessageBox.Show("Fail to capture! Please restart DrawingRecorder!");
                            break;
                        }
                    }
                }
        }

        public Bitmap GetBitmap()
        {
            lock (bmp)
                return (Bitmap)bmp.Clone();
        }

        public void UpdateIntPtr()
        {
            if (bmp_intptr == IntPtr.Zero)
                lock (bmp)
                    bmp_intptr = bmp.GetHbitmap();
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

        /*public int h()
        {
            return height;
        }

        public int w()
        {
            return width;
        }*/
    }
}