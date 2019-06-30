using System.Windows;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using Bruch = System.Windows.Media.Brush;

namespace DRnamespace
{
    public partial class AreaWind : Window
    {
        System.Drawing.Point mousePos_down;
        bool lock_size = false;
        double width, height, top, left;
        double p_x = 0.0, p_y = 0.0, w, h, l, t;

        Thread resize;

        Bruch LockColor, UnlockColor;

        public AreaWind()
        {
            InitializeComponent();

            LockColor = (Brush)FindResource("Red");
            UnlockColor = (Brush)FindResource("Background");

            Visibility = Visibility.Visible;
            Hide();
            ShowInTaskbar = false;

            width = Width;
            height = Height;

            Right.MouseDown += (s, e) => mouseDown(s,e);
            Left_.MouseDown += (s, e) => mouseDown(s, e);
            Up.MouseDown += (s, e) => mouseDown(s, e);
            Down.MouseDown += (s, e) => mouseDown(s, e);
        }

        public void setProportion(double x,double y)
        {
            if (!lock_size)
            {
                p_x = x;
                p_y = y;

                if (x != 0.0)
                    Width = Height * p_x / p_y;
                else
                {
                    Width = width;
                    Height = height;
                }
            }
        }

        public void LockSize()
        {
            lock_size = true;
            Right.Background = LockColor;
            Left_.Background = LockColor;
            Up.Background = LockColor;
            Down.Background = LockColor;

            Up.Cursor = Cursors.Arrow;
            Down.Cursor = Cursors.Arrow;
            Left_.Cursor = Cursors.Arrow;
            Right.Cursor = Cursors.Arrow;
        }

        public void UnlockSize()
        {
            lock_size = false;
            Right.Background = UnlockColor;
            Left_.Background = UnlockColor;
            Up.Background = UnlockColor;
            Down.Background = UnlockColor;

            Up.Cursor = Cursors.SizeNS;
            Down.Cursor = Cursors.SizeNS;
            Left_.Cursor = Cursors.SizeWE;
            Right.Cursor = Cursors.SizeWE;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (resize != null)
                resize.Abort();
        }

        private void Center_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        
        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!lock_size)
            {
                if (resize != null && resize.IsAlive)
                    resize.Join();

                mousePos_down = System.Windows.Forms.Control.MousePosition;
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
            else
                DragMove();
        }

        private void RightMove()
        {
            while (System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                w = width + mp().X - mousePos_down.X;
                w = w < 0 ? 0 : w;

                if (p_x != 0.0)
                    h = w * p_y / p_x;

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
                h = height + mp().Y - mousePos_down.Y;
                h = h < 0 ? 0 : h;

                if (p_x != 0.0)
                    w = h * p_x / p_y;

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
                my = mp().Y;

                h = height - my + mousePos_down.Y;
                h = h < 0 ? 0 : h;
                t = top + my - mousePos_down.Y;

                if (p_x != 0.0)
                {
                    w = h * p_x / p_y;
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
                mx = mp().X;

                w = width - mx + mousePos_down.X;
                w = w < 0 ? 0 : w;
                l = left + mx - mousePos_down.X;

                if (p_x != 0.0)
                {
                    h = w * p_y / p_x;
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

        private System.Drawing.Point mp()
        {
            return System.Windows.Forms.Control.MousePosition;
        }
    }
}
