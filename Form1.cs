using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;

namespace cmusic
{
    public partial class Form1 : Form
    {
        private List<string> playlist = new List<string>();
        private int currentTrackIndex = 0;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private bool isScrubbing = false;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        // Visualizer
        private Timer visualizerTimer;
        private float[] visualSamples = new float[32]; // Number of bars
        private Random rng = new Random();

        public Form1()
        {
            InitializeComponent();
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Restore", null, (s, e) => ShowFromTray());
            trayMenu.Items.Add("Exit", null, (s, e) => {
                trayIcon.Visible = false;
                Application.Exit();
            });

            trayIcon = new NotifyIcon();
            trayIcon.Text = "CMusic";
            trayIcon.Icon = new Icon("favicon.ico");
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;

            trayIcon.DoubleClick += (s, e) => ShowFromTray();
            this.Text = "CMusic";
            string path = "config.txt";
            string imagepath;
            string[] lines;

            if (File.Exists(path))
            {
                lines = File.ReadAllLines(path);
            }
            else
            {
                lines = new string[]
                {
                    "BACKGROUND=BLACK",
                    "COLOR=WHITE",
                    "VISUALIZER=true",
                    "BGIMAGE="
                };
                File.WriteAllLines(path, lines);
            }

            // Parse config and apply colors
            Dictionary<string, string> config = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    config[parts[0].Trim().ToUpper()] = parts[1].Trim().ToUpper();
                }
            }

            if (config.ContainsKey("BACKGROUND"))
             this.BackColor = Color.FromName(config["BACKGROUND"]);
            this.button1.BackColor = Color.FromName(config["BACKGROUND"]);
            this.play.BackColor = Color.FromName(config["BACKGROUND"]);
            this.pause.BackColor = Color.FromName(config["BACKGROUND"]);
            this.skip.BackColor = Color.FromName(config["BACKGROUND"]);
            this.load.BackColor = Color.FromName(config["BACKGROUND"]);
            this.listBox1.BackColor = Color.FromName(config["BACKGROUND"]);

            if (config.ContainsKey("COLOR"))
            {
                this.ForeColor = Color.FromName(config["COLOR"]);
                ApplyColorsToControls(this.Controls, this.ForeColor);
            }
            if (config.ContainsKey("BGIMAGE") && File.Exists(config["BGIMAGE"]))
            {
                try
                {
                    this.BackgroundImage = Image.FromFile(config["BGIMAGE"]);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load background image: " + ex.Message);
                }
            }

            // Initialize visualizer timer and event
            visualizerTimer = new Timer();
            visualizerTimer.Interval = 50;
            visualizerTimer.Tick += VisualizerTimer_Tick;
            visualizerTimer.Start();

            visualizerBox.Paint += VisualizerBox_Paint;

            this.Resize += Form1_Resize;
            CenterLabel();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            timer1.Interval = 100;
            timer1.Tick += timer1_Tick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files|*.mp3;*.wav;*.aac;*.wma;*.opus;*.flac";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                playlist.Clear();
                playlist.AddRange(ofd.FileNames);
                currentTrackIndex = 0;

                listBox1.Items.Clear();
                foreach (var file in playlist)
                {
                    listBox1.Items.Add(Path.GetFileName(file));
                }

                MessageBox.Show("Playlist loaded with " + playlist.Count + " tracks.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (playlist.Count == 0)
            {
                MessageBox.Show("No playlist loaded.");
                return;
            }

            PlayCurrentTrack();
            UpdateLabel();
        }

        private void skip_Click(object sender, EventArgs e)
        {
            currentTrackIndex++;
            if (currentTrackIndex < playlist.Count)
            {
                PlayCurrentTrack();
                UpdateLabel();
            }
            else
            {
                MessageBox.Show("End of playlist.");
                currentTrackIndex = 0;
            }
        }

        private void previous_Click(object sender, EventArgs e)
        {
            currentTrackIndex--;
            if (currentTrackIndex < playlist.Count && currentTrackIndex >= 0)
            {
                PlayCurrentTrack();
                UpdateLabel();
            }
            else
            {
                MessageBox.Show("Start of playlist.");
                currentTrackIndex = 0;
            }
        }

