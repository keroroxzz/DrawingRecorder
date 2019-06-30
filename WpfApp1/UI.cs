using System;
using System.Threading;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;


using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace DRnamespace
{
    public partial class MainWindow : Window
    {
        double
            WindOpacity = 1.0,
            ToolBarHeight = 60.0,
            SettingBarHeight = 0.0;

        void UIthread()
        {
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
                        if (graph.GetIntPtr() != IntPtr.Zero)
                            try
                            {
                                displayer.ImageBox.Source = Imaging.CreateBitmapSourceFromHBitmap(graph.GetIntPtr(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                graph.DeleteIntPtr();
                            }
                            catch { }

                        if (area_window.IsVisible)
                            graph.Shift(area_window.Left, area_window.Top);

                        if (recorder.Recording())
                            if (appmani.IsTargetActive())
                            {
                                RecordButton.Background = Brushes.Red;

                                if (ffmpeg.IsWrited())
                                    notify.Icon = Properties.Resources.notify_writing;
                                else
                                    notify.Icon = Properties.Resources.notify_capturing;
                            }
                            else
                            {
                                RecordButton.Background = Brushes.IndianRed;
                                notify.Icon = Properties.Resources.notify_norm;
                            }
                        }
                    else if(!appmani.Lock())
                        appmani.SetTargetToNextWindow();
                };
                Dispatcher.Invoke(dele);

                //Thread.Sleep(65);
                SpinWait.SpinUntil(() => false, 65);
            }
        }

        void UpdateUI()
        {
            tool_bar.Height = new GridLength((tool_bar.Height.Value + ToolBarHeight) * 0.5);
            setting_bar.Height = new GridLength((setting_bar.Height.Value + SettingBarHeight) * 0.5);
            this.Opacity = (WindOpacity + this.Opacity) * 0.5;
            area_window.Opacity = (1.0 - WindOpacity + area_window.Opacity) * 0.5 + 0.4;
        }

        void LockTextBoxs()
        {
            FileNameBox.Background = (Brush)FindResource("TextBoxLocked_Color");
            FrameRateBox.Background = (Brush)FindResource("TextBoxLocked_Color");
            ArgumentBox.Background = (Brush)FindResource("TextBoxLocked_Color");

            FileNameBox.IsReadOnly = FrameRateBox.IsReadOnly = ArgumentBox.IsReadOnly = true;
        }

        private void UnlockTextBoxs()
        {
            FileNameBox.Background = (Brush)FindResource("TextBox_Color");
            FrameRateBox.Background = (Brush)FindResource("TextBox_Color");
            ArgumentBox.Background = (Brush)FindResource("TextBox_Color");

            FileNameBox.IsReadOnly = FrameRateBox.IsReadOnly = ArgumentBox.IsReadOnly = false;
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
            if (area_window.IsVisible)
            {
                AreaButton.Background = Brushes.GhostWhite;
                area_window.Hide();
            }
            else
            {
                AreaButton.Background = Brushes.Red;
                area_window.Show();
                area_window.Activate();
            }
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (!recorder.Capturing())   //start capturing
            {
                if (isParametersReady())
                {
                    if (area_window.IsVisible)
                    {
                        AreaButton.Background = Brushes.GhostWhite;
                        area_window.Hide();
                    }

                    if (ArgumentBox.Text == "")
                        FileNameBox_TextChanged(sender, null);

                    graph.Initial(area_window.Left, area_window.Top, area_window.Width, area_window.Height);

                    area_window.LockSize();

                    LockTextBoxs();
                    ffmpeg.Startup(ArgumentBox.Text);

                    recorder.StartCapturing();
                }
                else
                    ArgumentBox.Text = "Parameters Error!";
            }
            else   //stop capturing
            {
                notify.Icon = Properties.Resources.notify_norm;

                UnlockTextBoxs();
                ffmpeg.finish();

                area_window.UnlockSize();

                //reset argumentbox
                ArgumentBox.Text = "";

                //reset the record button
                if (recorder.Recording())
                    recorder.StopRecording();

                recorder.StopCapturing();
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
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
            }
        }

        string[] proportion_text = { "None", "4:3", "16:9", "3:4", "9:16" };
        int proportion_index = 0;

        private void ProportionButtion_Click(object sender, RoutedEventArgs e)
        {
            if (!recorder.Capturing())
            {
                proportion_index = proportion_index == 4 ? 0 : proportion_index + 1;

                ProportionButtion.Content = proportion_text[proportion_index];

                switch (proportion_index)
                {
                    case 0: area_window.setProportion(0.0, 0.0); break;
                    case 1: area_window.setProportion(4.0, 3.0); break;
                    case 2: area_window.setProportion(16.0, 9.0); break;
                    case 3: area_window.setProportion(3.0, 4.0); break;
                    case 4: area_window.setProportion(9.0, 16.0); break;
                }
            }
        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
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
                    notify.ShowBalloonTip(300, "", "Unavaliable Target!", System.Windows.Forms.ToolTipIcon.Info);
                }
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            ShowInTaskbar = false;
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            WindOpacity = 0.0;
            displayer.Close();
            area_window.Close();

            void wait()
            {
                Thread.Sleep(600);

                Action del = delegate () { this.Close(); };
                Dispatcher.Invoke(del);
            }
            new Thread(wait).Start();
        }

        //=========================================================================================================================

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (WindOpacity != 0.0)
                WindOpacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (WindOpacity != 0.0)
                WindOpacity = 0.75;
        }

        private void FrameRateBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CreateArgument();
        }

        private void IntervalBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (recorder != null)
                recorder.SetCaptureInterval((int)(Convert.ToDouble(IntervalBox.Text) * 1000));
        }

        private void FileNameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (FileNameBox != null)
            {
                if (File.Exists(FileNameBox.Text + ".mp4"))
                {
                    int i = 0;
                    for (; File.Exists(FileNameBox.Text + "_" + i + ".mp4"); i++) ;
                    FileNameBox.Text = FileNameBox.Text + "_" + i;
                }

                CreateArgument();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (graph != null)
                graph.ActiveDectect();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (graph != null)
                graph.StopDectect();
        }

        private void CreateArgument()
        {
            if (ArgumentBox != null && FileNameBox != null && FrameRateBox != null)
                ArgumentBox.Text = "-framerate " + FrameRateBox.Text + " -i - -c:v libx264 -vf format=yuv420p -r " + FrameRateBox.Text + "  " + FileNameBox.Text + ".mp4";
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
                return true;
            }
            return false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notify.Visible = false;
            recorder.Abort();
            UI_thread.Abort();
        }
    }
}