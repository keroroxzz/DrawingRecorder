using System;
using System.Threading;
using System.Windows.Controls;
using Brushes = System.Windows.Media.Brushes;

namespace DRnamespace
{
    class Recorder
    {
        Thread RecordThread;

        //booleans
        private Boolean isCapturing;
        private Boolean isRecording;

        int CapInt;

        AppManager appfind;
        FFmpeg ffmpeg;
        Graph graph;

        Button RecBut, CapBut;
        System.Windows.Forms.NotifyIcon NotifIco;

        public Recorder(AppManager app, FFmpeg ffm, Graph gra, Button cb, Button rb, System.Windows.Forms.NotifyIcon nf)
        {
            appfind = app;
            ffmpeg = ffm;
            graph = gra;
            CapBut = cb;
            RecBut = rb;
            NotifIco = nf;
        }

        public bool Recording()
        {
            return isRecording;
        }

        public void StartRecording()
        {
            if (isCapturing)
            {
                isRecording = true;
                NotifIco.Icon = DRnamespace.Properties.Resources.notify_capturing;
            }
        }

        public void StopRecording()
        {
            isRecording = false;
            RecBut.Background = Brushes.GhostWhite;
            NotifIco.Icon = DRnamespace.Properties.Resources.notify_norm;
        }

        public bool Capturing()
        {
            return isCapturing;
        }

        public void StartCapturing()
        {
            isCapturing = true;
            CapBut.Background = Brushes.Red;
        }

        public void StopCapturing()
        {
            isCapturing = false;
            CapBut.Background = Brushes.GhostWhite;
        }

        public void SetCaptureInterval(int t)
        {
            CapInt = t < 1 ? 1 : t;
        }

        public void Start()
        {
            RecordThread = new Thread(Loop);
            RecordThread.Start();
        }

        public void Abort()
        {
            RecordThread.Abort();
        }

        private void Loop()
        {
            while (true)
            {
                if (isCapturing && (appfind.IsTargetActive() || !isRecording))
                {
                    graph.Capture();

                    if (graph.IsScreenChanged() && (appfind.IsTargetActive() || !isRecording))
                    {
                        graph.UpdatePrevious();
                        if (isRecording)
                            ffmpeg.Write(graph.BmpToByte());

                        graph.UpdateIntPtr();

                        SpinWait.SpinUntil(() => false, CapInt);
                    }
                    else
                        SpinWait.SpinUntil(() => false, 500);
                }
                else
                    SpinWait.SpinUntil(() => false, 500);
            }
        }
    }
}
