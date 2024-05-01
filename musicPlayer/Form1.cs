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
            MessageBox.Show("Hello,\n Welcome to my music player :)\n\nHere is quick guide to this music player.\n1. Click add and select folder which contains .mp3 files you want to load.\n2. After you have done that you can select it in dropbox and then click start which will start playing the album, other working features: play/stop, next, previous, close player.");
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
        { ///selecting from dropbox
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
                { ///reset everything so the program can load new playlist
                    playlist.clear();
                    Player.currentPlaylist.clear();

                    listBox2.Items.Clear();
                    listBox3.Items.Clear();

                    que.Position = 0;
                }

                int sel = comboBox1.SelectedIndex - 1;
                Playlist selectedPlaylist = Playlist.GetPlaylist(sel);

                playlist = Player.newPlaylist(selectedPlaylist.playlistName, ""); ///this loads somehow the playlist into the player

                que.CreateQueue(selectedPlaylist); ///create queue from playlist
                
                foreach(Song s in que.q) 
                { ///each song is added to playlist in order +to show them in listbox 2 and 3
                    IWMPMedia media = Player.newMedia(s.location);
                    playlist.appendItem(media);

                    listBox2.Items.Add(s.name); //add songs from queue to middle listbox

                    listBox3.Items.Add(s.name); //add songs from queue/direct playlist to last listbox
                }

                ///play the playlist
                Player.currentPlaylist = playlist;
                Player.Ctlcontrols.play();
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
        { /// this is "Play All" button ----- SHUFFLE BUTTON
          ///Start button

            if (playlist != null)
            { ///reset everything so the program can load new playlist
                playlist.clear();
                Player.currentPlaylist.clear();

                listBox2.Items.Clear();

                que.Position = 0;
            }

            que.Shuffle();
            
            foreach (Song s in que.q)
            { ///each song is added to playlist in order +to show them in listbox 2 and 3
                IWMPMedia media = Player.newMedia(s.location);
                playlist.appendItem(media);

                listBox2.Items.Add(s.name); //add songs from queue to middle listbox
            } ///^this doesnt keep up with the player, the player plays media sooner than the listBox2 items are inserted

            ///play the playlist
            Player.currentPlaylist = playlist;
            Player.Ctlcontrols.play();

        }

        /// Adding new playlist
        private void btnAdd_Click(object sender, EventArgs e)
        {
            new Playlist();
            try
            {
                comboBox1.Items.Add(Playlist.GetLastPlaylist().playlistName);
            }
            catch (Exception ex) { }
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
            //que.PlayNext(); ///this is useless because when song ends by itself the position number doesnt change
            Player.Ctlcontrols.next();
        }
        ///=====================================

        /// Last song ========================
        private void btnPrev_Click(object sender, EventArgs e)
        {
            //que.PlayLast();
            Player.Ctlcontrols.previous();
        }
        /// ==============================================
        /// =================================================
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            //THIS WAS EMPTY!!!!! shuffle on off checkmark?
        }
        ///==========================Double click left listbox to add songs to queue===========
        int currentSongPlayingIndex;
        int listBox1SelectedIndex;
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(Player.currentMedia != null)
            {
                currentSongPlayingIndex = que.GetIndex(Player.currentMedia.name);
                listBox1SelectedIndex = listBox1.SelectedIndex;

                if (currentSongPlayingIndex == -1)
                {
                    MessageBox.Show("Error double click listbox1"); 
                    return;
                }
                //play specific song from listbox1
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Play next", null, queueTop);
                contextMenu.Items.Add("Play last", null, queueBot);
                contextMenu.Show(listBox1, e.Location);
            }
            else
            {
                MessageBox.Show("If you want to add song to queue, you need to be playing some music first.");
            }

        }

        ///========================Queue handlers for listbox1===========================
        private void queueTop(object sender, EventArgs e)
        {
            double currentPosition = Player.Ctlcontrols.currentPosition;

            int sel = comboBox1.SelectedIndex - 1; ///get index of selected playlist
            Playlist selectedPlaylist = Playlist.GetPlaylist(sel); ///get selected playlist

            Song selectedSong = selectedPlaylist.GetSong(listBox1SelectedIndex); ///get selected song from playlist

            if(que.q.Contains(selectedSong))
            {
                MessageBox.Show("You can not add song that is already in the queue playlist. \nOne day there might be feature so you can move it to desired place");
                return;
            }

            IWMPMedia newSong = Player.newMedia(selectedSong.location); ///convert the song to good format

            playlist.insertItem(currentSongPlayingIndex + 1, newSong); ///insert the song to play as next
            listBox2.Items.Insert(currentSongPlayingIndex + 1, selectedSong.name); ///insert the song to queue
            que.AddToQueue(currentSongPlayingIndex + 1, selectedSong); ///add selected song to the que so we can get name from the list "q"

            // Optionally update the Windows Media Player control
            Player.currentPlaylist = playlist; ///set the playlist for currently playing playlist / update


            ///this section will get you back where you added the song, it is needed because the Player doesnt have anything to set the current position in playlist
            for (int i = 0; i < currentSongPlayingIndex; i++)
                Player.Ctlcontrols.next();

            Player.Ctlcontrols.currentPosition = currentPosition;
        }

        private void queueBot(object sender, EventArgs e)
        {
            double currentPosition = Player.Ctlcontrols.currentPosition;

            int sel = comboBox1.SelectedIndex - 1; ///get index of selected playlist
            Playlist selectedPlaylist = Playlist.GetPlaylist(sel); ///get selected playlist

            Song selectedSong = selectedPlaylist.GetSong(listBox1SelectedIndex); ///get selected song from playlist

            if (que.q.Contains(selectedSong))
            {
                MessageBox.Show("You can not add song that is already in the queue playlist. \nOne day there might be feature so you can move it to desired place");
                return;
            }

            IWMPMedia newSong = Player.newMedia(selectedSong.location); ///convert the song to good format

            playlist.appendItem(newSong); ///insert the song to play as next
            listBox2.Items.Add(selectedSong.name); ///insert the song to queue
            que.AddToQueueLast(selectedSong); ///add selected song to the que so we can get name from the list "q"

            // Optionally update the Windows Media Player control
            Player.currentPlaylist = playlist; ///set the playlist for currently playing playlist / update


            ///this section will get you back where you added the song, it is needed because the Player doesnt have anything to set the current position in playlist
            for (int i = 0; i < currentSongPlayingIndex; i++)
                Player.Ctlcontrols.next();

            Player.Ctlcontrols.currentPosition = currentPosition;
        }
        ///==============================================
        /// write what song is currently playing
        private void Player_MediaChange(object sender, _WMPOCXEvents_MediaChangeEvent e) //no need to change anything here
        {
            if (Player.currentMedia != null)
            {
                string currentSongName = Player.currentMedia.name;

                label4.Text = currentSongName;

                if (que.GetSong(currentSongName).isLiked)
                {
                    label4.Text = "❤" + label4.Text.Substring(0, label4.Text.Length - 1);
                    //button1.Text = "Unlike Song";
                }
                /*else
                {
                    button1.Text = "Like Song";
                }*/
                
                int temp = que.GetIndex(currentSongName);
                if (temp != -1)
                {
                    ///bug fix? MessageBox.Show(temp.ToString());
                    listBox2.SelectedIndex = temp; ///listbox2 index doenst exist, line 177
                }
                else
                    MessageBox.Show("Error player media change and song not found in *que*");
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

        ///´=====================double click listbox2 to delete an item========
        int listBox2SelectedIndex;
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Player.currentMedia != null)
            {
                currentSongPlayingIndex = que.GetIndex(Player.currentMedia.name);
                listBox2SelectedIndex = listBox2.SelectedIndex;

                if (currentSongPlayingIndex == -1)
                {
                    MessageBox.Show("Error double click listbox2");
                    return;
                }
                //play specific song from listbox1
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Remove song", null, removeFromListBox2);
                contextMenu.Items.Add("Play now", null, playNowFromListBox2);
                contextMenu.Show(listBox2, e.Location);
            }
            else
            {
                MessageBox.Show("If you want to add song to queue, you need to be playing some music first.");
            }
        }

        private void removeFromListBox2(object sender, EventArgs e)
        {
            try
            {
                double currentPosition = Player.Ctlcontrols.currentPosition;

                Song selectedSong = que.q[listBox2SelectedIndex];

                IWMPMedia newSong = playlist.Item[listBox2SelectedIndex];

                playlist.removeItem(newSong); ///WHY this doesnt want to take this song

                listBox2.Items.Remove(selectedSong.name); ///insert the song to queue
                que.RemoveFromQueue(selectedSong); ///add selected song to the que so we can get name from the list "q"

                // Optionally update the Windows Media Player control
                Player.currentPlaylist = playlist; ///set the playlist for currently playing playlist / update


                ///this section will get you back where you added the song, it is needed because the Player doesnt have anything to set the current position in playlist
                if (listBox2SelectedIndex < currentSongPlayingIndex)
                    currentSongPlayingIndex--;
                    
                for (int i = 0; i < currentSongPlayingIndex; i++)
                    Player.Ctlcontrols.next();
                if(listBox2SelectedIndex != currentSongPlayingIndex)
                    Player.Ctlcontrols.currentPosition = currentPosition;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playNowFromListBox2(object sender, EventArgs e)
        {
            try
            {
                int moveBy = 0;
                if(listBox2SelectedIndex > currentSongPlayingIndex)
                {
                    moveBy = listBox2SelectedIndex - currentSongPlayingIndex;
                }

                else 
                {
                    moveBy = que.q.Count() - currentSongPlayingIndex + listBox2SelectedIndex;
                }


                for (int i = 0; i < moveBy; i++)
                    Player.Ctlcontrols.next();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Player.currentMedia != null)
            {
                string currentSongName = Player.currentMedia.name;

                Song temp = que.GetSong(currentSongName);
                if (temp != null)
                {
                    temp.LikeSong();
                }
                else
                    MessageBox.Show("Error player media change and song not found in *que*");
            }
            else
            {
                MessageBox.Show("Nothing playing now...");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool isThereSomething = false;
            foreach (Song s in Song.AllSongs)
            { ///each song is added to playlist in order +to show them in listbox 2 and 3
                if (s.isLiked)
                {
                    isThereSomething = true;
                    break;
                }
            }
            if(!isThereSomething)
            {
                MessageBox.Show("There are no songs added as Liked");
                return;
            }

            if (playlist != null)
            { ///reset everything so the program can load new playlist
                playlist.clear();
                Player.currentPlaylist.clear();

                listBox2.Items.Clear();
                que.ClearQueue();
                que.Position = 0;
            }

            foreach (Song s in Song.AllSongs)
            { ///each song is added to playlist in order +to show them in listbox 2 and 3
                if(s.isLiked) 
                { 
                    IWMPMedia media = Player.newMedia(s.location);
                    que.AddToQueueLast(s);
                    listBox2.Items.Add(s.name); //add songs from queue to middle listbox
                    playlist.appendItem(media);
                }
            }

            ///play the playlist
            Player.currentPlaylist = playlist;
            Player.Ctlcontrols.play();
        }
        
    }
}