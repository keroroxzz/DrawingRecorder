/*
 * Author : RTU(keroroxzz)
 * Last modify : 2020/8/27
 * 
 * Extent content of MainWindow, for UI stuff.
 * 
 * Main window is always top when area window is active.
 * Fix dragging when highlighting textboxes
 * The proportion button will be locked when the area window is hidden.
 * Exit button will be locked when capturing.
 */

using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using static DRnamespace.GlobalFunctions;

using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;


namespace DRnamespace
{
    public partial class MainWindow : Window
    {
        double
            WindowsOpacity = 1.0,
            ToolBarHeight = 60.0,
            SettingBarHeight = 0.0,
            DragThreshold = 10.0;

        MyDragable Drag;
        Thread UI_thread;

        void UiInit()
        {
            Trace.WriteLine("UI is initializing.");

            UI_thread = new Thread(UIthread);
            UI_thread.Start();

            Drag = new MyDragable(this, DragThreshold);
            Drag.StartDragThread();
        }

        void UIthread()
        {
            Trace.WriteLine("UIthread starts.");
            bool wasRecording = false;

            System.Drawing.Point MouseP = System.Windows.Forms.Control.MousePosition;

            while (true)
            {
                Action dele = delegate ()
                {
                    UpdateUI();

                    //prevent maximizing
                    if (this.WindowState != WindowState.Maximized)
                        this.WindowState = WindowState.Normal;

                    //get window information
                    if (recorder.Capturing())
                    {
                        if (ExitButton.IsEnabled)
                            ExitButton.IsEnabled = false;

                        if (displayer.Visibility == Visibility.Visible)
                        {
                            graph.UpdateIntPtr();
                            if (graph.GetIntPtr() != IntPtr.Zero)
                                try
                                {
                                    displayer.ImageBox.Source = Imaging.CreateBitmapSourceFromHBitmap(graph.GetIntPtr(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                    graph.DeleteIntPtr();
                                }
                                catch { }
                        }

                        if (recorder.Recording())
                            if (appmani.IsTargetActive())
                            {
                                RecordButton.Background = Brushes.Red;

                                /*if (ffmpeg.IsWrited())
                                    notify.Icon = Properties.Resources.notify_writing;
                                else
                                    notify.Icon = Properties.Resources.notify_capturing;*/
                                notify.Icon = Properties.Resources.notify_capturing;

                                if (!wasRecording)
                                {
                                    recorder.PlayStartSound();
                                    wasRecording = true;
                                }
                            }
                            else
                            {
                                RecordButton.Background = Brushes.IndianRed;
                                notify.Icon = Properties.Resources.notify_norm;

                                if (wasRecording)
                                {
                                    recorder.PlayStopSound();
                                    wasRecording = false;
                                }
                            }
                    }
                    else
                    {
                        if (!appmani.Lock())
                            appmani.SetTargetToNextWindow();

                        if (!ExitButton.IsEnabled)
                            ExitButton.IsEnabled = true;
                    }

                    if (area_window.IsVisible)
                    {
                        Topmost = false;
                        Topmost = true;
                    }
                    else if (Topmost)
                        Topmost = false;

                    MouseP = System.Windows.Forms.Control.MousePosition;
                };
                Dispatcher.Invoke(dele);

                SpinWait.SpinUntil(() => false, 20);
            }
        }

        void UpdateUI()
        {
            tool_bar.Height = new GridLength((tool_bar.Height.Value + ToolBarHeight) * 0.5);
            setting_bar.Height = new GridLength((setting_bar.Height.Value + SettingBarHeight) * 0.5);
            this.Opacity = (WindowsOpacity + this.Opacity) * 0.5;
            area_window.Opacity = (1.0 - WindowsOpacity + area_window.Opacity) * 0.5 + 0.4;
        }

        void LockTextBoxs()
        {
            FileNameBox.Background = (Brush)FindResource("TextBoxLocked_Color");
            DirectoryBox.Background = (Brush)FindResource("TextBoxLocked_Color");
            FrameRateBox.Background = (Brush)FindResource("TextBoxLocked_Color");
            ArgumentBox.Background = (Brush)FindResource("TextBoxLocked_Color");

            FileNameBox.IsReadOnly = FrameRateBox.IsReadOnly = ArgumentBox.IsReadOnly = DirectoryBox.IsReadOnly = true;
        }

        private void UnlockTextBoxs()
        {
            FileNameBox.Background = (Brush)FindResource("TextBox_Color");
            DirectoryBox.Background = (Brush)FindResource("TextBox_Color");
            FrameRateBox.Background = (Brush)FindResource("TextBox_Color");
            ArgumentBox.Background = (Brush)FindResource("TextBox_Color");

            FileNameBox.IsReadOnly = FrameRateBox.IsReadOnly = ArgumentBox.IsReadOnly = DirectoryBox.IsReadOnly = false;
        }

        //====================================================ToolBar=============================================================

        private void DragBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && e.ChangedButton == MouseButton.Right)
                if (displayer.IsVisible)
                {
                    displayer.Hide();
                }
                else
                {
                    displayer.Show();
                    displayer.Activate();
                }
        }

