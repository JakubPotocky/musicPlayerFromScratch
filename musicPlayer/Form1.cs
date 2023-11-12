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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace musicPlayer
{
    public partial class Form1 : Form
    {

        string xd;
        int totalAdded = 0;
        int totalIndex = 0;
        WMPLib.IWMPPlaylist playlist; /// TOTO BOLO ZADEFINOVANE TU(XD!) VELMI DOLE
        public Form1()
        {InitializeComponent();}
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Empty");
            MessageBox.Show("Hello, welcome to ");
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
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            int selectedIndex = comboBox1.SelectedIndex - 1;
            if (Playlist1 != null && selectedIndex != 0 && selectedIndex != -1)
            {
                int startValue = lastIndex[selectedIndex - 1];
                int endValue = lastIndex[selectedIndex];

                for (int i = startValue + 1; i <= endValue; i++)
                {
                    if (Playlist1.TryGetValue(i, out var song))
                    {
                        string songName = song.Item1;
                        string songURL = song.Item2;
                        listBox1.Items.Add(songName);
                    }
                    else
                    {
                        listBox1.Items.Add("Nejde to :/");
                    }
                }
            }
            if (Playlist1 != null && selectedIndex == 0)
            {
                for (int i = 0; i <= lastIndex[0]; i++)
                {
                    if (Playlist1.TryGetValue(i, out var song))
                    {
                        string songName = song.Item1;
                        string songURL = song.Item2;
                        listBox1.Items.Add(songName);
                    }
                    else
                    {
                        listBox1.Items.Add("Doesnt work :/");
                    }
                }
            }
        }
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

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

        private void btnStart_Click(object sender, EventArgs e)
        {
            Player.URL = @"C:\\Users\\jakub\\Desktop\\FINAL Juice WRLD\\Juice wrld\\Juice WRLD - californication.mp3";
        }

        int volume;
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            volume = trackBar1.Value;
            Player.settings.volume = volume;
        }

        Dictionary<int, (string, string)> Playlist1 = new Dictionary<int, (string, string)>();
        List<int> lastIndex = new List<int>();

        Dictionary<int, (string, string)> Playlist2 = new Dictionary<int, (string, string)>();

    private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            foreach (var kvp in Playlist1)
            {
                Playlist2.Add(kvp.Key, kvp.Value);
            }
            Random rnd = new Random();
            int totalPesnickyIndex = Playlist2.Count;

            List<int> keys = new List<int>(Playlist2.Keys);

            playlist.clear();

            for (int i = totalPesnickyIndex - 1; i >= 0; i--)
            {
                int randomIndex = rnd.Next(0, keys.Count);
                int takeIndex = keys[randomIndex];

                if (Playlist2.TryGetValue(takeIndex, out var song))
                {
                    string songName = song.Item1;
                    string songURL = song.Item2;
                    listBox2.Items.Add(songName);
                    Playlist2.Remove(takeIndex);
                    keys.RemoveAt(randomIndex);
                    IWMPMedia media = Player.newMedia(songURL);
                    playlist.appendItem(media);///okej dostal som to ale neviem ako to dat do poradia(takto nie)
                }
                Player.Ctlcontrols.play();
            }
            ///play all function from playlist1 pls urob
        }
        string lastFolderName;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select specific playlist from folder you entered at the start ONLY!";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;
                    lastFolderName = new DirectoryInfo(folderPath).Name;
                    LoadMp3FilesFromFolder(folderPath);
                }
            }

        }

        private void LoadMp3FilesFromFolder(string folderPath) //async pouzi?** na lepsie nacitavanie
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("The specified folder does not exist.XDXD");
                return;
            }

            //string playlistName = GetDifferingCharacters(folderPath); ///playlist-40 song-48
            ///playlist == null; zatial kym tomu nepriradim nazov, potom pesnicky v foreach loope
            string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3"); ///zoberie fily do stringu (mp3 fily / iba ich cestu)
            Player.currentPlaylist.clear(); ///(vycisty playlist)
            playlist = Player.newPlaylist(lastFolderName, "");/// TOTO BOLO ZADEFINOVANE TAM(XD!)  ///vytvori playlist s nazvom
            comboBox1.Items.Add(lastFolderName); /// do comboboxu da na vyber playlist
            foreach (string mp3File in mp3Files) ///precita vsetky fily postupne
            {
                WMPLib.IWMPMedia media = Player.newMedia(mp3File); ///media premenna je na citanie
                playlist.appendItem(media); ///prida do "poradia" viacmenej
            }
            Player.currentPlaylist = playlist; /// zada playeru playlist co teraz vytvoril; PREMENNA PLAYLIST MOZE SLUZIT MOZNO NA PREPINANIE PLAYLISTOV
            if (Player.currentPlaylist != null)
            {
                for (int i = 0; i < Player.currentPlaylist.count; i++) /// toto cita z playeru fily po jednom a zapisuje ich
                {
                    WMPLib.IWMPMedia media = Player.currentPlaylist.get_Item(i); ///precita hodnotu z currentPlaylist a zapise do media
                    string mediaName = media.name;
                    string mediaSourceURL = media.sourceURL;
                    Playlist1.Add((totalAdded), (mediaName, mediaSourceURL));
                    totalAdded++;
                }
                lastIndex.Add(Player.currentPlaylist.count - 1 + totalIndex);///Na zapis vsetkych indexov potrebujes pripocitat celkovu hodnotu ktra sa stale meni :skull:
                totalIndex += Player.currentPlaylist.count;
            }
            else
            {
                MessageBox.Show("No songs selected.");
            }
            foreach (var songToUrl in Playlist1)
            {
                reverseLookup[songToUrl.Value.Item1] = songToUrl.Value.Item2;
            }
        }

        Dictionary<string, string> reverseLookup = new Dictionary<string, string>();
        private void btnNext_Click(object sender, EventArgs e)
        {
            Player.Ctlcontrols.next();

            if (Player.currentMedia != null)
            {
                string currentSongName = Player.currentMedia.name;
                label4.Text = currentSongName;
                //refreshQ(); // queue nefunguje- dobre, iba indexovanie na prvom playliste XD
            }
            else
            {
                Console.WriteLine("Nothing playing now...");
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            Player.Ctlcontrols.previous();

            if (Player.currentMedia != null)
            {
                string currentSongName = Player.currentMedia.name;
                label4.Text = currentSongName;
            }
            else
            {
                Console.WriteLine("Nothing playing now...");
            }
        }

        bool isRnd = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(!isRnd)
            {
                Player.settings.setMode("shuffle", true);
                isRnd = true;
            }
            if (isRnd)
            {
                Player.settings.setMode("shuffle", false);
                isRnd = false;
            }

        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (Player.currentMedia != null)
            {
                string currentSong = listBox1.Text;
                if (reverseLookup.TryGetValue(currentSong, out string url))
                {
                    Player.URL = url;
                    label4.Text = listBox1.Text;
                }
            }
        }

        int currentSongIndex = 0;
        int playlistNumber;
        public void refreshQ()
        {
            if (playlist != null && Playlist1 != null)
            {
                listBox2.Items.Clear();
                string currentSongName = Player.currentMedia.name;
                for(int i = 0; i < Playlist1.Count; i++) 
                {
                    if (Playlist1.TryGetValue(i, out var song))
                    {
                        string songName = song.Item1;
                        if(currentSongName == songName)
                        {
                            currentSongIndex = i;
                            isCloseTo(); ///
                            if (lastIndex.Count == 2)
                            {
                                //MessageBox.Show("Output: " + playlistNumber.ToString() + "\nlastIndex0: " + lastIndex[0].ToString() + "\nlastIndex1:" + lastIndex[1].ToString());
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong :/");
                    }
                }
                for (int i = currentSongIndex; i < Player.currentPlaylist.count-1; i++) /// toto cita z playeru fily po jednom a zapisuje ich
                {                                         /// tu plus a minus 1 aby sa to vyrovnalo
                    WMPLib.IWMPMedia media = Player.currentPlaylist.get_Item(i+1); ///precita hodnotu z currentPlaylist a zapise do media
                    listBox2.Items.Add(media.name);
                }
            }
        }

        public void isCloseTo()
        {
            if (lastIndex[0] <= currentSongIndex)
            {
                playlistNumber = 0;
            }
            else
            {
                for (int i = 0; i < lastIndex.Count - 1; i++) ///potrebujem zistit kde sa nachadza aby som mohol upravit vypis
                {
                    if (currentSongIndex < lastIndex[0])
                    {
                        playlistNumber = i + 1;
                    }

                    if (currentSongIndex >= lastIndex[i] && currentSongIndex < lastIndex[i + 1])
                    {
                        playlistNumber = i + 1;
                    }
                }
            }

        }

        private void Player_MediaChange(object sender, _WMPOCXEvents_MediaChangeEvent e)
        {
            if (Player.currentMedia != null)
            {
                string currentSongName = Player.currentMedia.name;
                label4.Text = currentSongName;
            }
            else
            {
                Console.WriteLine("Nothing playing now...");
            }
        }
    }
}