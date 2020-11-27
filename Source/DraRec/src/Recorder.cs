/*
 * Author : Brian Tu (RTU)
 * Last modify : -
 * 
 * The Recorder manages the recording thread.
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

using static DRnamespace.GlobalFunctions;

namespace DRnamespace
{
    class Recorder
    {
        Thread RecordThread;

        //booleans
        private bool isCapturing;
        private bool isRecording;

        bool mouseChecking = true, recMouse=true;

        int CapInt;

        MainWindow mw;

        private MediaPlayer StartSound, StopSound;

        public Recorder(MainWindow mw_)
        {
            mw = mw_;

            StartSound = new MediaPlayer();
            StartSound.Open(new Uri(System.Windows.Forms.Application.StartupPath+"/Sounds/Start.wav"));
            StopSound = new MediaPlayer();
            StopSound.Open(new Uri(System.Windows.Forms.Application.StartupPath + "/Sounds/Stop.wav"));
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
                mw.notify.Icon = Properties.Resources.notify_capturing;
            }
        }
        public void PlayStartSound()
        {
            StartSound.Position = TimeSpan.Zero;
            StartSound.Play();
        }
        public void PlayStopSound()
        {
            StopSound.Position = TimeSpan.Zero;
            StopSound.Play();
        }

        public void StopRecording()
        {
            isRecording = false;
            mw.RecordButton.Background = Brushes.GhostWhite;
            mw.notify.Icon = Properties.Resources.notify_norm;
            PlayStopSound();
        }

        public bool Capturing()
        {
            return isCapturing;
        }

        public void StartCapturing()
        {
            isCapturing = true;
            mw.CaptureButton.Background = Brushes.Red;
        }

        public void StopCapturing()
        {
            isCapturing = false;
            mw.CaptureButton.Background = Brushes.GhostWhite;
        }

        public void MouseChecking(bool mc)
        {
            mouseChecking = mc;
            Trace.WriteLine("MouseChecking :" + mouseChecking);
        }

        public void RecordMouse(bool rm)
        {
            recMouse = rm;
            Trace.WriteLine("RecordMouse :" + recMouse);
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
            Trace.WriteLine("Starting recorder loop...");

            Stopwatch time = new Stopwatch();

            while (true)
            {
                time.Restart();

                if (isCapturing && ( mw.appmani.IsTargetActive() || !isRecording ))
                {
                    mw.graph.Capture(recMouse);

                    if (isRecording && mw.appmani.IsTargetActive() && ( !mouseChecking || IsMousePressed() ))
                        mw.ffmpeg.Enqueue(mw.graph.GetBitmap());

                    time.Stop();
                    int timeLeft = CapInt - (int)time.Elapsed.TotalMilliseconds;
                    if (timeLeft > 0)
                        SpinWait.SpinUntil(() => false, timeLeft);
                }
                else
                    SpinWait.SpinUntil(() => false, 500);
            }
        }
    }
}