        private void BarChanger_Click(object sender, RoutedEventArgs e)
        {
            if (Drag.IsDragging())
                return;

            if (ToolBarHeight == 60.0)
            {
                ToolBarHeight = 0.0;
                SettingBarHeight = 60.0;
            }
            else
            {
                ToolBarHeight = 60.0;
                SettingBarHeight = 0.0;
            }
        }

        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {
            if (Drag.IsDragging())
                return;

            if (area_window.Activity())
            {
                AreaButton.Background = Brushes.GhostWhite;
                area_window.SetActivity(false);
            }
            else
            {
                AreaButton.Background = Brushes.Red;
                area_window.SetActivity(true);
            }
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (Drag.IsDragging())
                return;

            RefreshDpiScaling();

            if (!recorder.Capturing())   //start capturing
            {
                if (CreateArgument())
                {
                    if (area_window.Activity())
                    {
                        AreaButton.Background = Brushes.GhostWhite;
                        area_window.Hide();
                    }

                    if (ArgumentBox.Text == "")
                        FileNameBox_TextChanged(sender, null);

                    graph.Initial(area_window.Area(), area_window.IsFull());

                    area_window.LockSize();

                    LockTextBoxs();
                    CreateFolder();
                    ffmpeg.Startup(ArgumentBox.Text);

                    recorder.StartCapturing();

                    Trace.WriteLine("Capturing started.");
                }
                else
                    ArgumentBox.Text = "Parameters Error!";

            }
            else   //stop capturing
            {
                notify.Icon = Properties.Resources.notify_norm;

                UnlockTextBoxs();
                ffmpeg.Finish();

                area_window.UnlockSize();

                //reset argumentbox
                ArgumentBox.Text = "";

                //reset the record button
                if (recorder.Recording())
                    recorder.StopRecording();

                recorder.StopCapturing();

                Trace.WriteLine("Capturing stopped.");
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {

            if (Drag.IsDragging())
                return;

            if (recorder.Capturing())
            {
                if (!recorder.Recording())
                {
                    recorder.StartRecording();

                    if(area_window.IsVisible)
                    {
                        AreaButton.Background = Brushes.GhostWhite;
                        area_window.Hide();
                    }
                }
                else
                    recorder.StopRecording();

                Trace.WriteLine("Record : " + recorder.Recording());
            }
        }

        string[] proportion_text = { "None", "4:3", "16:9", "3:4", "9:16", "full" };
        int proportion_index = 0;

        private void ProportionButtion_Click(object sender, RoutedEventArgs e)
        {
            if (Drag.IsDragging() || !area_window.Activity())
                return;

            if (!recorder.Capturing())
            {
                proportion_index = ( proportion_index + 1 ) % proportion_text.Length;

                ProportionButtion.Content = proportion_text[proportion_index];

                switch (proportion_index)
                {
                    case 0: area_window.SetProportion(0.0); break;
                    case 1: area_window.SetProportion(4.0/3.0); break;
                    case 2: area_window.SetProportion(16.0/9.0); break;
                    case 3: area_window.SetProportion(3.0/4.0); break;
                    case 4: area_window.SetProportion(9.0 / 16.0); break;
                    case 5: area_window.SetProportion(-1.0); break;
                }
            }
        }


        private void Lock_Button_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //Lock_Button.Content = appmani.NextProcess();
        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            if (Drag.IsDragging())
                return;

            if (appmani.Lock())
            {
                appmani.Unlock();
                Lock_Button.Background = Brushes.GhostWhite;
                Lock_Button.Content = "";
            }
            else
            {
                appmani.SetTargetToNextWindow();
                if (appmani.IsTargetAvalible())
                {
                    appmani.ActiveLock();
                    Lock_Button.Content = appmani.Target();
                    Lock_Button.Background = Brushes.Red;
                }
                else
                {
                    Lock_Button.Content = "";
                    notify.ShowBalloonTip(300, "", "Switch to a target app and switch back to Drawing Recorder to set the target.", System.Windows.Forms.ToolTipIcon.Info);
                }
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            ShowInTaskbar = false;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Exit app.");

            WindowsOpacity = 0.0;
            displayer.Close();
            area_window.Close();

            void wait()
            {
                SpinWait.SpinUntil(() => false, 600);

                Dispatcher.Invoke(new Action(() => {
                    this.Close();
                }));
            }
            new Thread(wait).Start();
        }

        //=========================================================================================================================

        private void ControlBoard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Drag.MouseDown(sender, e);
        }

        private void ControlBoard_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Drag.MouseMove(sender, e);
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (WindowsOpacity != 0.0)
                WindowsOpacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (WindowsOpacity != 0.0)
                WindowsOpacity = 0.75;
        }
        private void FrameRateBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(!recorder.Capturing())
                FrameRateBox.Text = (Convert.ToInt32(FrameRateBox.Text) + (e.Delta > 0 ? 1:-1)).ToString();
        }
        private void setInterval()
        {
            if (recorder != null && SpeedBox.Text.Length > 0)
            {
                recorder.SetCaptureInterval((int)(Convert.ToDouble(SpeedBox.Text) / Convert.ToDouble(FrameRateBox.Text) * 1000.0));
                recorder.framerate = Convert.ToDouble(FrameRateBox.Text);
                recorder.speed = Convert.ToDouble(SpeedBox.Text);
            }

        }

