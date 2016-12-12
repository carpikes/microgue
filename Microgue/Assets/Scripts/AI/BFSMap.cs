using System;
using UnityEngine;
using System.Collections.Generic;

using IntPoint = AIMap.IntPoint;

public class BFSMap
{
	private byte[] mMap;
	private int mWidth, mHeight;
	private byte[] mObstacles;

	public BFSMap (int w, int h)
	{
		mMap = new byte[w * h];
		mWidth = w;
		mHeight = h;

        ResetMap();
	}

    public void ResetMap()
    {
		for (int i = 0; i < mMap.Length; i++)
			mMap [i] = 255;
    }

    public void SetMap(byte[] map)
    {
        mMap = map;
    }

	public byte[] StepAndGetMap(IntPoint playerPos, byte[] obstacles)
	{
		mObstacles = obstacles;

        if (!(playerPos.x < 0 || playerPos.x >= mWidth || playerPos.y < 0 || playerPos.y >= mHeight))
        {
            mMap[playerPos.x + mWidth * playerPos.y] = 1;
            mObstacles[playerPos.x + mWidth * playerPos.y] = 0;
        } 
        else
            Debug.Log("NANAN");

        bool stop = false;
		while (!stop)
		{
            stop = true;
            for (int y = 0; y < mHeight; y++)
                for (int x = 0; x < mWidth; x++)
                {
                    if (obstacles[y * mWidth + x] != 0)
                    {
                        mMap[y * mWidth + x] = 255;
                        continue;
                    }
                    int[] neigh = GetNeigh (x,y);
                    foreach (int p in neigh)
                    {
                        if (mMap[y * mWidth + x] - mMap[p] > 1)
                        {
                            mMap[y * mWidth + x] = (byte)(mMap[p] + 1);
                            stop = false;
                        }
                    }
                }
		}
		return mMap;
	}

	private int[] GetNeigh(int x, int y)
	{
		List<int> points = new List<int> ();
		if (x > 0 && mObstacles[(x - 1) + mWidth * y] == 0)
			points.Add (x - 1 + mWidth * y);
		if (y > 0 && mObstacles[x + mWidth * (y-1)] == 0)
			points.Add (x + mWidth * (y-1));
		if (x < mWidth - 1 && mObstacles[(x + 1) + mWidth * y] == 0)
			points.Add (x + 1 + mWidth * y);
		if (y < mHeight - 1 && mObstacles[x + mWidth * (y+1)] == 0)
			points.Add (x + mWidth * (y+1));		
		return points.ToArray();
	}
}

