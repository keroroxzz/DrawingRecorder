/*
 * Author : RTU(keroroxzz)
 * Last modify : 2020/8/11
 * 
 * New class in charge of dragging
 */

using System;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using MouseButtons = System.Windows.Forms.MouseButtons;

using static DRnamespace.GlobalFunctions;

namespace DRnamespace
{
    class MyDragable
    {
        bool
            Draging = false,
            IsMouseDown = false,
            Enabled = true;

        Point relativePosition;
        double DragThreshold;

        Thread Drag_thread;
        readonly Window window;
        readonly Func<bool> Inject;

        //constructer===========================================================
        public MyDragable(Window w, double dt = 5.0, Func<bool> ij = null)
        {
            window = w;
            Inject = ij;
            DragThreshold = dt;
        }

        //public func===========================================================
        public void StartDragThread()
        {
            Drag_thread = new Thread(MyDragMove);
            Drag_thread.Start();
        }

        public bool IsDragging()
        {
            return Draging;
        }

        public void Abort()
        {
            Drag_thread.Abort();
        }

        public void Enable()
        {
            Enabled = true;
        }
        public void Disnable()
        {
            Enabled = false;
        }

        //callback===========================================================
        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            relativePosition = e.GetPosition(window);
        }
        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (GetMouseButtons() == MouseButtons.None)
                IsMouseDown = false;

            if (!Draging && IsMouseDown && Enabled)
            {
                if (( e.GetPosition(window) - relativePosition ).Length > DragThreshold)
                {
                    Draging = true;
                    relativePosition = e.GetPosition(window);
                }
            }
        }

        //core===========================================================
        void MyDragMove()
        {
            while (true)
            {
                if (Draging && System.Windows.Forms.Control.MouseButtons != MouseButtons.Left)
                    Draging = false;
                else if (Draging)
                {
                    var mp = MousePosition();
                    window.Dispatcher.Invoke(new Action(() =>
                    {
                        window.Left = -relativePosition.X + mp.X;
                        window.Top = -relativePosition.Y + mp.Y;

                        Inject?.Invoke();
                    }));
                }

                SpinWait.SpinUntil(() => false, Draging ? 8 : 100);
            }
        }
    }
}
