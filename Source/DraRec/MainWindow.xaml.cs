/*____________________________________________________
Drawing Recorder v1.3.2

Developed by keroroxzz

1.3 - improve stuff, see comments in each file.

External Process:
    FFmpeg project (unmodified) under the LGPLv2.1
_____________________________________________________*/

using System.Windows;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media;

namespace DRnamespace
{
    public partial class MainWindow : Window
    {
        public readonly AppManager appManager;
        public readonly FFmpeg ffmpeg;
        public readonly Graph graph;
        public readonly Compare compare;
        public readonly UpdateManager update;
        readonly Recorder recorder;

        private ListButton qualitySelector;
        private ListButton proportionButtion;
        private ListButton lockButton;
        private SwitchButton areaButton;
        private SwitchButton recordButton;
        private SwitchButton captureButton; 
        readonly AreaWind areaWindow;
        readonly Displayer displayWindow;
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
            appManager = new AppManager(false, notify);
            ffmpeg = new FFmpeg(this);
            graph = new Graph();
            recorder = new Recorder(this);
            compare = new Compare();
            update = new UpdateManager();
            recorder.Start();

            //windows initializing
            areaWindow = new AreaWind(this);
            displayWindow = new Displayer();

            //setup the mouseclick event of the icon
            notify.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Show();
                    ShowInTaskbar = true;
                    Activate();

                    if (displayWindow.IsVisible)
                    {
                        displayWindow.Show();
                        displayWindow.Activate();
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

            LoadSettings();
            UiInit();

            if(CU_box.IsChecked==true && update.IsAnyUpdate())
            {
                notify.ShowBalloonTip(300, "", "There is a new version: " + update.LatestVersion() + ".", System.Windows.Forms.ToolTipIcon.None);
            }
        }

