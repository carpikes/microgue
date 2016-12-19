using UnityEngine;
using System.Collections;
using System;

public class AIMap : MonoBehaviour
{
    GameplayManager mGameManager;
    GameObject mWorld = null;
    GameObject mPlayer = null;
    bool[,] mArea, mEnemies;
    int mMapRefreshes;
    static int mColTiles, mRowTiles;
    static float mMapWidthWC, mMapHeightWC;
    static float mTileWidthWC, mTileHeightWC;
    Bounds mWorldArea;
	private float mEnemyUpdateInterval = 0.1f;
	private float mNextEnemyUpdate = 0.0f;

    public GameObject mDebugTileForPlayerPos;

	public struct IntPoint 
	{ 
		public int x, y;
        public IntPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
	};

    public IntPoint GetPlayerPosition()
    {
        return WorldToTileCoordinates(mPlayer.transform.position);
    }

    public Vector2 GetWorldPosition(IntPoint pos)
    {
        return TileToWorldCoordinates(pos);
    }

	// Use this for initialization
	void Start () {
        mGameManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        mWorld = null;
        mPlayer = GameObject.Find("MainCharacter");
    }

    private void InitializeMapMeasures(Tiled2Unity.TiledMap map)
    {
        mColTiles = map.NumTilesWide;
        mRowTiles = map.NumTilesHigh;

        // 32 x 24 ok!
        //Debug.Log("DIMENSIONI MAPPA: " + mRowTiles + " " + mColTiles);

        mMapWidthWC = map.GetMapWidthInPixelsScaled();
        mMapHeightWC = map.GetMapHeightInPixelsScaled();

        // 10.24 x 7.68 ok!
        // Debug.Log("MAPPA IN WC " + mMapWidthWC + " , " + mMapHeightWC);

        mTileWidthWC = map.TileWidth * map.ExportScale;
        mTileHeightWC = map.TileHeight * map.ExportScale;

        // 0.32 0.32 ok!
        // Debug.Log("DIMENSIONI TILE IN WC: " + mTileWidthWC + " " + mTileHeightWC);

        mWorldArea = new Bounds(new Vector2(mMapWidthWC / 2, mMapHeightWC / 2), new Vector2(mMapWidthWC, mMapHeightWC));
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
		if (mWorld != null && mColTiles > 0 && mRowTiles > 0)
		{
            if (mNextEnemyUpdate < Time.time)
			{
                //ShowRedTilePlayer();
                UpdateEnemies();
				mNextEnemyUpdate = Time.time + mEnemyUpdateInterval;
			}
		}
	}

    private void UpdateEnemies()
    {
        EnemyPosition[] arr = GameObject.Find(mWorld.name + "/Enemies").GetComponentsInChildren<EnemyPosition>();

        for( int j = 0; j < mColTiles; ++j )
            for (int i = 0; i < mRowTiles; i++)
                mEnemies[i, j] = false;

        foreach (EnemyPosition pos in arr)
        {
            //Debug.Log(pos.IsOutOfTileMap());
            if (!pos.IsEnabled() || pos.IsOutOfTileMap())
                continue;

            IntPoint enemyTilePos = WorldToTileCoordinates(pos.GetWorldPosition());
            mEnemies[enemyTilePos.x, enemyTilePos.y] = true;
            //Debug.Log(enemyTilePos);
        }
    }

    private void ShowRedTile(IntPoint tilePos)
    {
        mDebugTileForPlayerPos.SetActive(true);
        mDebugTileForPlayerPos.transform.position = TileToWorldCoordinates(tilePos);
    }

