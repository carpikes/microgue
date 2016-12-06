using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RoomMapGenerator;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {
    private GameplayManager mGameManager;
    private RoomMap mMap = null;
    private GameObject mTile;
    private GameObject mContainer;
    private Dictionary<int, GameObject> mTiles;

	// Use this for initialization
	void Start () {
        GameObject gm = GameObject.Find("GameplayManager");
        mGameManager = gm.GetComponent<GameplayManager>();

        mContainer = gameObject.transform.GetChild(0).gameObject;
        mTile = gameObject.transform.GetChild(1).gameObject;
        mTile.SetActive(false);
        mTiles = new Dictionary<int, GameObject>();
	}

    private struct IntRect { public int xMin, yMin, xMax, yMax; }
    private void DrawMap() {
        IntRect r;
        r.xMin = r.yMin = 999;
        r.xMax = r.yMax = 0;


        for (int j = 0; j < mMap.GetWidth(); j++)
            for (int i = 0; i < mMap.GetHeight(); i++)
                if (mMap.GetDoors(j, i) != 0)
                {
                    if (j < r.xMin) r.xMin = j;
                    if (i < r.yMin) r.yMin = i;
                    if (j > r.xMax) r.xMax = j;
                    if (i > r.yMax) r.yMax = i;
                }

        for (int i = r.yMin; i <= r.yMax; i++)
            for (int j = r.xMin; j <= r.xMax; j++)
                if (mMap.GetDoors(j, i) != 0)
                {
                    GameObject tile = Instantiate(mTile, mContainer.transform) as GameObject;
                    int iv = i - r.yMin, jv = j - r.xMin;
                    Vector3 pos = new Vector3(16 * jv, -(16 * iv),0);
                    tile.transform.position = mContainer.transform.position + pos;
                    tile.SetActive(true);
                    mTiles[j + i * mMap.GetWidth()] = tile;
                    Debug.Log("Aggiungo il tile" + j + "," + i);
                }
    }
	
    private int mLastRoom = -1;
	// Update is called once per frame
	void Update () {
        //int n = mGameManager.GetCurrentRoomId();
        //int x = n % mMap.GetWidth();
        //int y = n / mMap.GetWidth();

        if (mMap == null && mGameManager.GetMap() != null)
        {
            Debug.Log("Disegno la minimap");
            mMap = mGameManager.GetMap();
            DrawMap();
        }

        if (mMap != null && mLastRoom != mGameManager.GetCurrentRoomId())
        {
            mLastRoom = mGameManager.GetCurrentRoomId();
            foreach (KeyValuePair<int, GameObject> i in mTiles)
            {
                Image r = i.Value.GetComponent<Image>();
                if (i.Key == mLastRoom)
                    r.color = Color.white;
                else
                    r.color = Color.blue;
            }
        }
	}
}
