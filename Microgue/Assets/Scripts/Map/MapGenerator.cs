using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomMapGenerator;

namespace RoomMapGenerator
{
    public class MapGenerator
    {
        public int mWidth = 4;
        public int mHeight = 4;
        public int mMaxRooms = 6;
        public int mMinRooms = 3;

        private int mStartRoom, mEndRoom;
        private RoomMap mMap;
        private int mCurRooms = 0;
        private RandomQueue mQueue;

        bool CheckIfRemovable(int door, RoomMap.Door ll, RoomMap.Door rr, int xc, int yc)
        {
            return ((door & (int)ll) != 0 && mMap.GetDoors(xc, yc) != 0 && (mMap.GetDoors(xc, yc) & (int)rr) == 0);
        }

        // Generate a random door (Param 'm' = mandatory doors for that room)
        int RandomDoor(int x, int y, int m)
        {
            int ctr = 100;
            while (ctr > 0)
            {
                int door = m | (int)Random.Range(0, 16);

                if (x == 0 && (door & (int)RoomMap.Door.LEFT) != 0)
                    continue;
                if (x == mWidth - 1 && (door & (int)RoomMap.Door.RIGHT) != 0)
                    continue;
                if (y == 0 && (door & (int)RoomMap.Door.UP) != 0)
                    continue;
                if (y == mHeight - 1 && (door & (int)RoomMap.Door.DOWN) != 0)
                    continue;

                // Remove some doors (otherwise center rooms always have 4 doors)
                //if(CheckIfRemovable(door, RoomMap.Door.LEFT, RoomMap.Door.RIGHT, x-1, y))
                //    door &= ~(int)RoomMap.Door.LEFT;
                //if(CheckIfRemovable(door, RoomMap.Door.RIGHT, RoomMap.Door.LEFT, x+1, y))
                //    door &= ~(int)RoomMap.Door.RIGHT;
                //if(CheckIfRemovable(door, RoomMap.Door.UP, RoomMap.Door.DOWN, x, y-1))
                //    door &= ~(int)RoomMap.Door.UP;
                //if(CheckIfRemovable(door, RoomMap.Door.DOWN, RoomMap.Door.UP, x, y+1))
                //    door &= ~(int)RoomMap.Door.DOWN;

                door &= 0x0f;

                if (door == 0)
                    continue;
                return door;
            }
            Debug.LogError("CRITICAL: Cannot find a good door for (" + x + ", " + y + "): " + m);
            return 0xff;
        }

        public bool GenerateMap()
        {
            mMap.Clear();
            mQueue.Clear();
            mCurRooms = 0;

            // Start point
            int startX = Random.Range(0, mWidth);
            int startY = Random.Range(0, mHeight);
            int d = 0;
            while (d == 0 || d == 0xff)
                d = RandomDoor(startX, startY, 0);
            if (d == 0xff)
                return false;
            mQueue.Push(startX, startY, d);

            int maxCtr = mMinRooms;

            // Build rooms
            while (!mQueue.IsEmpty() && maxCtr-- > 0)
            {
                QueueEl el = mQueue.Pop();
                d = RandomDoor(el.x, el.y, el.m);

                mMap.SetDoors(el.x, el.y, d);

                // Add the corresponding door in the near room
                if ((d & (int)RoomMap.Door.LEFT) != 0)
                    mQueue.Push(el.x - 1, el.y, (int)RoomMap.Door.RIGHT);
                if ((d & (int)RoomMap.Door.RIGHT) != 0)
                    mQueue.Push(el.x + 1, el.y, (int)RoomMap.Door.LEFT);
                if ((d & (int)RoomMap.Door.UP) != 0)
                    mQueue.Push(el.x, el.y - 1, (int)RoomMap.Door.DOWN);
                if ((d & (int)RoomMap.Door.DOWN) != 0)
                    mQueue.Push(el.x, el.y + 1, (int)RoomMap.Door.UP);
            }

            // Add remaining rooms (just not to have doors on the void :P)
            while (!mQueue.IsEmpty())
            {
                QueueEl el = mQueue.Pop();
                mMap.AddDoors(el.x, el.y, el.m);
            }

            CalcNumberOfRooms();
            if (!CalcStartEndPoints())
                return false;

            //Debug.Log("Gen ok");
            return true;
        }

        private bool CalcStartEndPoints()
        {
            List<int> candidates = new List<int>();
            for (int i = 0; i < mWidth * mHeight; i++)
            {
                int n = mMap.GetDoors(i);
                if (n != 0 && (n & 0x0f) != 0x0f)
                    candidates.Add(i);
            }
            if (candidates.Count < 2)
                return false;

            int s = 0, e = 0;
            while (s == e)
            {
                s = Random.Range(0, candidates.Count);
                e = Random.Range(0, candidates.Count);
            }

            mMap.AddDoors(candidates[s], (int)RoomMap.Door.START_POINT);
            mMap.AddDoors(candidates[e], (int)RoomMap.Door.END_POINT);
            mStartRoom = candidates[s];
            mEndRoom = candidates[e];
            return true;
        }

        // Calculate the number of rooms in the map
        private void CalcNumberOfRooms()
        {
            mCurRooms = 0;
            for (int i = 0; i < mWidth * mHeight; i++)
                if (mMap.GetDoors(i) != 0)
                    mCurRooms++;
        }

        public int NumberOfRooms()
        {
            return mCurRooms;
        }

        public MapGenerator()
        {
            mMap = new RoomMap(mWidth, mHeight);
            mQueue = new RandomQueue(ref mMap);
            mStartRoom = mEndRoom = -1;
        }

        public int GetStartRoomId()
        {
            return mStartRoom;
        }

        public int GetEndRoomId()
        {
            return mEndRoom;
        }

        public RoomInfo GetRoom(int id)
        {
            RoomInfo info = new RoomInfo(mMap.GetWidth(), id, mMap.GetDoors(id));

            return info;
        }

        public RoomMap GetMap()
        {
            if (GetStartRoomId() == -1)
                return null;
            return mMap;
        }
    };
}
