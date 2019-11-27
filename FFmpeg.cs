using System.IO;
using System.Diagnostics;

namespace DRnamespace
{
    public class FFmpeg
    {
        bool isWrited = false;

        Process ffmpegProcess;
        Stream ffmpegStream;

        public void Startup(string arg)
        {
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
            
            ffmpegProcess.ErrorDataReceived += (s, e) => MainWindow.ErrorString = e.Data;

            ffmpegProcess.Start();

            ffmpegProcess.BeginErrorReadLine();

            ffmpegStream = ffmpegProcess.StandardInput.BaseStream;
        }

        public void Write(byte[] array)
        {
            isWrited = true;
            ffmpegStream.Write(array, 0, array.Length);
        }

        public void finish()
        {
            ffmpegStream.Flush();
            ffmpegStream.Dispose();
            ffmpegProcess.Close();
        }

        public bool IsWrited()
        {
            bool result = isWrited;
            isWrited = false;
            return result;
        }
    }
}