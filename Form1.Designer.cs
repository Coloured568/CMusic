using System;

namespace cmusic
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.load = new System.Windows.Forms.Button();
            this.play = new System.Windows.Forms.Button();
            this.skip = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.volumeSlider1 = new NAudio.Gui.VolumeSlider();
            this.songlength = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pause = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.visualizerBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.songlength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.visualizerBox)).BeginInit();
            this.SuspendLayout();
            // 
            // load
            // 
            this.load.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load.Location = new System.Drawing.Point(286, 221);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(75, 50);
            this.load.TabIndex = 0;
            this.load.Text = "Load playlist";
            this.load.UseVisualStyleBackColor = true;
            this.load.Click += new System.EventHandler(this.button1_Click);
            // 
            // play
            // 
            this.play.Font = new System.Drawing.Font("MS Gothic", 6.25F, System.Drawing.FontStyle.Bold);
            this.play.Location = new System.Drawing.Point(367, 221);
            this.play.Name = "play";
            this.play.Size = new System.Drawing.Size(75, 50);
            this.play.TabIndex = 1;
            this.play.Text = "Start/Stop";
            this.play.UseVisualStyleBackColor = true;
            this.play.Click += new System.EventHandler(this.button2_Click);
            // 
            // skip
            // 
            this.skip.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.skip.Location = new System.Drawing.Point(531, 221);
            this.skip.Name = "skip";
            this.skip.Size = new System.Drawing.Size(75, 50);
            this.skip.TabIndex = 2;
            this.skip.Text = "Skip";
            this.skip.UseVisualStyleBackColor = true;
            this.skip.Click += new System.EventHandler(this.skip_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(283, 296);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Now playing: Nothing";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(205, 45);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(226, 160);
            this.listBox1.TabIndex = 4;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(363, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "CMusic";
            // 
            // volumeSlider1
            // 
            this.volumeSlider1.Location = new System.Drawing.Point(338, 402);
            this.volumeSlider1.Name = "volumeSlider1";
            this.volumeSlider1.Size = new System.Drawing.Size(104, 14);
            this.volumeSlider1.TabIndex = 7;
            // 
            // songlength
            // 
            this.songlength.Location = new System.Drawing.Point(230, 354);
            this.songlength.Name = "songlength";
            this.songlength.Size = new System.Drawing.Size(320, 42);
            this.songlength.TabIndex = 9;
            this.songlength.Scroll += new System.EventHandler(this.songlength_Scroll);
            this.songlength.MouseDown += new System.Windows.Forms.MouseEventHandler(this.songlength_MouseDown);
            this.songlength.MouseUp += new System.Windows.Forms.MouseEventHandler(this.songlength_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(363, 332);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "0:00";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // pause
            // 
            this.pause.Font = new System.Drawing.Font("MS Gothic", 6.25F, System.Drawing.FontStyle.Bold);
            this.pause.Location = new System.Drawing.Point(448, 221);
            this.pause.Name = "pause";
            this.pause.Size = new System.Drawing.Size(75, 50);
            this.pause.TabIndex = 11;
            this.pause.Text = "Play/Pause";
            this.pause.UseVisualStyleBackColor = true;
            this.pause.Click += new System.EventHandler(this.pause_click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(205, 221);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 50);
            this.button1.TabIndex = 12;
            this.button1.Text = "Previous";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.previous_Click);
            // 
            // visualizerBox
            // 
            this.visualizerBox.Location = new System.Drawing.Point(437, 45);
            this.visualizerBox.Name = "visualizerBox";
            this.visualizerBox.Size = new System.Drawing.Size(169, 160);
            this.visualizerBox.TabIndex = 13;
            this.visualizerBox.TabStop = false;
            this.visualizerBox.Click += new System.EventHandler(this.visualizerBox_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.visualizerBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pause);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.songlength);
            this.Controls.Add(this.volumeSlider1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.skip);
            this.Controls.Add(this.play);
            this.Controls.Add(this.load);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.songlength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.visualizerBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void label3_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.Button load;
        private System.Windows.Forms.Button play;
        private System.Windows.Forms.Button skip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label2;
        private NAudio.Gui.VolumeSlider volumeSlider1;
        private System.Windows.Forms.TrackBar songlength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button pause;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox visualizerBox;
    }
}

