/*
 * Author : RTU(keroroxzz)
 * Last modify : 2020/9/7
 * 
 * Add new drag to fix dismatching of area when dragging the area
 * Grip resize add when proportion unset
 * Add a sizing bar
 * 
 * fix sizing problem
 */

using System;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Button = System.Windows.Controls.Button;

using static DRnamespace.GlobalFunctions;
using Screen = System.Windows.Forms.Screen;

namespace DRnamespace
{
    public partial class AreaWind : Window
    {
        System.Drawing.Point mousePos_down;
        bool lock_size = false, active = false;
        double width, height, top, left;
        double p = 0.0, w, h, l, t;
        Rect area, none_size;

        Thread resize;

        //Brush LockColor, UnlockColor;

        Button AreaButton;

        MyDragable Drag;

        MainWindow mw;

        public AreaWind(Button button, MainWindow mw_)
        {
            InitializeComponent();

            AreaButton = button;
            mw = mw_;

            //LockColor = (Brush)FindResource("Red");
            //UnlockColor = (Brush)FindResource("Background");

            Hide();
            ShowInTaskbar = false;

            Right.MouseDown += (s, e) => Resizing(s,e);
            Left_.MouseDown += (s, e) => Resizing(s, e);
            Up.MouseDown += (s, e) => Resizing(s, e);
            Down.MouseDown += (s, e) => Resizing(s, e);

            Up.Cursor = Cursors.SizeNS;
            Down.Cursor = Cursors.SizeNS;
            Left_.Cursor = Cursors.SizeWE;
            Right.Cursor = Cursors.SizeWE;

            Drag = new MyDragable(this, 0.0, SetShift);
            Drag.StartDragThread();
        }

        public void SetProportion(double xdy)
        {
            if (!lock_size)
            {
                p = xdy;

                if (p > 0.0)
                {
                    Width = Height * p;

                    ResizeMode = ResizeMode.NoResize;
                    Show();
                }
                else if (p == 0.0)
                {
                    Width = none_size.Width;
                    Height = none_size.Height;
                    ResizeMode = ResizeMode.CanResizeWithGrip;
                    Show();
                }
                else if (p < 0.0)
                {
                    var wa = Screen.PrimaryScreen.WorkingArea;
                    SetArea(0, 0, wa.Width, wa.Height);

                     ResizeMode = ResizeMode.NoResize;
                    Hide();
                }
            }
        }

        public Rect Area()
        { 
            return area;
        }

        public bool IsFull()
        {
            return p < 0.0;
        }

        public new void Show()
        {
            if (active)
                Visibility = Visibility.Visible;
        }

        public void SetActivity(bool act)
        {
            active = act;
            Visibility = (active && p >= 0.0) ? Visibility.Visible : Visibility.Hidden;
        }
        public bool Activity()
        {
            return active;
        }

        public void LockSize()
        {
            lock_size = true;
            SetResizeBaeState(false);
        }

        private void AreaExit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            AreaButton.Background = Brushes.GhostWhite;
        }


        public void UnlockSize()
        {
            lock_size = false;
            SetResizeBaeState(true);
        }

        void SetResizeBaeState(bool isenable, Brush color = null)
        {
            SizeBar.IsEnabled=
            Right.IsEnabled =
            Left_.IsEnabled =
            Up.IsEnabled =
            Down.IsEnabled = isenable;

            SizeBar.Visibility =
            Right.Visibility =
            Left_.Visibility =
            Up.Visibility =
            Down.Visibility = isenable ? Visibility.Visible : Visibility.Hidden;


            ResizeMode = isenable? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;

            if (color != null)
                Right.Background = 
                Left_.Background = 
                Up.Background = 
                Down.Background = color;
        }

        private void SetArea(int x = -1, int y = -1, int w = 0, int h = 0)
        {
            area = new Rect(x < 0 ? Left : x, y < 0 ? Top : y, w > 0 ? w : Width, h > 0 ? h : Height);

            if (p == 0.0)
            {
                none_size.Width = w > 0 ? w : Width;
                none_size.Height = h > 0 ? h : Height;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (resize != null)
                resize.Abort();
            if (Drag != null)
                Drag.Abort();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetArea();

            WidthBox.Text = area.Width.ToString();
            HeightBox.Text = area.Height.ToString();
        }

        private void TypeInSize(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                w = Convert.ToInt32(WidthBox.Text);
                h = Convert.ToInt32(HeightBox.Text);

                if (p > 0.0)
                    if (sender == WidthBox)
                        h = w / p;
                    else
                        w = h * p;

                Width = w;
                Height = h;
            }
            else if (e.Key == Key.Up)
                Increment(sender, 1);
            else if (e.Key == Key.Down)
                Increment(sender, -1);
        }