        private void SpeedBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double speed = Convert.ToDouble(SpeedBox.Text) + (e.Delta > 0 ? 0.2 : -0.2);
            SpeedBox.Text = (speed < 0.2 ? 0.2 : speed).ToString();
        }
        private void FrameRateBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (FrameRateBox != null && FrameRateBox.Text.Length > 0)
            {
                CreateArgument();
                setInterval();
            }
        }

        private void SpeedBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setInterval();
        }

        private void FileNameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CreateArgument();
        }

        private void CreateFolder()
        {
            if (DirectoryBox != null)
                if (!Directory.Exists(DirectoryBox.Text))
                    Directory.CreateDirectory(DirectoryBox.Text);
        }

        private string ModifiedFileName()
        {
            string name = FileNameBox.Text;

            if (name.EndsWith("_"))
            {
                DateTime time = DateTime.Now;
                name += time.ToShortDateString().Replace('/','_') + "-" + time.Hour + "_" + time.Minute;
            }

            if (DirectoryBox.Text.Length > 0)
                name = DirectoryBox.Text + "/" + name;

            string result = name;

            int i = 0;
            while (File.Exists(result + ".mp4"))
                result = name + "_" + i++;

            return result + ".mp4";
        }
        
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (compare != null && sender == DC_box)
                compare.ActiveDectect();

            if (recorder != null && sender == MD_box)
                recorder.MouseChecking(true);

            if (recorder != null && sender == RM_box)
                recorder.RecordMouse(true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (compare != null && sender == DC_box)
                compare.StopDectect();

            if (recorder != null && sender == MD_box)
                recorder.MouseChecking(false);

            if (recorder != null && sender == RM_box)
                recorder.RecordMouse(false);
        }

        private void TextBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            Drag.Disnable();
        }
        private void TextBoxMouseUp(object sender, MouseButtonEventArgs e)
        {
            Drag.Enable();
        }

        string ffmpegArgs = "";
        private bool CreateArgument()
        {
            if (ArgumentBox != null && FileNameBox != null && FrameRateBox != null && DirectoryBox != null)
            {
                if (FileNameBox.Text.Length > 0 && FrameRateBox.Text.Length > 0 && SpeedBox.Text.Length > 0)
                {
                    ArgumentBox.Text = ffmpegArgs.Replace("{frameRate}", FrameRateBox.Text).Replace("{fileName}", ModifiedFileName());
                    return true;
                }
                Trace.WriteLine("Argument wrong. " + FileNameBox.Text + "/" + FrameRateBox.Text + "/" + SpeedBox.Text);
            }
            return false;
        }
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryBox.Text.Length > 0 && Directory.Exists(DirectoryBox.Text))
                Process.Start(Environment.CurrentDirectory + "\\" + DirectoryBox.Text);
            else
                Process.Start(Environment.CurrentDirectory);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notify.Visible = false;

            try
            {
                recorder.Abort();
                UI_thread.Abort();
                Drag.Abort();
            }
            catch(ThreadAbortException exp)
            {
                Trace.WriteLine("Abort thread... : " + exp.Message);
            }

            IniManip inimanip = new IniManip("/settings.ini");
            inimanip.Write("Settings", "FrameRate", FrameRateBox.Text);
            inimanip.Write("Settings", "Speed", SpeedBox.Text);
            inimanip.Write("Settings", "DetectScreenChange", DC_box.IsChecked.ToString());
            inimanip.Write("Settings", "DetectMouseDown", MD_box.IsChecked.ToString());
            inimanip.Write("Settings", "RecordMouse", RM_box.IsChecked.ToString());
            inimanip.Write("Settings", "FileName", FileNameBox.Text);
            inimanip.Write("Settings", "OutputDirectory", DirectoryBox.Text);

            inimanip.Write("CaptureArea", "X", ((int)area_window.Left).ToString());
            inimanip.Write("CaptureArea", "Y", ((int)area_window.Top).ToString());
            inimanip.Write("CaptureArea", "Width", ((int)area_window.Width).ToString());
            inimanip.Write("CaptureArea", "Height", ((int)area_window.Height).ToString());

            Trace.WriteLine("Closing process finished.");
        }
    }
}