/*
 * Author : Brian Tu (RTU)
 * Last modify : 2020/8/27
 * 
 * This is a class managing ffmpeg.
 * 
 * Add stream buffer
 * Improve streaming
 * Replace difference detection here
 * Exceptiom handle for saving to stream
 */

using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Drawing.Imaging;

namespace DRnamespace
{
    public class FFmpeg
    {
        //bool DetectDifference = true;
        //bool isWrited = false;
        //bool threading = false;
        //Thread thread;

        Process ffmpegProcess;
        Stream ffmpegStream;
        //ArrayList buffer = new ArrayList();

        MainWindow mw;

        //constructer===========================================================
        public FFmpeg(MainWindow mw_)
        {
            mw = mw_;
        }

        //public func===========================================================
        public void Startup(string arg)
        {
            Trace.WriteLine("Startup FFmpeg...");

            if (ffmpegProcess != null)
            {
                ffmpegProcess.Close();
                ffmpegProcess.Dispose();
            }

            if (!File.Exists("ffmpeg.exe"))
                File.WriteAllBytes("ffmpeg.exe", Properties.Resources.ffmpeg);

            ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = arg,
                    FileName = "ffmpeg.exe"
                }
            };
            Trace.WriteLine("Arguments : " + arg);

            ffmpegProcess.Start();

            ffmpegProcess.BeginErrorReadLine();

            ffmpegStream = ffmpegProcess.StandardInput.BaseStream;

            //StartThread();
        }

        public void Enqueue(Bitmap bitmap)
        {
            if(mw.compare.Comparing(bitmap))
                ToStream(bitmap);

            //Sending to ffmpge is acutally pretty fast (<= 1ms), so there's no need to save them to another queue and streaming in another thread.
            //buffer.Add(bitmap);
        }
        public void Finish()
        {
            try
            {
                Trace.WriteLine("Close FFmpeg streaming...");
                ffmpegStream.Close();
            }
            catch (Exception e)
            {
                Trace.WriteLine("Close FFmpeg streaming failed : " + e.Message);
            }
        }


        /*public void Finish()
        {
            try
            {
                StopThread(true);
                ffmpegStream.Flush();
                ffmpegStream.Dispose();
                ffmpegProcess.Close();

                foreach (Bitmap bmp in buffer)
                    bmp.Dispose();
                buffer.Clear();

                Trace.WriteLine("FFmpeg is finishing...");
            }
            catch(Exception e)
            {
                Trace.WriteLine("FFmpeg finishing failed : " + e.Message);
            }
        }*/

        /*public bool IsWrited()
        {
            bool result = isWrited;
            isWrited = false;
            return result;
        }*/

        /*public void ActiveDectect()
        {
            DetectDifference = true;
            Trace.WriteLine("Dectect state : " + DetectDifference);
        }

        public void StopDectect()
        {
            DetectDifference = false;
            Trace.WriteLine("Dectect state : " + DetectDifference);
        }*/

        //core===========================================================
        /*void StartThread()
        {
            StopThread();
            thread = new Thread(Loop);
            thread.Start();
        }

        void StopThread(bool wait = false)
        {
            if (thread != null && thread.IsAlive)
            {
                Trace.WriteLine("Stopping FFmpeg thread...");

                threading = false;
                if (wait)
                    thread.Join();
                thread.Abort();
            }
        }

        Bitmap Dequeue()
        {
            if (buffer.Count > 0)
            {
                Bitmap bmp = (Bitmap)buffer[0];
                buffer.RemoveAt(0);
                return bmp;
            }
            return null;
        }*/

        void ToStream(Bitmap bmp)
        {
            //isWrited = true;
            try
            {
                bmp.Save(ffmpegStream, ImageFormat.Bmp);
                bmp.Dispose();
            }
            catch(Exception e)
            { Trace.TraceError("Save ffmpegStream Failed!" + e.Message); }
        }

        /*void Loop()
        {
            Trace.WriteLine("Starting FFmpeg thread...");
            threading = true;
            while (threading)
            {
                Bitmap bmp = Dequeue();
                if (bmp != null && ( !DetectDifference || compare.Comparing(bmp) ))
                    ToStream(bmp);
                else
                    SpinWait.SpinUntil(() => false, 10);
            }
        }*/
    }
}