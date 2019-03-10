using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Brushes = System.Windows.Media.Brushes;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //UI
        double ToolBarHeight = 15.0,
        WindOpacity = 1.0,
        SettingBarHeight = 0.0;
        String mouseEnterP = "", mouseEnter = "";

        void UIthread()
        {
            while (true)
            {
                //bar controlling
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
                    this.Opacity = (WindOpacity + this.Opacity) * 0.5;

                //prevent maximizing
                if (this.WindowState == WindowState.Maximized)
                        this.WindowState = WindowState.Normal;


                    CheckScreenDif = checkBox.IsChecked.Value;
                    isTargetLock = TargetLock.IsChecked.Value;

                    try
                    {
                        CapInt = (int)(Convert.ToDouble(IntervalBox.Text) * 1000);
                    }
                    catch { }

                    //get window information
                    if (isCapturing) 
                    {
                        if (isStarted)
                        {
                            ActiveName = GetProcessNameOfForegroundWindows();

                            if (TargetName == ActiveName)
                                stop_but.Background = Brushes.Red;
                            else
                                stop_but.Background = Brushes.IndianRed;
                        }

                        TargetTB.Text = TargetName;
                    }
                    else TargetName = GetProcessNameOfNextWindows();
                };
                Dispatcher.Invoke(dele);

                Thread.Sleep(30);
            }
        }

        void LockTextBoxs()
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


        private void stop_but_Click(object sender, RoutedEventArgs e)
        {
            if (isCapturing && TargetName != "DrawingRecorder")
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
            if (WindOpacity != 0.0)
                WindOpacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (WindOpacity != 0.0)
                WindOpacity = 0.75;
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {

            WindOpacity = 0.0;

            void wait()
            {
                Thread.Sleep(800);
                System.Environment.Exit(1);
            }

            Thread Closing = new Thread(wait);

            Closing.Start();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToolBarHeight = 40.0;
            SettingBarHeight = 100.0;
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


        private void Start_but_Click(object sender, RoutedEventArgs e)
        {
            if (!isCapturing)   //start capturing
            {
                intializeGraph();

                start_but.Background = Brushes.Red;

                Width = System.Windows.SystemParameters.PrimaryScreenWidth / 7.0;
                Height = (float)height / (float)width * Width;

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
                TargetTB.Text = "";

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