        private void Increment(object sender, int amount)
        {
            w = Width;
            h = Height;

            if (sender == WidthBox)
                w += amount;
            else if (sender == HeightBox)
                h += amount;

            if (p > 0.0)
                if (sender == WidthBox)
                    h = w / p;
                else
                    w = h * p;

            Width = w;
            Height = h;
        }

        private void MouseWheelResize(object sender, MouseWheelEventArgs e)
        {
            Increment(sender, e.Delta/10);
        }

        private void Center_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (p >= 0.0)
                Drag.MouseDown(sender, e);
        }
        private void Center_MouseMove(object sender, MouseEventArgs e)
        {
            if (p >= 0.0)
                Drag.MouseMove(sender, e);
        }

        private bool SetShift()
        {
            SetArea();
            if (p >= 0.0)
                mw.graph.Shift(Left, Top);
            return true;
        }

        private void Resizing(object sender, MouseButtonEventArgs e)
        {
            if (!lock_size && p >= 0.0)
            {
                if (resize != null && resize.IsAlive)
                    resize.Join();

                mousePos_down = MousePosition();
                width = Width;
                height = Height;
                top = Top;
                left = Left;

                w = Width;
                h = Height;
                t = Top;
                l = Left;

                if (sender == Right)
                    resize = new Thread(RightMove);
                else if (sender == Left_)
                    resize = new Thread(LeftMove);
                else if (sender == Up)
                    resize = new Thread(UpMove);
                else if (sender == Down)
                    resize = new Thread(DownMove);

                resize.Start();
            }
        }

        private void RightMove()
        {
            while (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                w = width + MousePosition().X - mousePos_down.X;
                w = w < 0 ? 0 : w;

                if (p > 0.0)
                    h = w / p;

                Dispatcher.Invoke(
                    delegate ()
                    {
                        Width = (Width + w) / 2.0;
                        Height = (Height + h) / 2.0;
                    });

                SpinWait.SpinUntil(() => false, 1);
            }
        }

        private void DownMove()
        {
            while (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                h = height + MousePosition().Y - mousePos_down.Y;
                h = h < 0 ? 0 : h;

                if (p > 0.0)
                    w = h * p;

                Dispatcher.Invoke(
                    delegate ()
                    {
                        Width = (Width + w) / 2.0;
                        Height = (Height + h) / 2.0;

                    });

                SpinWait.SpinUntil(() => false, 1);
            }
        }

        private void UpMove()
        {
            double my;
            while (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                my = MousePosition().Y;

                h = height - my + mousePos_down.Y;
                h = h < 0 ? 0 : h;
                t = top + my - mousePos_down.Y;

                if (p > 0.0)
                {
                    w = h * p;
                    l = left - w + width;
                }

                Dispatcher.Invoke(
                    delegate ()
                    {
                        Left = (Left + l) / 2.0;
                        Width = (Width + w) / 2.0;
                        Top = (Top + t) / 2.0;
                        Height = (Height + h) / 2.0;
                    });

                SpinWait.SpinUntil(() => false, 1);
            }
        }

        private void LeftMove()
        {
            double mx;
            while (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                mx = MousePosition().X;

                w = width - mx + mousePos_down.X;
                w = w < 0 ? 0 : w;
                l = left + mx - mousePos_down.X;

                if (p > 0.0)
                {
                    h = w / p;
                    t = top - h + height;
                }

                Dispatcher.Invoke(
                    delegate ()
                    {
                        Left = (Left + l) / 2.0;
                        Width = (Width + w) / 2.0;
                        Top = (Top + t) / 2.0;
                        Height = (Height + h) / 2.0;
                    });

                SpinWait.SpinUntil(() => false, 1);
            }
        }
    }
}
