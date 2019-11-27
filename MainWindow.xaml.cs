/*____________________________________________________
Drawing Recorder Beta v1.01

Developed by keroroxzz

Fixed wrong file name lable (1.01)

External Process:
    FFmpeg project (unmodified) under the LGPLv2.1
_____________________________________________________*/

using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace DRnamespace
{
    public partial class MainWindow : Window
    {
        public static string ErrorString { get; set; }

        //thread
        readonly Thread UI_thread;
        readonly Thread Drag_thread;

        readonly AppManager appmani;
        readonly FFmpeg ffmpeg;
        readonly Graph graph;
        readonly Recorder recorder;

        readonly AreaWind area_window;
        readonly Displayer displayer;
        readonly NotifyIcon notify;

        public MainWindow()
        {
            //Set up the icon first
            notify = new NotifyIcon
            {
                Text = "DrawingRecorder",
                Icon = Properties.Resources.notify_norm,
                Visible = true
            };

            //startup initializing
            InitializeComponent();

            //objects needed
            appmani = new AppManager(false, notify);
            ffmpeg = new FFmpeg();
            graph = new Graph((bool)DC_box.IsChecked);
            recorder = new Recorder(appmani, ffmpeg, graph, CaptureButton, RecordButton, notify);
            recorder.Start();

            //windows initializing
            area_window = new AreaWind();
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

            //initialize the UI thread and start it
            UI_thread = new Thread(UIthread);
            UI_thread.Start();

            Drag_thread = new Thread(NewDragMove);
            Drag_thread.Start();

            loadSettings();
        }

        void loadSettings()
        {
            IniManip inimanip = new IniManip("/settings.ini");
            FrameRateBox.Text = inimanip.Get("Settings", "FrameRate", "30");
            SpeedBox.Text = inimanip.Get("Settings", "Speed", "5");
            DC_box.IsChecked = inimanip.Get("Settings", "DetectScreenChange", "True").ToLower() == "true" ? true : false;
            DragThreshold = inimanip.GetInt("Settings", "DragThreshold", 6);

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
            fileNane_textBlock.Text = Lang.Get("Lang", "FileName", "FileName");
        }
    }
}