        private void PlayCurrentTrack()
        {
            try
            {
                if (outputDevice != null)
                {
                    if (outputDevice.PlaybackState == PlaybackState.Playing)
                        outputDevice.Stop();

                    outputDevice.PlaybackStopped -= OnPlaybackStopped;
                    outputDevice.Dispose();
                    outputDevice = null;
                }

                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }

                audioFile = new AudioFileReader(playlist[currentTrackIndex]);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                songlength.Value = 0;
                songlength.Maximum = (int)Math.Max(1, audioFile.TotalTime.TotalSeconds);
                outputDevice.Play();
                timer1.Start();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing file: " + ex.Message);
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                currentTrackIndex++;
                if (currentTrackIndex < playlist.Count)
                {
                    PlayCurrentTrack();
                    UpdateLabel();
                }
                else
                {
                    timer1.Stop();
                }
            }
        }

        private void UpdateLabel()
        {
            if (currentTrackIndex < playlist.Count)
            {
                string fileName = Path.GetFileName(playlist[currentTrackIndex]);
                label1.Text = $"Now playing: {fileName}";
                CenterLabel();

                if (currentTrackIndex >= 0 && currentTrackIndex < listBox1.Items.Count)
                {
                    listBox1.SelectedIndex = currentTrackIndex;
                }
            }
        }

        private void CenterLabel()
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterLabel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                trayIcon.ShowBalloonTip(1000, "CMusic", "Minimized to tray. Double-click the icon to restore.", ToolTipIcon.Info);
                return;
            }
            else
            {
                base.OnFormClosing(e);

                if (outputDevice != null)
                {
                    outputDevice.Stop();
                    outputDevice.Dispose();
                }

                if (audioFile != null)
                {
                    audioFile.Dispose();
                }
            }

            base.OnFormClosing(e);
        }

        private TimeSpan GetTimeRemaining()
        {
            if (audioFile != null)
            {
                return audioFile.TotalTime - audioFile.CurrentTime;
            }
            return TimeSpan.Zero;
        }

        private void UpdateTimeDisplay()
        {
            if (audioFile != null)
            {
                TimeSpan remaining = GetTimeRemaining();
                label3.Text = $"{remaining:mm\\:ss}";
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && listBox1.SelectedIndex < playlist.Count)
            {
                currentTrackIndex = listBox1.SelectedIndex;
                PlayCurrentTrack();
                UpdateLabel();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (audioFile != null && !isScrubbing)
            {
                if (audioFile.TotalTime.TotalSeconds > 0)
                {
                    songlength.Value = Math.Min(songlength.Maximum,
                        (int)(audioFile.CurrentTime.TotalSeconds));
                }

                UpdateTimeDisplay();
            }
        }

        private void songlength_MouseDown(object sender, MouseEventArgs e)
        {
            isScrubbing = true;
        }

        private void songlength_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePlaybackPosition();
            isScrubbing = false;
            UpdateTimeDisplay();
        }

        private void songlength_Scroll(object sender, EventArgs e)
        {
            if (audioFile != null && audioFile.TotalTime.TotalSeconds > 0)
            {
                double targetSeconds = songlength.Value;
                TimeSpan remaining = audioFile.TotalTime - TimeSpan.FromSeconds(targetSeconds);
                label3.Text = $"{remaining:mm\\:ss}";
            }
        }

        private void UpdatePlaybackPosition()
        {
            if (audioFile != null && audioFile.TotalTime.TotalSeconds > 0)
            {
                double targetSeconds = songlength.Value;
                audioFile.CurrentTime = TimeSpan.FromSeconds(targetSeconds);
            }
        }

        private void pause_click(object sender, EventArgs e)
        {
            if (outputDevice.PlaybackState == PlaybackState.Paused)
            {
                outputDevice.Play();
            }
            else
            {
                outputDevice.Pause();
            }
        }

        private void ApplyColorsToControls(Control.ControlCollection controls, Color foreColor)
        {
            foreach (Control ctrl in controls)
            {
                ctrl.ForeColor = foreColor;

                if (ctrl.HasChildren)
                {
                    ApplyColorsToControls(ctrl.Controls, foreColor);
                }
            }
        }

        private void ShowFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        // Visualizer code
        private void VisualizerTimer_Tick(object sender, EventArgs e)
        {
            if (audioFile != null)
            {
                // Simulating with random values for now (replace this with actual audio data)
                for (int i = 0; i < visualSamples.Length; i++)
                {
                    visualSamples[i] = (float)rng.NextDouble();
                }
            }
            visualizerBox.Invalidate(); // Triggers a repaint
        }

        private void VisualizerBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(visualizerBox.BackColor);
            visualizerBox.BackColor = Color.Black;
            int barCount = visualSamples.Length;
            float barWidth = visualizerBox.Width / (float)barCount;
            Brush barBrush = Brushes.Lime;

            for (int i = 0; i < barCount; i++)
            {
                float val = visualSamples[i];
                float barHeight = val * visualizerBox.Height;
                float x = i * barWidth;
                float y = visualizerBox.Height - barHeight;
                g.FillRectangle(barBrush, x, y, barWidth - 2, barHeight);
            }
        }

        private void visualizerBox_Click(object sender, EventArgs e)
        {

        }
    }
}
