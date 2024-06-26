﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace musicPlayer
{
    internal class Song
    {
        private static int nextID = -1;
        private int id {get;}
        public string name { get;set;}
        public string location { get;set;}
        public double minutes { get;set;}
        public static List<Song> AllSongs = new List<Song>();
        public bool isLiked { get; set; }

        public Song(string Name, string Location)
        {
            id = nextID++;
            name = Name;
            location = Location;
            isLiked = false;
            AllSongs.Add(this);
        }

        public void LikeSong()
        {
            if (isLiked) 
            {
                isLiked = false;
            }
            else
                isLiked = true;
        }
    }
}
