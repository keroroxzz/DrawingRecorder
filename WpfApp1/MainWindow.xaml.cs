using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Brushes = System.Windows.Media.Brushes;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //thread
        Thread Record_thread, UI_thread;

        //booleans
        Boolean
            isCapturing = false,
            isStarted = false,
            CheckScreenDif = true;

        //UI
        double ToolBarHeight = 15.0,
            WindOpacity = 1.0,
            SettingBarHeight = 0.0;
        String mouseEnterP = "", mouseEnter = "";

        //graph
        int px, py, width, height;
        Bitmap bmp, bmp_old;
        Graphics bitGraph;

        //video
        int CapInt = 1000;
        Process ffmpegProcess;
        Stream ffmpegStream;
        String ErrorString = "";

        //dynamic link libs
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr a ,IntPtr b,long count);

        [DllImport("gdi32.dll")]
        private static extern void DeleteObject(IntPtr obj);

        public MainWindow()
        {
            InitializeComponent();

            //initialize threads
            Record_thread = new Thread(RecordThread);
            Record_thread.Start();

            Record_thread = new Thread(UIthread);
            Record_thread.Start();
        }

        Boolean IsBitmapsDiff(Bitmap a,Bitmap b)
        {
            var a_b = a.LockBits(new Rectangle(new System.Drawing.Point(0, 0), a.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var b_b = b.LockBits(new Rectangle(new System.Drawing.Point(0, 0), b.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            IntPtr 
                a_ptr = a_b.Scan0,
                b_ptr = b_b.Scan0;

            int res = memcmp(a_ptr, b_ptr, a_b.Height * a_b.Stride);


            a.UnlockBits(a_b);
            b.UnlockBits(b_b);

            return res!=0;
        }

        private void initializeFFmpeg()
        {
            LockTextBoxs();

            if (ffmpegProcess != null)
            {
                ffmpegProcess.Close();
                ffmpegProcess.Dispose();
            }

            ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = ArgumentBox.Text,
                    FileName = "ffmpeg.exe"
                }
            };

            ffmpegProcess.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
            ffmpegProcess.ErrorDataReceived += (s, e) => ErrorString = e.Data;

            ffmpegProcess.Start();

            ffmpegProcess.BeginErrorReadLine();

            ffmpegStream = ffmpegProcess.StandardInput.BaseStream;
        }

        private void finishFFmpeg()
        {
            UnlockTextBoxs();

            ffmpegStream.Flush();
            ffmpegStream.Dispose();
            ffmpegProcess.Close();
        }

        private void LockTextBoxs()
        {
            FileNameBox.Background = (System.Windows.Media.Brush)FindResource("TextBoxLocked_Color");
            FrameRateBox.Background = (System.Windows.Media.Brush)FindResource("TextBoxLocked_Color");
            ArgumentBox.Background = (System.Windows.Media.Brush)FindResource("TextBoxLocked_Color");
            FileNameBox.IsReadOnly = true;
            FrameRateBox.IsReadOnly = true;
            ArgumentBox.IsReadOnly = true;
        }

        private void UnlockTextBoxs()
        {
            FileNameBox.Background = (System.Windows.Media.Brush)FindResource("TextBox_Color");
            FrameRateBox.Background = (System.Windows.Media.Brush)FindResource("TextBox_Color");
            ArgumentBox.Background = (System.Windows.Media.Brush)FindResource("TextBox_Color");
            FileNameBox.IsReadOnly = false;
            FrameRateBox.IsReadOnly = false;
            ArgumentBox.IsReadOnly = false;
        }

        private String GetArgument()
        {
            String name = FileNameBox.Text;

            if (File.Exists(name + ".mp4"))
            {
                int i = 0;
                for (; File.Exists(name + "_" + i + ".mp4"); i++) ;
                name = name + "_" + i;
            }

            return "-framerate " + FrameRateBox.Text + " -i - -c:v libx265 -vf format=yuv420p -r " + FrameRateBox.Text + "  " + name + ".mp4";
        }

        private Boolean isParametersReady()
        {
            if (FileNameBox.Text.Length > 0 && FrameRateBox.Text.Length > 0 && IntervalBox.Text.Length > 0)
            {
                try
                {
                    Convert.ToDouble(FrameRateBox.Text);
                    Convert.ToDouble(IntervalBox.Text);
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }

        private void stop_but_Click(object sender, RoutedEventArgs e)
        {
            if (isCapturing)
            {
                isStarted = !isStarted;
                if (isStarted)
                    stop_but.Background = Brushes.Red;
                else
                    stop_but.Background = Brushes.GhostWhite;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tool_bar.Height = new GridLength(30);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            WindOpacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            WindOpacity = 0.75;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToolBarHeight = 40.0;
            SettingBarHeight = 100.0;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void SettingGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseEnter = "SB";
        }

        private void SettingGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseEnterP = mouseEnter;
            mouseEnter = "";
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseEnter = "TB";
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mouseEnterP == "SB")
                SettingBarHeight = 0.0;

            mouseEnterP = mouseEnter;
            mouseEnter = "";
        }

        void RecordThread()
        {
            while (true)
            {
                if(isCapturing)
                {
                    //capture screen
                    bitGraph.CopyFromScreen(px, py, 0, 0, bmp.Size);

                    //compare with previouse one
                    if (!CheckScreenDif || (IsBitmapsDiff(bmp, bmp_old) && CheckScreenDif)) 
                    {
                        //refresh pre-screen
                        bmp_old.Dispose();
                        bmp_old = (Bitmap)bmp.Clone();

                        IntPtr bmp_ptr = bmp.GetHbitmap();

                        //save file
                        if (isStarted){
                            try{
                                using (var bmpStream = new MemoryStream()){
                                    bmp.Save(bmpStream, ImageFormat.Jpeg);
                                    ffmpegStream.Write(bmpStream.ToArray(), 0, (int)bmpStream.Length);
                                }
                            }catch { }
                        }

                        //delegate action refreshing the image
                        Action deleg = delegate (){
                            CheckScreenDif = checkBox.IsChecked.Value;
                            try{
                                CapInt = (int)(Convert.ToDouble(IntervalBox.Text) * 1000);}
                            catch { }
                            ImageBox.Source = Imaging.CreateBitmapSourceFromHBitmap(bmp_ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());};
                        Dispatcher.Invoke(deleg);

                        //dispose resources
                        DeleteObject(bmp_ptr);
                    }
                }
                Thread.Sleep(CapInt);
            }
        }

        void UIthread()
        {
            while (true)
            {
                if (mouseEnter == "TB")
                {
                    ToolBarHeight = 40.0;
                }
                else if (mouseEnter == "SB" && mouseEnterP == "TB")
                {
                    SettingBarHeight = 100.0;
                }
                else if (mouseEnter == "" && mouseEnterP == "SB")
                {
                    ToolBarHeight = 15.0;
                    SettingBarHeight = 50.0;
                }
                else if (mouseEnter == "" && mouseEnterP == "TB")
                {
                    ToolBarHeight = 15.0;
                }
                else if (mouseEnter == "SB" && mouseEnterP == "SB")
                {
                    ToolBarHeight = 40.0;
                    SettingBarHeight = 100.0;
                }

                Action dele = delegate ()
                {
                    ErrorMsg.Text = ErrorString;

                    tool_bar.Height = new GridLength((tool_bar.Height.Value + ToolBarHeight) * 0.5);
                    setting_bar.Height = new GridLength((setting_bar.Height.Value + SettingBarHeight) * 0.5);
                    this.Opacity = (WindOpacity+this.Opacity)*0.5;
                    this.WindowState = WindowState.Normal;
                };
                Dispatcher.Invoke(dele);

                Thread.Sleep(50);
            }
        }

        private void Start_but_Click(object sender, RoutedEventArgs e)
        {
            if (!isCapturing)   //start capturing
            {
                px = (int)this.Left;
                py = (int)this.Top;
                width = (int)this.Width;
                height = (int)this.Height;
                width = (width % 2 == 0) ? width : width - 1;
                height = (height % 2 == 0) ? height : height - 1;

                bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmp_old = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bitGraph = Graphics.FromImage(bmp);

                start_but.Background = Brushes.Red;

                if (isParametersReady())
                {
                    if (ArgumentBox.Text.Length == 0)
                        ArgumentBox.Text = GetArgument();

                    initializeFFmpeg();
                }
                else
                    ArgumentBox.Text = "Parameters Error!";
            }
            else   //stop capturing
            {
                start_but.Background = Brushes.GhostWhite;

                finishFFmpeg();

                //reset argumentbox
                ArgumentBox.Text = "";

                //reset the record button
                if (isStarted)
                {
                    isStarted = false;
                    stop_but.Background = Brushes.GhostWhite;
                }
            }
            isCapturing = !isCapturing;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Record_thread.Abort();
            UI_thread.Abort();
        }
    }
}
