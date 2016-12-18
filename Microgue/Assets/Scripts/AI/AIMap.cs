using UnityEngine;
using System.Collections;

public class AIMap : MonoBehaviour
{
    GameplayManager mGameManager;
    GameObject mWorld = null;
    GameObject mPlayer = null;
    byte[] mArea, mEnemies;
    int mMapRefreshes;
    int mRowTiles, mColTiles;
    float mMapWidthWC, mMapHeightWC;
    float mTileWidthWC, mTileHeightWC;
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

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
	};

    public IntPoint GetPlayerPosition()
    {
        return GetTilePosition(mPlayer.transform.position);
    }

    public IntPoint GetTilePosition(Vector2 pos)
    {
        Debug.Log(pos + ", " + WorldToTileCoordinates(pos).x + "-" + WorldToTileCoordinates(pos).y + ", "); // + TileToWorldCoordinates(WorldToTileCoordinates(pos)));
        return WorldToTileCoordinates(pos);
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
        mRowTiles = map.NumTilesWide;
        mColTiles = map.NumTilesHigh;

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
		if (mWorld != null && mRowTiles > 0 && mColTiles > 0)
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
		/*EnemyPosition[] arr = GameObject.Find(mWorld.name + "/Enemies").GetComponentsInChildren<EnemyPosition>();

        for (int i = 0; i < mWidth * mHeight; i++)
            mEnemies[i] = 0;

		foreach(EnemyPosition pos in arr)
        {
			if (!pos.IsEnabled ())
				continue;
			IntPoint p = WorldToTileCoordinates(pos.GetPosition());
            mEnemies[p.x + mWidth * p.y] = 1;
        }*/
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

        mArea = new byte[mRowTiles * mColTiles];
        mEnemies = new byte[mRowTiles * mColTiles];

        for (int i = 0; i < mRowTiles * mColTiles; i++)
        {
            mArea[i] = 0;
            mEnemies[i] = 0;
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

            BlackRect(dl, ur);
        }

        mMapRefreshes++;
    }

    private void DrawLine(IntPoint s, IntPoint e)
    {
        if (s.y == e.y)
        {
            for (int x = s.x; x <= e.x; x++)
                mArea[x + s.y * mRowTiles] = 1;
            return;
        }

        if (s.x == e.x)
        {
            for (int y = s.y; y <= e.y; y++)
                mArea[s.x + y * mRowTiles] = 1;
            return;
        }

        float dy = e.y - s.y;
        float dx = e.x - s.x;

        float d = dy / dx;

        for (int x = s.x; x <= e.x; x++)
        {
            float y = s.y + (x - s.x) * d;
            mArea[x + (int)y * mRowTiles] = 1;
        }
    }

    private void BlackRect(IntPoint dl, IntPoint ur)
    {
        for (int x = dl.x; x <= ur.x; x++)
            for (int y = dl.y; y <= ur.y; y++)
                mArea[x + y * mRowTiles] = 1;
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
        Debug.Assert(p.x >= 0 && p.x < mRowTiles && p.y >= 0 && p.y < mColTiles, "Invalid tile coords: " + p.x + ", " + p.y);

        Vector2 result = new Vector2((p.x + 0.5f) * mTileWidthWC, (p.y + 0.5f) * mTileHeightWC);
        Debug.Log(result);
        return result;
    }

    private IntPoint WorldToTileCoordinates(Vector2 p)
    {
        IntPoint tileCoords;

        // find tile and clamp to limits
        tileCoords.y = Mathf.FloorToInt(p.x / mTileWidthWC);
        tileCoords.x = Mathf.FloorToInt((-p.y) / mTileHeightWC); // NB the minus sign!

        tileCoords.y = Mathf.Clamp(tileCoords.y, 0, mRowTiles - 1);
        tileCoords.x = Mathf.Clamp(tileCoords.x, 0, mColTiles - 1);

        return tileCoords;
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
        return mRowTiles;
    }

    public int GetHeight()
    {
        return mColTiles;
    }

}
