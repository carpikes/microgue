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
        int cols = AIMap.GetNrTileColumns();
        int rows = AIMap.GetNrTileRows();
        AIMap.IntPoint playerPosition = mAIMap.GetPlayerPosition();

        Texture2D texture = new Texture2D(cols,rows);

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
            {
                if (enemies[i, j])
                    texture.SetPixel(j, rows - i - 1, Color.red);
                else
                    texture.SetPixel(j, rows - i - 1, map[i, j] ? Color.white : Color.black);
            }

        texture.SetPixel(playerPosition.y, rows - playerPosition.x - 1, Color.cyan);
        texture.Apply();
        GetComponent<RawImage>().texture = texture;
    }
}
