using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //LICENSE String
        String about = "Drawing Recorder Alpha\n" +
            "Libraries Used :\n" +
            "FluentWPF Project under the MIT License.\n" +
            "FFmpeg project (unmodified) under the LGPLv2.1.";

        //thread
        Thread Record_thread, UI_thread;

        //booleans
        Boolean
            isCapturing = false,
            isStarted = false,
            CheckScreenDif = true,
            isTargetLock = true;

        [DllImport("gdi32.dll")]
        private static extern void DeleteObject(IntPtr obj);

        public MainWindow()
        {
            InitializeComponent();

            ErrorString = about;

            this.Opacity = 0.0;

            //initialize threads
            Record_thread = new Thread(RecordThread);
            Record_thread.Start();

            UI_thread = new Thread(UIthread);
            UI_thread.Start();
        }

        void RecordThread()
        {
            while (true)
            {
                if (isCapturing && (TargetName == ActiveName || !isTargetLock || !isStarted))
                {
                    try
                    {
                        //capture screen
                        bitGraph.CopyFromScreen(px, py, 0, 0, new System.Drawing.Size(width, height));
                            
                        //compare with previouse one
                        if (!CheckScreenDif || (IsBitmapsDiff(bmp, bmp_old) && CheckScreenDif))
                        {
                            //refresh pre-screen
                            bmp_old.Dispose();
                            bmp_old = (Bitmap)bmp.Clone();
                                
                            IntPtr bmp_ptr = bmp.GetHbitmap();

                            //save file
                            if (isStarted)
                            {
                                try
                                {
                                    using (var bmpStream = new MemoryStream())
                                    {
                                        bmp.Save(bmpStream, ImageFormat.Jpeg);
                                        ffmpegStream.Write(bmpStream.ToArray(), 0, (int)bmpStream.Length);
                                    }
                                }
                                catch { }
                            }

                            Action deleg = delegate ()
                            {
                                ImageBox.Source = Imaging.CreateBitmapSourceFromHBitmap(bmp_ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                                //dispose resources
                                DeleteObject(bmp_ptr);
                            };
                            Dispatcher.BeginInvoke(deleg);

                            Thread.Sleep(CapInt);
                        }
                        else
                            Thread.Sleep(50);
                    }
                    catch { }
                }
                else
                    Thread.Sleep(50);
            }
        }
    }
}
