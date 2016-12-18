using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIMapRenderer : MonoBehaviour {
    private AIMap mAIMap;
	private float mTimer = 0;
	private float mUpdateSeconds = 0.1f;

	// Use this for initialization
	void Start () {
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
	}

    int mId = -1;
	// Update is called once per frame
	void Update () {
        if (mAIMap.GetMapRefreshId() != mId)
        {
            mId = mAIMap.GetMapRefreshId();
            UpdateMap();
        }

		if (mId != -1 && mTimer < Time.time)
		{
			UpdateMap ();
			mTimer = Time.time + mUpdateSeconds;
		}
	}

    void UpdateMap()
	{
        bool[,] map = mAIMap.GetMap();
        bool[,] enemies = mAIMap.GetEnemies();
        int w = AIMap.GetNrTileColumns();
        int h = AIMap.GetNrTileRows();

        Texture2D texture = new Texture2D(w,h);

        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
            {
                if (enemies[i, j])
                    texture.SetPixel(j, i, Color.red);
                else
                    texture.SetPixel(j, i, map[i, j] ? Color.white : Color.black);
            }
        texture.Apply();
        GetComponent<RawImage>().texture = texture;
    }
}
