﻿using UnityEngine;
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
            START_POINT = 0x10,
            END_POINT = 0x20,
        };

        private char[] mMap;
        private int mWidth, mHeight;

        public RoomMap(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mMap = new char[mWidth * mHeight];
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < mMap.Length; i++)
                mMap[i] = (char)0;
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
            mMap[x + y * mWidth] = (char)d;
        }

        public void AddDoors(int x, int y, int d)
        {
            mMap[x + y * mWidth] |= (char)d;
        }

        public void AddDoors(int n, int d)
        {
            mMap[n] |= (char)d;
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
