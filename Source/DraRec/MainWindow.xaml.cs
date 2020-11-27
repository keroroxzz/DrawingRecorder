/*____________________________________________________
Drawing Recorder v1.2

Developed by keroroxzz

1.2 - improve stuff, see comments in each file.

External Process:
    FFmpeg project (unmodified) under the LGPLv2.1
_____________________________________________________*/

using System.Windows;
using System.Diagnostics;
using System.Windows.Forms;

namespace DRnamespace
{
    public partial class MainWindow : Window
    {
        public readonly AppManager appmani;
        public readonly FFmpeg ffmpeg;
        public readonly Graph graph;
        readonly Recorder recorder;

        readonly AreaWind area_window;
        readonly Displayer displayer;
        public readonly NotifyIcon notify;

        public MainWindow()
        {
            Trace.WriteLine("\n\n\n[---" + System.DateTime.Now + "---]\n\n");
            Trace.WriteLine("Startup Application...");

            //Set up the icon first
            notify = new NotifyIcon
            {
                Text = "DrawingRecorder",
                Icon = Properties.Resources.notify_norm,
                Visible = true,
            };

            //startup initializing
            InitializeComponent();

            //objects needed
            appmani = new AppManager(false, notify);
            ffmpeg = new FFmpeg(this);
            graph = new Graph();
            recorder = new Recorder(this);
            recorder.Start();

            //windows initializing
            area_window = new AreaWind(AreaButton, this);
            displayer = new Displayer();

            //setup the mouseclick event of the icon
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
                        recorder.StartRecording();
                }
            };

            loadSettings();
            UiInit();
        }

        void loadSettings()
        {
            Trace.WriteLine("Loading settings...");

            IniManip inimanip = new IniManip("/settings.ini");
            FrameRateBox.Text = inimanip.Get("Settings", "FrameRate", "30");
            SpeedBox.Text = inimanip.Get("Settings", "Speed", "5");
            DC_box.IsChecked = inimanip.GetBool("Settings", "DetectScreenChange", true);
            MD_box.IsChecked = inimanip.GetBool("Settings", "DetectMouseDown", true);
            RM_box.IsChecked = inimanip.GetBool("Settings", "RecordMouse", true);
            DragThreshold = inimanip.GetInt("Settings", "DragThreshold", 10);
            ffmpegArgs = inimanip.Get("Settings", "ffmpegArgs", "-framerate {frameRate} -i - -c:v libx264 -preset veryslow -crf 28 -vf format=yuv420p -r {frameRate} {fileName}");
            DirectoryBox.Text = inimanip.Get("Settings", "OutputDirectory", "Output");
            FileNameBox.Text = inimanip.Get("Settings", "FileName", "video_");
            area_window.Top = inimanip.GetInt("CaptureArea", "Y", 70);
            area_window.Left = inimanip.GetInt("CaptureArea", "X", 70);
            area_window.Width = inimanip.GetInt("CaptureArea", "Width", 300);
            area_window.Height = inimanip.GetInt("CaptureArea", "Height", 300);

            IniManip Lang = new IniManip("/lang.ini");
            CaptureButton.Content = Lang.Get("Lang", "Capture", "Capture");
            RecordButton.Content = Lang.Get("Lang", "Record", "Record");
            AreaButton.Content = Lang.Get("Lang", "Area", "Area");
            ProportionButtion.Content = Lang.Get("Lang", "Prop-None", "None");
            frameRate_textBlock.Text = Lang.Get("Lang", "FrameRate", "FrameRate");
            speed_textBlock.Text = Lang.Get("Lang", "Speed", "Speed");
            DC_box.Content = Lang.Get("Lang", "DetectChange", "DetectChange");
            MD_box.Content = Lang.Get("Lang", "DetectMouse", "DetectMouse");
            RM_box.Content = Lang.Get("Lang", "RecordMouse", "RecordMouse");
            fileNane_textBlock.Text = Lang.Get("Lang", "FileName", "FileName");

            Trace.WriteLine("Loading settings is Done!");
        }
    }
}
