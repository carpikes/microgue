using System;
using UnityEngine;
using System.Collections.Generic;

using IntPoint = AIMap.IntPoint;

public class BFSMap
{
	private byte[] mMap;
	private int mWidth, mHeight;
	private IntPoint mPlayerPos;
	private bool[] mVisited;
	private byte[] mObstacles;

	public BFSMap (int w, int h)
	{
		mMap = new byte[w * h];
		mVisited = new byte[w * h];
		mWidth = w;
		mHeight = h;

		for (int i = 0; i < w * h; i++)
		{
			mMap [i] = 0;
			mVisited [i] = false;
		}
	}

	byte[] StepAndGetMap(byte[] obstacles)
	{
		mObstacles = obstacles;
		for (int i = 0; i < w * h; i++)
		{
			mMap [i] = 0;
			mVisited [i] = false;
		}

		mMap [mPlayerPos.x + mWidth * mPlayerPos.y] = 0;
		Queue<IntPoint> queue = new Queue<IntPoint> ();

		while (queue.Count > 0)
		{
			IntPoint p = queue.Dequeue();

			IntPoint[] neigh = GetNeigh (p);
			byte dist = mMap [p.x + mWidth * p.y];
			foreach (IntPoint n in neigh) 
			{
				if (!mVisited [n.x + mWidth * n.y]) 
				{
					mVisited [n.x + mWidth * n.y] = true;
					mMap [n.x + mWidth * n.y] = dist + 1;
					queue.Enqueue (n);
				}
			}
		}
		return mMap;
	}

	IntPoint[] GetNeigh(IntPoint p)
	{
		List<IntPoint> points = new List<IntPoint> ();
		if (p.x > 0 && mObstables [(p.x - 1) + mWidth * p.y] == 0)
			points.Add (new IntPoint (p.x - 1, p.y));
		if (p.y > 0 && mObstables [p.x + mWidth * (p.y-1)] == 0)
			points.Add (new IntPoint (p.x, p.y-1));
		if (p.x < mWidth - 1 && mObstables [(p.x + 1) + mWidth * p.y] == 0)
			points.Add (new IntPoint (p.x + 1, p.y));
		if (p.y < mHeight - 1 && mObstables [p.x + mWidth * (p.y+1)] == 0)
			points.Add (new IntPoint (p.x, p.y+1));		
		return points.ToArray();
	}
}

