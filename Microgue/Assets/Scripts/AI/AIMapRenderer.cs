using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIMapRenderer : MonoBehaviour {
    private AIMap mAIMap;
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
        UpdateMap();
	}

    void UpdateMap() {
        byte[] map = mAIMap.GetMap();
        byte[] enemies = mAIMap.GetEnemies();
        int w = mAIMap.GetWidth();
        int h = mAIMap.GetHeight();
        Texture2D texture = new Texture2D(w,h);
        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
            {
                if (enemies[i * w + j] == 1)
                    texture.SetPixel(j, i, Color.red);
                else
                    texture.SetPixel(j, i, map[i * w + j] == 1 ? Color.white : Color.black);
            }
        texture.Apply();
        GetComponent<RawImage>().texture = texture;
    }
}
