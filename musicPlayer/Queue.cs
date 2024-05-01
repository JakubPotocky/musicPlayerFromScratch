using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.Design;

namespace musicPlayer
{
    internal class Queue
    {
        public List<Song> q; //create new queue which will be play next queue, and if the queue is empty it will play from q
        //this way I cant add the song randomly into the queue :/

        public List<Song> NextQ;
        public int Position;
        public int maxPos;
        public Queue()
        {
            q = new List<Song>();
            NextQ = new List<Song>();
            Position = 0;
        }

        public void CreateQueue(Playlist playlist) 
        {
            q.Clear();
            foreach(Song s in playlist.songs) 
            {
                q.Add(s);
            }
            //q.Reverse();
            maxPos = q.Count() - 1;
        }

        public void ClearQueue() 
        {
            q.Clear(); 
        }
        public void Shuffle()
        {
            int temp = q.Count()-1;
            List<Song> tempQ = q.ToList();
            q.Clear();
            Random rnd = new Random();
            for(int i = temp; i >= 0; i--) 
            {
                int newIndex = rnd.Next(0, i+1);
                q.Add(tempQ[newIndex]);
                tempQ.RemoveAt(newIndex);
            }
        }

        public Song PlayNext()
        {
            if(q.Count > 0)
            {
                Position++;
                if (Position > maxPos)
                {
                    Position = 0;
                }

                Song s = q[Position];
                return s;
            }
            else
            {
                return null;
            }
        }

        public Song PlayLast()
        {
            Position--;
            if (Position < 0)
                Position = maxPos;

            Song s = q[Position];
            return s;
        }

        public void AddToQueueFront(Song s)
        {
            NextQ.Add(s);
        }

        public void AddToQueue(int index, Song s)
        {
            q.Insert(index, s);
        }
        public void AddToQueueLast(Song s)
        {
            q.Add(s);
        }

        public void RemoveFromQueue(Song s) 
        {
            if(NextQ.Contains(s))
            {
                NextQ.Remove(s);
                return;
            }

            else if(q.Contains(s))
            {
                q.Remove(s);
                maxPos--;
                return;
            }
        }
        public int GetIndex(string songName) 
        {
            int temp = 0;
            foreach(Song s in q) 
            {
                if (s.name == songName)
                    return temp;

                temp++;
            }
            return -1;
        }

        public Song GetSong(string songName)
        {
            foreach (Song s in Song.AllSongs)
            {
                if (s.name == songName)
                    return s;
            }
            return null;
        }


    }
}
