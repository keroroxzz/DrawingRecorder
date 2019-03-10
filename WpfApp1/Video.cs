using System;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //video
        int CapInt = 1000;
        Process ffmpegProcess;
        Stream ffmpegStream;
        String ErrorString = "",
            TargetName = "",
            ActiveName = "";

        private void initializeFFmpeg()
        {
            LockTextBoxs();

            if (ffmpegProcess != null)
            {
                ffmpegProcess.Close();
                ffmpegProcess.Dispose();
            }

            ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = ArgumentBox.Text,
                    FileName = "ffmpeg.exe"
                }
            };

            ffmpegProcess.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
            ffmpegProcess.ErrorDataReceived += (s, e) => ErrorString = e.Data;

            ffmpegProcess.Start();

            ffmpegProcess.BeginErrorReadLine();

            ffmpegStream = ffmpegProcess.StandardInput.BaseStream;
        }

        private void finishFFmpeg()
        {
            UnlockTextBoxs();

            ffmpegStream.Flush();
            ffmpegStream.Dispose();
            ffmpegProcess.Close();
        }

        private String GetArgument()
        {
            String name = FileNameBox.Text;

            if (File.Exists(name + ".mp4"))
            {
                int i = 0;
                for (; File.Exists(name + "_" + i + ".mp4"); i++) ;
                name = name + "_" + i;
            }

            return "-framerate " + FrameRateBox.Text + " -i - -c:v libx264 -vf format=yuv420p -r " + FrameRateBox.Text + "  " + name + ".mp4";
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
    }
}