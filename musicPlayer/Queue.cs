using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace musicPlayer
{
    internal class Queue
    {
        public List<Song> q; //create new queue which will be play next queue, and if the queue is empty it will play from q
        //this way I cant add the song randomly into the queue :/

        public List<Song> NextQ;
        public int Position;
        private int maxPos;
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

        public Song PlayNext()
        {
            if(NextQ.Count > 0)
            {
                Song s = NextQ[NextQ.Count-1];
                NextQ.RemoveAt(NextQ.Count-1);
                return s;
            }
            else
            {
                Position++;
                if (Position > maxPos)
                { 
                    Position = 0;
                }
                
                Song s = q[Position];
                return s;
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

        public void AddToQueueBack(Song s)
        {
            NextQ.Insert(0, s);
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
                return;
            }
        }


    }
}
