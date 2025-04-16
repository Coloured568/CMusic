using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using System.Security.Cryptography;

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
            trayMenu.Items.Add("Save Playlist", null, (s, e) => SavePlaylist());
            trayMenu.Items.Add("Load Playlist", null, (s, e) => LoadPlaylist());
            var toggleVisualizerItem = new ToolStripMenuItem("Enable Visualizer")
            {
                CheckOnClick = true,
                Checked = visualizerBox.Visible
            };
            loop.CheckedChanged += (s, e) =>
            {
                if (loop.Checked)
                {
                    loop_single.Checked = false;
                    shuffle.Checked = false;
                }
            };

            loop_single.CheckedChanged += (s, e) =>
            {
                if (loop_single.Checked)
                {
                    loop.Checked = false;
                    shuffle.Checked = false;
                }
            };

            shuffle.CheckedChanged += (s, e) =>
            {
                if (shuffle.Checked)
                {
                    loop.Checked = false;
                    loop_single.Checked = false;
                }
            };

            toggleVisualizerItem.CheckedChanged += (s, e) =>
            {
                visualizerBox.Visible = toggleVisualizerItem.Checked;
                if(toggleVisualizerItem.Checked)
                {
                    visualizerBox.Size = new Size(169, 160);
                    listBox1.Size = new Size(226, 160);
                }
                else
                {
                    visualizerBox.Size = new Size(0, 0);
                    listBox1.Size = new Size(226 + 182, 160);
                }
                
            };

            trayMenu.Items.Add(toggleVisualizerItem);

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
                    "BGIMAGE=",
                    "FONT=",
                    "FONTSIZE=",
                    "HEADER="
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
            {
                try
                {
                    this.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.button1.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.play.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.pause.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.skip.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.load.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.listBox1.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.button2.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.button2.BackColor = Color.FromName(config["BACKGROUND"]);
                    this.button3.BackColor = Color.FromName(config["BACKGROUND"]);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Failed to change background color: {ex}");
                }
            }
            if (config.ContainsKey("COLOR"))
            {
                try
                {
                    this.ForeColor = Color.FromName(config["COLOR"]);
                    ApplyColorsToControls(this.Controls, this.ForeColor);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Failed to load color: {ex.Message}");
                }
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
            if (config.ContainsKey("FONT")) 
            {
                try
                {
                    int fontSize = int.Parse(config["FONTSIZE"]);
                    string font = (config["FONT"]);
                    //this.Font = new Font(font, fontSize);
                    this.button1.Font = new Font(font, fontSize);
                    this.play.Font = new Font(font, fontSize);
                    this.pause.Font = new Font(font, fontSize);
                    this.skip.Font = new Font(font, fontSize);
                    this.load.Font = new Font(font, fontSize);
                    this.listBox1.Font = new Font(font, fontSize);
                    this.button2.Font = new Font(font, fontSize);
                    this.button2.Font = new Font(font, fontSize);
                    this.button3.Font = new Font(font, fontSize);
                    this.label2.Font = new Font(font, fontSize);
                    this.label1.Font = new Font(font, fontSize);
                    this.label3.Font = new Font(font, fontSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load font: {ex.Message}");
                }
            }
            if (config.ContainsKey("HEADER"))
            {
                try
                {
                    label2.Text = config["HEADER"];
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load header: {ex}");
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
            if (playlist.Count == 0)
            {
                MessageBox.Show("No playlist loaded.");
                return;
            }

            if (shuffle.Checked)
            {
                // Shuffle mode: Pick a random track that is not the current one
                Random rand = new Random();
                int newTrackIndex;
                do
                {
                    newTrackIndex = rand.Next(playlist.Count);
                } while (newTrackIndex == currentTrackIndex && playlist.Count > 1); // Ensure a different track
                currentTrackIndex = newTrackIndex;
            }
            else
            {
                // Normal mode: Move to the next track
                currentTrackIndex++;
                if (currentTrackIndex >= playlist.Count)
                {
                    if (loop.Checked)
                    {
                        currentTrackIndex = 0; // Loop back to the start
                    }
                    else
                    {
                        MessageBox.Show("End of playlist.");
                        currentTrackIndex = playlist.Count - 1; // Stay at the last track
                        return;
                    }
                }
            }

            PlayCurrentTrack();
            UpdateLabel();
        }

        private void previous_Click(object sender, EventArgs e)
        {
            if (playlist.Count == 0)
            {
                MessageBox.Show("No playlist loaded.");
                return;
            }

            if (shuffle.Checked)
            {
                // Shuffle mode: Pick a random track that is not the current one
                Random rand = new Random();
                int newTrackIndex;
                do
                {
                    newTrackIndex = rand.Next(playlist.Count);
                } while (newTrackIndex == currentTrackIndex && playlist.Count > 1); // Ensure a different track
                currentTrackIndex = newTrackIndex;
            }
            else
            {
                // Normal mode: Move to the previous track
                currentTrackIndex--;
                if (currentTrackIndex < 0)
                {
                    if (loop.Checked)
                    {
                        currentTrackIndex = playlist.Count - 1; // Loop back to the last track
                    }
                    else
                    {
                        MessageBox.Show("Start of playlist.");
                        currentTrackIndex = 0; // Stay at the first track
                        return;
                    }
                }
            }

            PlayCurrentTrack();
            UpdateLabel();
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
                if (loop_single.Checked)
                {
                    // Repeat the current track
                    PlayCurrentTrack();
                    UpdateLabel();
                }
                else if (loop.Checked)
                {
                    // Loop through the playlist
                    currentTrackIndex++;
                    if (currentTrackIndex >= playlist.Count)
                    {
                        currentTrackIndex = 0; // Start from the beginning
                    }
                    PlayCurrentTrack();
                    UpdateLabel();
                }
                else if (shuffle.Checked)
                {
                    Random rand = new Random();
                    currentTrackIndex = rand.Next(0, playlist.Count);
                    PlayCurrentTrack();
                    UpdateLabel();
                }
                else
                {
                    // Move to the next track or stop if at the end of the playlist
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

        private void SavePlaylist()
        {
            if (playlist.Count == 0)
            {
                MessageBox.Show("No songs in the playlist to save.", "CMusic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CMusic Playlist (*.cmpl)|*.cmpl";
            sfd.Title = "Save Playlist";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllLines(sfd.FileName, playlist);
                    MessageBox.Show("Playlist saved successfully.", "CMusic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving playlist: " + ex.Message, "CMusic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadPlaylist()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CMusic Playlist (*.cmpl)|*.cmpl";
            ofd.Title = "Load Playlist";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    List<string> validPaths = new List<string>();

                    foreach (string line in lines)
                    {
                        if (File.Exists(line))
                            validPaths.Add(line);
                    }

                    if (validPaths.Count == 0)
                    {
                        MessageBox.Show("No valid files found in playlist.", "CMusic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    playlist.Clear();
                    playlist.AddRange(validPaths);
                    currentTrackIndex = 0;

                    listBox1.Items.Clear();
                    foreach (var file in playlist)
                    {
                        listBox1.Items.Add(Path.GetFileName(file));
                    }

                    MessageBox.Show("Playlist loaded successfully.", "CMusic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading playlist: " + ex.Message, "CMusic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void saveplaylist_click(object sender, EventArgs e)
        {
            SavePlaylist();
        }
        private void loadplaylist_click(Object sender, EventArgs e)
        {
            LoadPlaylist();
        }
    }
}
