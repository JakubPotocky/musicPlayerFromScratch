using AxWMPLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Speech.Synthesis.TtsEngine;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WMPLib;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace musicPlayer
{
    public partial class Form1 : Form
    {
        WMPLib.IWMPPlaylist playlist; /// TOTO BOLO ZADEFINOVANE TU(XD!) VELMI DOLE
        public Form1()
        {InitializeComponent();}
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Empty");
            comboBox1.SelectedIndex = 0;
            MessageBox.Show("Hello,\n Welcome to my music player :)\n\nHere is quick guide to this music player.\n1. Click add and select folder which contains .mp3 files you want to load.\n2. After you have done that you can enjoy to very limited settings you can change heh- sound, track, track current position, shuffle(only works once to turn on, cant turn off haha), play/stop, next, previous, Play All button(creates shuffle from all songs you added) close player.");
            //MessageBox.Show($"Output: {xd.Count()}"); //Just checking value
        }
        private void Form1_Click(object sender, EventArgs e)
        { }
        private void btnClose_Click(object sender, EventArgs e)
        {Environment.Exit(0);}
        private bool mouseDown;
        private Point lastLocation;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
        ///======================================================= FROM NOW ON CODE STARTS
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if(comboBox1.SelectedIndex != 0)
            { 
                int sel = comboBox1.SelectedIndex-1;
                Playlist selectedPlaylist = Playlist.GetPlaylist(sel);
                foreach (Song song in selectedPlaylist.songs)
                {
                    listBox1.Items.Add(song.name);
                }
            }
        }
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// This should be fine for playing and stop playing
        bool playing = true;
        private void btnSS_Click(object sender, EventArgs e)
        {
            if (playing)
            {
                Player.Ctlcontrols.pause();
                playing = false;
            }
            else
            {
                Player.Ctlcontrols.play();
                playing = true;
            }

        }
        ///=================================================

        Queue que = new Queue();
        private void btnStart_Click(object sender, EventArgs e)
        {
            ///Start button
            if(comboBox1.SelectedIndex != 0)
            {
                if (playlist != null)
                {
                    playlist.clear();
                    Player.currentPlaylist.clear();

                    listBox2.Items.Clear();
                    listBox3.Items.Clear();

                    que.Position = 0;
                }

                int sel = comboBox1.SelectedIndex - 1;
                Playlist selectedPlaylist = Playlist.GetPlaylist(sel);

                playlist = Player.newPlaylist(selectedPlaylist.playlistName, "");

                que.CreateQueue(selectedPlaylist);
                
                foreach(Song s in que.q)
                {
                    IWMPMedia media = Player.newMedia(s.location);
                    playlist.appendItem(media);

                    listBox2.Items.Add(s.name); //add songs from queue to middle listbox

                    listBox3.Items.Add(s.name); //add songs from queue/direct playlist to last listbox
                }

                MessageBox.Show("Start");
                Player.currentPlaylist = playlist;
                Player.Ctlcontrols.play();
                MessageBox.Show("Should be playing");
            }
            else
                label4.Text = @"Please select playlist and then click *Start* button.";

        }


        ///====================== VOLUME EDITOR =====================================================
        int volume;
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            volume = trackBar1.Value;
            Player.settings.volume = volume;
        }
        ///==========================================================================================


        private void btnSaveSettings_Click(object sender, EventArgs e)
        { /// this is "Play All" button
           
        }

        /// Adding new playlist
        private void btnAdd_Click(object sender, EventArgs e)
        {
            new Playlist();
            comboBox1.Items.Add(Playlist.GetLastPlaylist().playlistName);
        }
        ///=================================================
        /**     Cool stuff here:
                listBox2.Items.Add(songName);

                IWMPMedia media = Player.newMedia(songURL);
                playlist.appendItem(media);
                Player.Ctlcontrols.play();
         */
        
        /// Next song ===================================
        private void btnNext_Click(object sender, EventArgs e)
        {
            que.PlayNext();
            Player.Ctlcontrols.next();
        }
        ///=====================================

        /// Last song ========================
        private void btnPrev_Click(object sender, EventArgs e)
        {
            que.PlayLast();
            Player.Ctlcontrols.previous();
        }
        /// ==============================================


        /// I hope this stuff works as it should
        bool isRnd = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(isRnd)
            {
                Player.settings.setMode("shuffle", true);
            }
            else //if(!isRnd)
            {
                Player.settings.setMode("shuffle", false);
                isRnd = false;
            }
            if(isRnd)
            {
                isRnd = true;
            }
            
        }
        /// =================================================
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            //THIS WAS EMPTY!!!!! shuffle on off checkmark?
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //play specific song from listbox1
        }

        /// write what song is currently playing
        private void Player_MediaChange(object sender, _WMPOCXEvents_MediaChangeEvent e) //no need to change anything here
        {
            if (Player.currentMedia != null)
            {
                string currentSongName = Player.currentMedia.name;
                label4.Text = currentSongName;

                listBox2.SelectedIndex = que.Position;
            }
            else
            {
                Console.WriteLine("Nothing playing now...");
            }
        }
        /// ======================================

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}