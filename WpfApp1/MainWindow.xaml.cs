/*____________________________________________________
Drawing Recorder Alpha v1.2

Developed by keroroxzz

External Process:
    FFmpeg project (unmodified) under the LGPLv2.1
_____________________________________________________*/

using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace DRnamespace
{
    public partial class MainWindow : Window
    {
        public static string ErrorString { get; set; }

        //thread
        Thread UI_thread;

        AppManager appmani;
        FFmpeg ffmpeg;
        Graph graph;
        Recorder recorder;

        AreaWind area_window;
        Displayer displayer;

        NotifyIcon notify;

        public MainWindow()
        {
            notify = new NotifyIcon
            {
                Icon = DRnamespace.Properties.Resources.notify_norm,
                Visible = true
            };

            InitializeComponent();

            appmani = new AppManager(false);
            ffmpeg = new FFmpeg();
            graph = new Graph((bool)DC_box.IsChecked);
            recorder = new Recorder(appmani, ffmpeg, graph, CaptureButton, RecordButton, notify);

            notify.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Show();
                    ShowInTaskbar = true;
                    Activate();

                    if (displayer.IsVisible)
                    {
                        displayer.Show();
                        displayer.Activate();
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (recorder.Recording())
                        recorder.StopRecording();
                    else
                    {
                        recorder.StartRecording();
                    }
                }
            };

            this.Opacity = 0.0;

            recorder.Start();

            UI_thread = new Thread(UIthread);
            UI_thread.Start();

            area_window = new AreaWind();
            displayer = new Displayer();
        }
    }
}
