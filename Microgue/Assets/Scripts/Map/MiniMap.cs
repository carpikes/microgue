using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RoomMapGenerator;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {
    private GameplayManager mGameManager;
    private RoomMap mMap = null;
    private MapGenerator mMapGen = null;
    private GameObject mTile, mBossTile;
    private GameObject mContainer;
    private Dictionary<int, GameObject> mTiles;

	// Use this for initialization
	void Start () {
        GameObject gm = GameObject.Find("GameplayManager");
        mGameManager = gm.GetComponent<GameplayManager>();

        mContainer = gameObject.transform.GetChild(0).gameObject;
        mTile = gameObject.transform.GetChild(1).gameObject;
        mBossTile = gameObject.transform.GetChild(2).gameObject;
        mTile.SetActive(false);
        mBossTile.SetActive(false);
        mTiles = new Dictionary<int, GameObject>();
	}

    private struct IntRect { public int xMin, yMin, xMax, yMax; }
    private void DrawMap(int curRoom) {
        IntRect r;
        r.xMin = r.yMin = 999;
        r.xMax = r.yMax = 0;

        mTiles.Clear();
        foreach (Transform t in mContainer.transform)
            Destroy(t.gameObject);

        if (curRoom == -1)
            return;

        int xCurRoom = curRoom % mMap.GetWidth(), yCurRoom = curRoom / mMap.GetWidth();
        for (int j = 0; j < mMap.GetWidth(); j++)
            for (int i = 0; i < mMap.GetHeight(); i++)
                if (mMap.GetDoors(j, i) != 0)
                {
                    if (j < r.xMin) r.xMin = j;
                    if (i < r.yMin) r.yMin = i;
                    if (j > r.xMax) r.xMax = j;
                    if (i > r.yMax) r.yMax = i;
                }


        xCurRoom -= r.xMin;
        yCurRoom -= r.yMin;

        float tileSize = 16;
        // xOff e yOff centrano la minimap
        float xOff = (xCurRoom * tileSize);
        float yOff = (yCurRoom * tileSize);

        for (int i = r.yMin; i <= r.yMax; i++)
            for (int j = r.xMin; j <= r.xMax; j++)
                if (mMap.GetDoors(j, i) != 0)
                {
                    GameObject tile = Instantiate(mTile, mContainer.transform) as GameObject;
                    // rimuovo l'offset della tilemap
                    int iv = i - r.yMin, jv = j - r.xMin;

                    Vector3 pos = new Vector3(tileSize * jv - xOff, -(tileSize * iv) + yOff,0);
                    tile.transform.position = mContainer.transform.position + pos;
                    tile.SetActive(true);
                    mTiles[j + i * mMap.GetWidth()] = tile;

                    // Disegno la stanza del boss!
                    if ((mMap.GetDoors(j, i) & (int)RoomMap.Door.END_POINT) != 0)
                    {
                        int bossDoor = mMapGen.GetRoom(j + i * mMap.GetWidth()).GetStartOrEndDoor();
                        int i2 = i, j2 = j;
                        switch (bossDoor)
                        {
                            case (int)RoomMap.Door.UP: i2--; break;
                            case (int)RoomMap.Door.DOWN: i2++; break;
                            case (int)RoomMap.Door.LEFT: j2--; break;
                            case (int)RoomMap.Door.RIGHT: j2++; break;
                            default: Debug.LogError("Wat.."); break;
                        }

                        tile = Instantiate(mBossTile, mContainer.transform) as GameObject;

                        iv = i2 - r.yMin;
                        jv = j2 - r.xMin;

                        pos = new Vector3(tileSize * jv - xOff, -(tileSize * iv) + yOff, 0);
                        tile.transform.position = mContainer.transform.position + pos;
                        tile.SetActive(true);
                        mTiles[j2 + i2 * mMap.GetWidth()] = tile;
                    }
                }
    }
	
    private int mLastRoom = -1;
	// Update is called once per frame
	void Update () {
        WorldManager wm = mGameManager.GetWorldManager();

        if (wm.GetMap() != null && ( mMap == null || mLastRoom != wm.GetCurrentRoomId()))
        {
            //Debug.Log("Disegno la minimap");
            mMap = wm.GetMap();
            mMapGen = wm.GetMapGenerator();
            DrawMap(wm.GetCurrentRoomId());
        }

        Color normalColor   = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        Color nearColor     = new Color(0.3f, 0.3f, 0.3f, 0.6f);
        Color inColor       = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        if (mMap != null && mLastRoom != wm.GetCurrentRoomId() && wm.GetCurrentRoomId() != -1)
        {
            mLastRoom = wm.GetCurrentRoomId();
            HashSet<int> nearIds = new HashSet<int>();

            int doors = mMap.GetDoors(mLastRoom);
            if ((doors & (int)RoomMap.Door.LEFT) != 0)  nearIds.Add(mLastRoom - 1);
            if ((doors & (int)RoomMap.Door.RIGHT) != 0) nearIds.Add(mLastRoom + 1);
            if ((doors & (int)RoomMap.Door.UP) != 0)    nearIds.Add(mLastRoom - mMap.GetWidth());
            if ((doors & (int)RoomMap.Door.DOWN) != 0)  nearIds.Add(mLastRoom + mMap.GetWidth());

            foreach (KeyValuePair<int, GameObject> i in mTiles)
            {
                Image r = i.Value.GetComponent<Image>();

                if (i.Value.CompareTag("BossTilePiece")) // stanza boss
                {
                    if ((mMap.GetDoors(mMapGen.GetEndRoomId()) & (int)RoomMap.Door.VISITED) != 0)
                        r.color = Color.white;
                    else
                        r.color = new Color(0, 0, 0, 0);
                }
                else if ((mMap.GetDoors(i.Key) & (int)RoomMap.Door.VISITED) != 0)
                {
                    if (i.Key == mLastRoom)
                        r.color = inColor;
                    else
                        r.color = normalColor;
                }
                else if(nearIds.Contains(i.Key))
                    r.color = nearColor;
                else
                    r.color = new Color(0, 0, 0, 0);
            }
        }
	}
}