    private void UpdateArea()
    {
        Tiled2Unity.TiledMap map = mWorld.GetComponent<Tiled2Unity.TiledMap>();
        EdgeCollider2D[] coll = GameObject.Find(mWorld.name + "/Collision").GetComponentsInChildren<EdgeCollider2D>();
        PolygonCollider2D[] poly = GameObject.Find(mWorld.name + "/Collision").GetComponentsInChildren<PolygonCollider2D>();
        BoxCollider2D[] box = GameObject.Find(mWorld.name + "/Doors").GetComponentsInChildren<BoxCollider2D>();

        // TODO si riesce a fixare?
        //Renderer r = GameObject.Find(mWorld.name + "/Background/water").GetComponent<Renderer>();

        if (map == null || coll == null || poly == null)
        {
            Debug.LogError("Cannot find map or colliders or 'water' layer in this map");
            return;
        }

        InitializeMapMeasures(map);

        mArea = new bool[mRowTiles, mColTiles];
        mEnemies = new bool[mRowTiles, mColTiles];

        for (int j = 0; j < mColTiles; j++)
        {
            for(int i = 0; i < mRowTiles; ++i)
            {
                mArea[i,j] = false;
                mEnemies[i, j] = false;
            }
        }

        foreach (EdgeCollider2D c in coll)
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

            //Debug.Log("DOOR: dl " + dl + " - ur " + ur);

            DrawRectangle(dl, ur);
        }

        mMapRefreshes++;
    }

    private void DrawLine(IntPoint s, IntPoint e)
    {
        if( Mathf.Abs(e.x - s.x) >= Mathf.Abs(e.y - s.y) )
        {
            // always start with s on the left
            if( s.x > e.x )
                Swap<IntPoint>(ref s, ref e);

            float y = s.y;
            float dy = (float)(e.y - s.y) / (e.x - s.x);

            for (int x = s.x; x <= e.x; ++x)
            {
                mArea[x, (int)Mathf.Round(y)] = true;
                y += dy;
            }             
        } else {
            if (s.y > e.y)
                Swap<IntPoint>(ref s, ref e);

            Debug.Log(s + " " + e);

            float x = s.x;
            float dx = (float)(e.x - s.x) / (e.y - s.y);

            for( int y = s.y; y <= e.y; y++ )
            {
                Debug.Log("DRAWING: " + x + " " + y);
                mArea[(int)Mathf.Round(x), y] = true;
                x += dx;
            }
        }
    }

    private void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }

    private void DrawRectangle(IntPoint p1, IntPoint p2)
    {
        int xMin = Mathf.Min(p1.x, p2.x);
        int xMax = Mathf.Max(p1.x, p2.x);

        int yMin = Mathf.Min(p1.y, p2.y);
        int yMax = Mathf.Max(p1.y, p2.y);

        for (int x = xMin; x <= xMax; x++)
            for (int y = yMin; y <= yMax; y++)
                mArea[x, y] = true;
    }

    private void DrawTriangle(IntPoint[] p)
    {
        Debug.Assert(p.Length == 3, "p must have 3 points!");

        DrawLine(p[0], p[1]);
        DrawLine(p[1], p[2]);
        DrawLine(p[2], p[0]);
    }

    public static Vector2 TileToWorldCoordinates(IntPoint p)
    {
        Debug.Assert(p.x >= 0 && p.x < mRowTiles && p.y >= 0 && p.y < mColTiles, "Invalid tile coords: " + p.x + ", " + p.y);

        // Return center of the tile
        // NB x and y are inverted, BEWARE!
        Vector2 result = new Vector2((p.y + 0.5f) * mTileHeightWC, -(p.x + 0.5f) * mTileWidthWC);
        return result;
    }

    public static IntPoint WorldToTileCoordinates(Vector2 p)
    {
        IntPoint tileCoords;

        // find tile and clamp to limits
        tileCoords.y = Mathf.FloorToInt(p.x / mTileWidthWC);
        tileCoords.x = Mathf.FloorToInt((-p.y) / mTileHeightWC); // NB the minus sign!

        tileCoords.y = Mathf.Clamp(tileCoords.y, 0, mColTiles - 1);
        tileCoords.x = Mathf.Clamp(tileCoords.x, 0, mRowTiles - 1);

        return tileCoords;
    }

    public bool[,] GetMap()
    {
        return mArea;
    }

    public bool[,] GetEnemies()
    {
        return mEnemies;
    }

    public int GetMapRefreshId()
    {
        return mMapRefreshes;
    }

    static public float GetWidth()
    {
        return mMapWidthWC;
    }

    static public float GetHeight()
    {
        return mMapHeightWC;
    }

    static public int GetNrTileColumns()
    {
        return mColTiles;
    }

    static public int GetNrTileRows()
    {
        return mRowTiles;
    }

}
