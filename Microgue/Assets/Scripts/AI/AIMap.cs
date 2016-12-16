﻿using UnityEngine;
using System.Collections;

public class AIMap : MonoBehaviour
{
    GameplayManager mGameManager;
    GameObject mWorld = null;
    GameObject mPlayer = null;
    byte[] mArea, mEnemies;
    int mMapRefreshes;
    int mWidth, mHeight;
    Bounds mWorldArea;
	private float mEnemyUpdateInterval = 0.1f;
	private float mNextEnemyUpdate = 0.0f;

	public struct IntPoint 
	{ 
		public int x, y; 
		public IntPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	};

    public IntPoint GetPlayerPosition()
    {
        return GetTilePosition(mPlayer.transform.position);
    }

    public IntPoint GetTilePosition(Vector2 pos)
    {
        return WorldToTileCoordinates(pos);
    }

    public Vector2 GetWorldPosition(IntPoint pos)
    {
        return TileToWorldCoordinates(pos);
    }

    public byte[] GetMap()
    {
        return mArea;
    }

    public byte[] GetEnemies()
    {
        return mEnemies;
    }

    public int GetMapRefreshId()
    {
        return mMapRefreshes;
    }

    public int GetWidth()
    {
        return mWidth;
    }

    public int GetHeight()
    {
        return mHeight;
    }

	// Use this for initialization
	void Start () {
        mGameManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        mWorld = null;
        mPlayer = GameObject.Find("MainCharacter");
	}
		
	// Update is called once per frame
	void Update () {
        GameObject world = mGameManager.GetWorld();

        // TODO EVENTO!!
        if (world != mWorld)
        {
            mWorld = world;
            UpdateArea();
        }
		if (mWorld != null && mWidth > 0 && mHeight > 0)
		{
			if (mNextEnemyUpdate < Time.time)
			{
				UpdateEnemies();
				mNextEnemyUpdate = Time.time + mEnemyUpdateInterval;
			}
		}
	}

    private void UpdateEnemies()
    {
		EnemyPosition[] arr = GameObject.Find(mWorld.name + "/Enemies").GetComponentsInChildren<EnemyPosition>();

        for (int i = 0; i < mWidth * mHeight; i++)
            mEnemies[i] = 0;

		foreach(EnemyPosition pos in arr)
        {
			if (!pos.IsEnabled ())
				continue;
			IntPoint p = WorldToTileCoordinates(pos.GetPosition());
            mEnemies[p.x + mWidth * p.y] = 1;
        }
    }

    private void UpdateArea()
    {
        Tiled2Unity.TiledMap map = mWorld.GetComponent<Tiled2Unity.TiledMap>();
        EdgeCollider2D[] coll = GameObject.Find(mWorld.name + "/Collision").GetComponentsInChildren<EdgeCollider2D>();
        PolygonCollider2D[] poly = GameObject.Find(mWorld.name + "/Collision").GetComponentsInChildren<PolygonCollider2D>();
        BoxCollider2D[] box = GameObject.Find(mWorld.name + "/Doors").GetComponentsInChildren<BoxCollider2D>();
        
        // TODO si riesce a fixare?
        Renderer r = GameObject.Find(mWorld.name + "/Background/water").GetComponent<Renderer>();

        if (map == null || coll == null || r == null || poly == null)
        {
            Debug.LogError("Cannot find map or colliders or 'water' layer in this map");
            return;
        }

        mWidth = map.TileWidth;
        mHeight = map.TileHeight;
        mWorldArea = r.bounds;
        mArea = new byte[mWidth * mHeight];
        mEnemies = new byte[mWidth * mHeight];

        for (int i = 0; i < mWidth * mHeight; i++)
        {
            mArea[i] = 0;
            mEnemies[i] = 0;
        }

        foreach(EdgeCollider2D c in coll)
        {
            Vector2[] p = c.points;
            for (int i = 0; i < p.Length; i++)
            {
                Vector2 other = p[(i + 1) % p.Length];
                DrawLine(WorldToTileCoordinates(p[i]), WorldToTileCoordinates(other));
            }
        }

        foreach (PolygonCollider2D c in poly)
        {
            Vector2[] p = c.points;
            for (int i = 0; i < p.Length; i++)
            {
                Vector2 other = p[(i + 1) % p.Length];
                DrawLine(WorldToTileCoordinates(p[i]), WorldToTileCoordinates(other));
            }
        }

        foreach (BoxCollider2D c in box)
        {
            IntPoint dl = WorldToTileCoordinates(c.bounds.min);
            IntPoint ur = WorldToTileCoordinates(c.bounds.max);

            BlackRect(dl, ur);
        }

        mMapRefreshes++;
    }

    private void BlackRect(IntPoint dl, IntPoint ur)
    {
        for (int x = dl.x; x <= ur.x; x++)
            for (int y = dl.y; y <= ur.y; y++)
                mArea[x + y * mWidth] = 1;
    }

    private void BlackTri(IntPoint[] p)
    {
        if (p.Length != 3)
        {
            Debug.LogError("p must have 3 points");
            return;
        }

        DrawLine(p[0], p[1]);
        DrawLine(p[1], p[2]);
        DrawLine(p[2], p[0]);
    }

    /* TODO MICHELE */
    private Vector2 TileToWorldCoordinates(IntPoint p)
    {
        Vector2 ret;
        float dx = mWorldArea.max.x - mWorldArea.min.x;
        float dy = mWorldArea.max.y - mWorldArea.min.y;
        float tsize = dx / mWidth / 2.0f;

        ret.x = (((float)p.x / (mWidth-1)) * dx) + mWorldArea.min.x + tsize;
        ret.y = (((float)p.y / (mHeight-1)) * dy) + mWorldArea.min.y + tsize;

        //IntPoint test = toMap(ret);
        //Debug.Log(string.Format("FromMap: {0},{1} -> {2},{3}; ToMap: {4},{5}", p.x, p.y, ret.x, ret.y, test.x, test.y));
        return ret;
    }

    private IntPoint WorldToTileCoordinates(Vector2 p)
    {
        IntPoint ret;
        float dx = mWorldArea.max.x - mWorldArea.min.x;
        float dy = mWorldArea.max.y - mWorldArea.min.y;
        ret.x = Mathf.FloorToInt(((p.x - mWorldArea.min.x) / dx) * (mWidth-1));
        ret.y = Mathf.FloorToInt(((p.y - mWorldArea.min.y) / dy) * (mHeight-1));

        if (ret.x >= mWidth) ret.x = mWidth - 1;
        if (ret.y >= mHeight) ret.y = mHeight - 1;
        if (ret.x < 0) ret.x = 0;
        if (ret.y < 0) ret.y = 0;
        return ret;
    }

    private void DrawLine(IntPoint s, IntPoint e)
    {
        if (s.y == e.y)
        {
            for(int x = s.x; x <= e.x; x++)
                mArea[x + s.y * mWidth] = 1;
            return;
        }

        if (s.x == e.x)
        {
            for(int y = s.y; y <= e.y; y++)
                mArea[s.x + y * mWidth] = 1;
            return;
        }

        float dy = e.y - s.y;
        float dx = e.x - s.x;

        float d = dy / dx;

        for (int x = s.x; x <= e.x; x++)
        {
            float y = s.y + (x - s.x) * d;
            mArea[x + (int)y * mWidth] = 1;
        }
    }
}