        void LoadSettings()
        {
            Trace.WriteLine("Loading settings...");

            IniManip inimanip = new IniManip("/settings.ini");
            FrameRateBox.Text = inimanip.Get("Settings", "FrameRate", "30");
            SpeedBox.Text = inimanip.Get("Settings", "Speed", "5");
            DC_box.IsChecked = inimanip.GetBool("Settings", "DetectScreenChange", true);
            MD_box.IsChecked = inimanip.GetBool("Settings", "DetectMouseDown", true);
            RM_box.IsChecked = inimanip.GetBool("Settings", "RecordMouse", true);
            DragThreshold = inimanip.GetInt("Settings", "DragThreshold", 10);
            ffmpegArgs = inimanip.Get("Settings", "ffmpegArgs", "-framerate {frameRate} -i - -c:v libx264 -preset veryslow -crf {quality} -vf format=yuv420p -r {frameRate} {fileName}");
            DirectoryBox.Text = inimanip.Get("Settings", "OutputDirectory", "Output");
            FileNameBox.Text = inimanip.Get("Settings", "FileName", "video_");
            areaWindow.Top = inimanip.GetInt("CaptureArea", "Y", 70);
            areaWindow.Left = inimanip.GetInt("CaptureArea", "X", 70);
            areaWindow.Width = inimanip.GetInt("CaptureArea", "Width", 300);
            areaWindow.Height = inimanip.GetInt("CaptureArea", "Height", 300);
            CU_box.IsChecked = inimanip.GetBool("Settings", "CheckUpdate", true);

            IniManip Lang = new IniManip("/lang.ini");
            frameRate_textBlock.Text = Lang.Get("Lang", "FrameRate", "FrameRate");
            speed_textBlock.Text = Lang.Get("Lang", "Speed", "Speed");
            DC_box.Content = Lang.Get("Lang", "DetectChange", "DetectChange");
            MD_box.Content = Lang.Get("Lang", "DetectMouse", "DetectMouse");
            RM_box.Content = Lang.Get("Lang", "RecordMouse", "RecordMouse");
            CU_box.Content = Lang.Get("Lang", "CheckUpdate", "CheckUpdate");
            fileNane_textBlock.Text = Lang.Get("Lang", "FileName", "FileName");
            quality_textBlock.Text = Lang.Get("Lang", "Quality", "Quality");

            //initialize the switch buttom
            qualitySelector = new ListButton(2, 5, SettingGrid);
            qualitySelector.Background = Brushes.Transparent;
            qualitySelector.BorderBrush = Brushes.Transparent;
            qualitySelector.Foreground = Brushes.White;
            qualitySelector.Style = (Style)Resources["Button_Style"];

            qualitySelector.AddItem(new ListBoxItem() { Content = Lang.Get("Lang", "QualityLow", "Low"), Tag = "30" });
            qualitySelector.AddItem(new ListBoxItem() { Content = Lang.Get("Lang", "QualityMid", "Mid"), Tag = "24" });
            qualitySelector.AddItem(new ListBoxItem() { Content = Lang.Get("Lang", "QualityHigh", "High"), Tag = "18" });

            qualitySelector.SelectedIndex(inimanip.GetInt("Settings", "Quality", 0));

            //initialize the switch buttom
            proportionButtion = new ListButton(1, 0, ToolGrid);
            proportionButtion.SelectedIndex(inimanip.GetInt("Settings", "Quality", 0));
            proportionButtion.Style = (Style)Resources["Button_Style"];
            proportionButtion.Background = (Brush)Resources["Background_Button"];
            proportionButtion.BorderBrush = Brushes.Transparent;
            proportionButtion.Click += ProportionButtion_Click;

            proportionButtion.AddItem(new ListBoxItem() { Content = Lang.Get("Lang", "Prop-None", "None"), Tag = 0.0 });
            proportionButtion.AddItem(new ListBoxItem() { Content = "4:3", Tag = 4.0 / 3.0 });
            proportionButtion.AddItem(new ListBoxItem() { Content = "16:9", Tag = 16.0 / 9.0 });
            proportionButtion.AddItem(new ListBoxItem() { Content = "3:4", Tag = 3.0 / 4.0 });
            proportionButtion.AddItem(new ListBoxItem() { Content = "9:16", Tag = 9.0 / 16.0 });
            proportionButtion.AddItem(new ListBoxItem() { Content = "Full", Tag = -1.0 });

            areaButton = new SwitchButton((Brush)Resources["Red"], (Brush)Resources["Background_Button"], 0, 0, ToolGrid);
            areaButton.Content = Lang.Get("Lang", "Area", "Area");
            areaButton.Style = (Style)Resources["Button_Style"];
            areaButton.BorderBrush = Brushes.Transparent;
            areaButton.Click += AreaButton_Click;
            areaButton.ActivateClick += AreaButton_ActivateClick;
            areaButton.DeactivateClick += AreaButton_DeactivateClick;

            captureButton = new SwitchButton((Brush)Resources["Red"], (Brush)Resources["Background_Button"], 2, 0, ToolGrid);
            captureButton.Content = Lang.Get("Lang", "Capture", "Record");
            captureButton.Style = (Style)Resources["Button_Style"];
            captureButton.BorderBrush = Brushes.Transparent;
            captureButton.Click += CaptureButton_Click;
            captureButton.ActivateClick += CaptureButton_ActivateClick;
            captureButton.DeactivateClick += CaptureButton_DeactivateClick;

            recordButton = new SwitchButton((Brush)Resources["Red"], (Brush)Resources["Background_Button"], 3, 0, ToolGrid);
            recordButton.Content = Lang.Get("Lang", "Record", "Start");
            recordButton.Style = (Style)Resources["Button_Style"];
            recordButton.BorderBrush = Brushes.Transparent;
            recordButton.Click += RecordButton_Click;
            recordButton.ActivateClick += RecordButton_ActivateClick;
            recordButton.DeactivateClick += RecordButton_DeactivateClick;

            lockButton = new ListButton(4, 0, ToolGrid, switch_by_click: false);
            lockButton.Style = (Style)Resources["Button_Style"];
            lockButton.Background = (Brush)Resources["Background_Button"];
            lockButton.BorderBrush = Brushes.Transparent;
            lockButton.Click += LockButton_Click;

            lockButton.AddItem(new ListBoxItem() { Content = Lang.Get("Lang", "NoLock", "NoLock"), Tag = false });
            foreach (Process prop in Process.GetProcesses())
            {
                if(prop.MainWindowTitle.Length>0)
                    lockButton.AddItem(new ListBoxItem() { Content = prop.ProcessName, Tag = true });
            }

            Trace.WriteLine("Settings are ready!");
        }
    }
}
