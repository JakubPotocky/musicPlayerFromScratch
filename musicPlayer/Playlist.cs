using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace musicPlayer
{
    internal class Playlist
    {
        private static int nextId = 0;
        private int id;
        public List<Song> songs;
        private int numberOfSongs { get; set; }
        public string playlistName { get; set; }
        private double numberOfMinutes { get; set; }
        private string folderPath { get; set; }
        public static List<Playlist> EveryPlaylist { get; private set; } = new List<Playlist>();

        public Playlist()
        {
            id = nextId++;
            songs = new List<Song>();
            addAllSongs();
            numberOfSongs = songs.Count;
            EveryPlaylist.Add(this);
        }

        public static Playlist GetPlaylist(int id)
        {
            foreach(Playlist playlist in EveryPlaylist) 
            {
                if (playlist.id == id) 
                {
                    return playlist;
                }
            }
            return null;
        }
        public static Playlist GetLastPlaylist()
        {
            int lastId = EveryPlaylist.Count();
            Playlist lastPlaylist = GetPlaylist(lastId-1);
            return lastPlaylist;
        }

        private void addAllSongs()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select specific playlist from folder you entered at the start ONLY!";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    folderPath = folderDialog.SelectedPath;
                    playlistName = new DirectoryInfo(folderPath).Name;
                    LoadMp3FilesFromFolder(folderPath);
                }
            }

        }

        private string getSongNameWithDifference(string songURL)
        {
            int longer = songURL.Length;
            int shorter = folderPath.Length;
            string result = "";
            for (int i = shorter+1; i < longer-4; i++) 
            {
                result += songURL[i];
            }
            return result;
        }

        private void LoadMp3FilesFromFolder(string _folderPath) //async pouzi?** na lepsie nacitavanie
        {
            if (!Directory.Exists(_folderPath))
            {
                MessageBox.Show("The specified folder does not exist.XDXD");
                return;
            }

        string[] mp3Files = Directory.GetFiles(_folderPath, "*.mp3"); ///zoberie fily do stringu (mp3 fily / iba ich cestu)
        foreach (string mp3File in mp3Files)
            {
                Song song = new Song(getSongNameWithDifference(mp3File), mp3File); ///need to get mp3File vs folderPath string difference
                songs.Add(song);
            }
        }

    }
}
