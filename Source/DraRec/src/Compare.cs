/*
 * Author : Brian Tu (RTU)
 * Last modify : -
 * 
 * The class Compare compares images to see if they are identical.
 */

using System;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DRnamespace
{
    public class Compare
    {
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr a, IntPtr b, long count);

        bool DetectDifference = true;
        private Bitmap last = null;
        private Bitmap target = null;
        bool ret = false;

        Thread thread=null;

        public bool Comparing(Bitmap bmp)
        {
            if ((thread==null || !thread.IsAlive) && DetectDifference)
            {
                target = (Bitmap)bmp.Clone();
                thread = new Thread(run);
                thread.Start();
            }
            return !DetectDifference || ret;
        }

        public void ActiveDectect()
        {
            DetectDifference = true;
            Trace.WriteLine("Dectect state : " + DetectDifference);
        }

        public void StopDectect()
        {
            DetectDifference = false;
            Trace.WriteLine("Dectect state : " + DetectDifference);
        }

        private void run()
        {
            try
            {
                if (last == null)
                {
                    last = target;
                    ret = true;
                    return;
                }

                //fast comparison : https://stackoverflow.com/questions/2031217/what-is-the-fastest-way-i-can-compare-two-equal-size-bitmaps-to-determine-whethe

                var a_b = target.LockBits(new Rectangle(new Point(0, 0), target.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var b_b = last.LockBits(new Rectangle(new Point(0, 0), last.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                IntPtr
                    a_ptr = a_b.Scan0,
                    b_ptr = b_b.Scan0;

                int res = memcmp(a_ptr, b_ptr, a_b.Height * a_b.Stride);

                target.UnlockBits(a_b);
                last.UnlockBits(b_b);

                last.Dispose();
                last = target;

                ret = res != 0;
                return;
            }
            catch(Exception e)
            {
                Trace.WriteLine("Compare failed : " + e.Data);
            }
            ret = true;
            return;
        }
    }
}
