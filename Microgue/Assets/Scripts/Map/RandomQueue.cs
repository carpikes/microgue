using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomMapGenerator;

namespace RoomMapGenerator
{

    public struct QueueEl
    {
        public int x, y, m;
        public QueueEl(int x, int y, int m)
        {
            this.x = x;
            this.y = y;
            this.m = m;
        }
    };

    public class RandomQueue
    {
        private List<QueueEl> mQueue;
        private readonly RoomMap mMap;

        public RandomQueue(ref RoomMap map)
        {
            mQueue = new List<QueueEl>();
            mMap = map;
        }

        public void Clear()
        {
            mQueue.Clear();
        }

        public bool IsEmpty()
        {
            return mQueue.Count == 0;
        }

        // Add a new element in the queue.
        // It changes the element on the map(or in the queue) if it's already there
        public void Push(int x, int y, int m)
        {
            // Is it already on the map?
            if (mMap.GetDoors(x, y) != 0)
            {
                mMap.AddDoors(x, y, m);
                return;
            }

            // Is it already in the queue?
            for (int i = 0; i < mQueue.Count; i++)
            {
                QueueEl el = mQueue[i];
                if (el.x == x && el.y == y)
                {
                    el.m |= m;
                    mQueue[i] = el;
                    return;
                }
            }

            // Add it in the queue
            mQueue.Add(new QueueEl(x, y, m));
        }

        // Pop a random element from this queue
        public QueueEl Pop()
        {
            int n = Random.Range(0, mQueue.Count);
            QueueEl a = mQueue[n];
            mQueue.RemoveAt(n);
            return a;
        }
    };

}