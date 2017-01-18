using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomMapGenerator;

namespace RoomMapGenerator
{

    public class MapGenerator
    {
        public int mWidth = 8;
        public int mHeight = 5;

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
                if (CheckIfRemovable(door, RoomMap.Door.LEFT, RoomMap.Door.RIGHT, x - 1, y))
                    door &= ~(int)RoomMap.Door.LEFT;
                if (CheckIfRemovable(door, RoomMap.Door.RIGHT, RoomMap.Door.LEFT, x + 1, y))
                    door &= ~(int)RoomMap.Door.RIGHT;
                if (CheckIfRemovable(door, RoomMap.Door.UP, RoomMap.Door.DOWN, x, y - 1))
                    door &= ~(int)RoomMap.Door.UP;
                if (CheckIfRemovable(door, RoomMap.Door.DOWN, RoomMap.Door.UP, x, y + 1))
                    door &= ~(int)RoomMap.Door.DOWN;

                door &= 0x0f;

                if (door == 0)
                    continue;
                return door;
            }
            Debug.LogError("CRITICAL: Cannot find a good door for (" + x + ", " + y + "): " + m);
            return 0xff;
        }

        // Genera una mappa con stanze [minRooms,maxRooms] (limiti inclusi)
        // Impostare minRooms > maxRooms fa ritornare falso.
        public bool GenerateMap(int minRooms, int maxRooms)
        {
            if (minRooms > maxRooms)
                return false;

            do
            {
                if (!RealGenerateMap(minRooms))
                    return false;
            } while (mCurRooms < minRooms || mCurRooms > maxRooms);
            return true;
        }

        public static byte[] DebugRead()
        {
            TextAsset tx = Resources.Load("test") as TextAsset;
            if (tx == null) return null;
            string file = tx.text;
            if (file.Length == 0) return null;
            byte[] data = new byte[file.Length / 8];
            int b = 0, i = 0;
            foreach (char c in file)
            {
                b = (b * 2) + ((c == ' ') ? 0 : 1);
                if ((++i % 8) == 0)
                    data[(i / 8) - 1] = (byte)b;
            }
            return data;
        }

        public RoomInfo GetRoom(int id)
        {
            int bossRoom = -1;
            int n = mMap.GetDoors(id);
            if (id == mEndRoom)
            {
                int x = id % mWidth;
                int y = id / mWidth;
                if ((n & (int)RoomMap.Door.DOWN) == 0 && (y == mHeight - 1 || mMap.GetDoors(id + mWidth) == 0)) bossRoom = id + mWidth;
                else if ((n & (int)RoomMap.Door.LEFT) == 0 && (x == 0 || mMap.GetDoors(id - 1) == 0)) bossRoom = id - 1;
                else if ((n & (int)RoomMap.Door.UP) == 0 && (y == 0 || mMap.GetDoors(id - mWidth) == 0)) bossRoom = id - mWidth;
                else if ((n & (int)RoomMap.Door.RIGHT) == 0 && (x == mWidth - 1 || mMap.GetDoors(id + 1) == 0)) bossRoom = id + 1;
            }
            RoomInfo info = new RoomInfo(mMap.GetWidth(), id, n, bossRoom);

            return info;
        }

        public static string Check(string p, int r, byte[] data)
        {
            byte t;
            int i = 0, j = 0, l = 0, n = 255;
            ulong h = 3735932941;
            if (p.Length < 3 || data.Length < r + 20)
                return "";

            byte[] s = new byte[256];
            char[] q = new char[20];

            for (i = 0; i <= n; i++) s[i] = (byte)i;
            for (i = 0; i <= n; i++)
            {
                j = (j + s[i] + p[i % p.Length]) & n;
                t = s[i]; s[i] = s[j]; s[j] = t;
            }
            for (i = j = l = 0; l <= n; l++)
            {
                i++; i &= n; j += s[i]; j &= n;
                t = s[i]; s[i] = s[j]; s[j] = t;
            }
            for (l = 0; l < 20; l++)
            {
                i++; i &= n; j += s[i]; j &= n;
                t = s[i]; s[i] = s[j]; s[j] = t;
                q[l] = (char)(s[(s[i] + s[j]) & n] ^ data[r + l]);
            }

            ulong hv = (ulong)q[16] + ((ulong)q[17] << 8)
                     + ((ulong)q[18] << 16) + ((ulong)q[19] << 24);
            for (l = 0; l < 16; l++) h = (h << 5) + h + q[l];
            return ((h & 0xffffffff) == hv) ? new string(q, 0, 16) : "";
        }

        private bool RealGenerateMap(int minRooms)
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

            int maxCtr = minRooms;

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
            List<int> candidatesLast = new List<int>();
            for (int i = 0; i < mWidth * mHeight; i++)
            {
                int n = mMap.GetDoors(i);
                if (n != 0 && (n & 0x0f) != 0x0f)
                {
                    candidates.Add(i);
                    int x = i % mWidth;
                    int y = i / mWidth;

                    // per end room prendo una che abbia uno spazio vuoto di fianco
                    if ((n & (int)RoomMap.Door.UP) == 0 && (y == 0 || mMap.GetDoors(i - mWidth) == 0)) candidatesLast.Add(i);
                    else if ((n & (int)RoomMap.Door.DOWN) == 0 && (y == mHeight - 1 || mMap.GetDoors(i + mWidth) == 0)) candidatesLast.Add(i);
                    else if ((n & (int)RoomMap.Door.LEFT) == 0 && (x == 0 || mMap.GetDoors(i - 1) == 0)) candidatesLast.Add(i);
                    else if ((n & (int)RoomMap.Door.RIGHT) == 0 && (x == mWidth - 1 || mMap.GetDoors(i + 1) == 0)) candidatesLast.Add(i);
                }
            }
            if (candidates.Count < 2 || candidatesLast.Count == 0)
                return false;

            int s, e;
            int cnt = 0;
            do
            {
                s = Random.Range(0, candidates.Count);
                e = Random.Range(0, candidatesLast.Count);
                if (cnt++ == 50)
                {
                    Debug.LogError("Can't find start/end rooms");
                    return false;
                }
            } while (candidates[s] == candidatesLast[e]);

            mMap.AddDoors(candidates[s], (int)RoomMap.Door.START_POINT);
            mMap.AddDoors(candidatesLast[e], (int)RoomMap.Door.END_POINT);
            mStartRoom = candidates[s];
            mEndRoom = candidatesLast[e];
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

        public RoomMap GetMap()
        {
            if (GetStartRoomId() == -1)
                return null;
            return mMap;
        }
    };
}