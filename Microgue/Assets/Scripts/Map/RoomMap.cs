using UnityEngine;
using System.Collections;
using RoomMapGenerator;

namespace RoomMapGenerator
{
    public class RoomMap
    {
        public enum Door
        {
            UP = 0x01,
            DOWN = 0x02,
            LEFT = 0x04,
            RIGHT = 0x08,
            // TODO togliere start point, end point
            START_POINT = 0x10,
            END_POINT = 0x20,
            VISITED = 0x40,
        };

        private byte[] mMap;
        private int mWidth, mHeight;

        public RoomMap(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mMap = new byte[mWidth * mHeight];
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < mMap.Length; i++)
                mMap[i] = (byte)0;
        }

        public int GetDoors(int n)
        {
            return mMap[n];
        }

        public int GetDoors(int x, int y)
        {
            return mMap[x + y * mWidth];
        }

        public void SetDoors(int x, int y, int d)
        {
            mMap[x + y * mWidth] = (byte)d;
        }

        public void AddDoors(int x, int y, int d)
        {
            mMap[x + y * mWidth] |= (byte)d;
        }

        public void AddDoors(int n, int d)
        {
            mMap[n] |= (byte)d;
        }

        public int GetWidth()
        {
            return mWidth;
        }

        public int GetHeight()
        {
            return mHeight;
        }
    };
}
