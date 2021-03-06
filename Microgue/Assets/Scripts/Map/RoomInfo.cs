﻿using UnityEngine;
using System.Collections;
using RoomMapGenerator;

namespace RoomMapGenerator
{
    public class RoomInfo
    {
        private int mDoors;
        private int mId;
        private int mMapWidth; // used to get near doors
        private int mBossRoomId; 

        public int GetId() { return mId; }

        public RoomInfo(int mapWidth, int id, int m, int bossRoomId = -1)
        {
            mId = id;
            mDoors = m;
            mMapWidth = mapWidth;
            mBossRoomId = bossRoomId;
        }

        public int GetDoors()
        {
            return mDoors;
        }

        public bool HasDoor(RoomMap.Door door)
        {
            return (mDoors & (int)door) != 0;
        }

        public bool HasStartPoint()
        {
            return (mDoors & (int)RoomMap.Door.START_POINT) != 0;
        }

        public bool HasEndPoint()
        {
            return (mDoors & (int)RoomMap.Door.END_POINT) != 0;
        }

        // TODO fix vita
        public int GetStartOrEndDoor()
        {
            if (!HasStartPoint() && !HasEndPoint())
                return 0;

            if (!HasDoor(RoomMap.Door.DOWN) && mBossRoomId == mId + mMapWidth)
                return (int)RoomMap.Door.DOWN;

            if (!HasDoor(RoomMap.Door.LEFT) && mBossRoomId == mId - 1)
                return (int)RoomMap.Door.LEFT;

            if (!HasDoor(RoomMap.Door.UP) && mBossRoomId == mId - mMapWidth)
                return (int)RoomMap.Door.UP;

            return (int)RoomMap.Door.RIGHT;
        }

        public int GetRoomIdAt(RoomMap.Door door)
        {
            int w = mMapWidth;
            int x = mId % w;
            int y = mId / w;

            if (!HasDoor(door))
                return -1;

            switch (door)
            {
                case RoomMap.Door.DOWN:  return x + (y + 1) * w;
                case RoomMap.Door.UP:    return x + (y - 1) * w;
                case RoomMap.Door.LEFT:  return x - 1 + y * w;
                case RoomMap.Door.RIGHT: return x + 1 + y * w;
            }

            return -1;
        }
    };
}
