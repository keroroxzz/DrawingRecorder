﻿/*
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
        public double framerate = 1.0;
        public double speed = 1.0;

        //experiment
        double tl = 0.0;
        double tc = 0.0;
        double smt_sp = 0.0;

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
                isRecording = true;
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
            PlayStopSound();
        }

        public bool Capturing()
        {
            return isCapturing;
        }

        public void StartCapturing()
        {
            isCapturing = true;
            Trace.WriteLine("Record : " + Recording());
        }

        public void StopCapturing()
        {
            isCapturing = false;
            Trace.WriteLine("Record : " + Recording());
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

            tl = CapInt - tc;
            tl = tl < 0 ? 0 : tl;
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
            Stopwatch t2 = new Stopwatch();


            while (true)
            {
                time.Restart();

                if (isCapturing && ( mw.appManager.IsTargetActive() || !isRecording ))
                {

                    double real_speed = t2.Elapsed.TotalMilliseconds * framerate / 1000.0;
                    t2.Restart();
                    smt_sp = smt_sp * 0.9 + 0.1 * real_speed;
                    tl = tl - (real_speed - speed)*Math.Sqrt(speed);
                    tl = tl < 0 ? 0 : tl;
                    Console.WriteLine("Real Speed:" + real_speed + "  \tSmt Speed:" + smt_sp + "  \tInterval: " + tl);
                    mw.graph.Capture(recMouse);

                    if (isRecording && mw.appManager.IsTargetActive() && ( !mouseChecking || IsMousePressed() ))
                        mw.ffmpeg.Enqueue(mw.graph.GetBitmap());

                    
                    tc = 0.9 * tc + 0.1 * time.Elapsed.TotalMilliseconds;
                    /*int timeLeft = CapInt - (int)tc;
                    if (timeLeft > 0)
                        SpinWait.SpinUntil(() => false, timeLeft);*/

                    /*int timeLeft = CapInt - (int)time.Elapsed.TotalMilliseconds;
                    if (timeLeft > 0)
                        SpinWait.SpinUntil(() => false, timeLeft);*/

                    SpinWait.SpinUntil(() => false, (int)tl);
                }
                else
                    SpinWait.SpinUntil(() => false, 500);
            }
        }
    }
}
