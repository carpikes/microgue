﻿using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;
using IntPoint = AIMap.IntPoint;

public class LittleJimmy : MonoBehaviour
{
    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
	private EnemyAI mEnemyAI;
    private AIMap mAIMap;
    private BFSMap mBFS;
    private byte[] mAwayMap = null;
    private byte[] mObstacleMap = null;
    
    // Use this for initialization
    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
		mEnemyAI = GetComponent<EnemyAI>();
		mEnemyAI.SetEnabled(true);
    }

    float wait = 0;

    public byte[] GetMap()
    {
        return mAwayMap;
    }

    void Update()
    {
        transform.localScale = new Vector3(mRb.position.x >= mPlayerRb.position.x ? 1 : -1, 1, 1);

        if (mEnemyAI != null)
			mEnemyAI.SetPosition (transform.position);

        if (wait < Time.time)
        {
            lol();
            wait = Time.time + 0.3f;
        }

        if (mAwayMap != null)
        {
            int w = mAIMap.GetWidth();
            int h = mAIMap.GetHeight();
            IntPoint p = mAIMap.GetPosition(mRb.position);

            int q = mAwayMap[p.x + w * p.y];
            IntPoint where = p;

            if (p.x < w-1 && mAwayMap[p.x + 1 + w * p.y]   <= q && mObstacleMap[p.x + 1 + w * p.y]   == 0) { q = mAwayMap[p.x + 1 + w * p.y]; where = new IntPoint(p.x + 1, p.y); }
            if (p.x > 0   && mAwayMap[p.x - 1 + w * p.y]   <= q && mObstacleMap[p.x - 1 + w * p.y]   == 0) { q = mAwayMap[p.x - 1 + w * p.y]; where = new IntPoint(p.x - 1, p.y); }
            if (p.y < h-1 && mAwayMap[p.x + w * (p.y + 1)] <= q && mObstacleMap[p.x + w * (p.y + 1)] == 0) { q = mAwayMap[p.x + w * (p.y+1)]; where = new IntPoint(p.x, p.y+1); }
            if (p.y > 0   && mAwayMap[p.x + w * (p.y - 1)] <= q && mObstacleMap[p.x + w * (p.y - 1)] == 0) { q = mAwayMap[p.x + w * (p.y-1)]; where = new IntPoint(p.x, p.y-1); }

            Debug.Log("Passo dal tile" + p.x + "," + p.y + " al " + where.x + "," + where.y);

            Vector2 whereWS = mAIMap.GetWorldPosition(where);
            Vector2 dir = (whereWS - mRb.position);

            if (p.x != where.x || p.y != where.y)
            {
                Vector2 pos = transform.position;
                pos += dir.normalized * 1.0f * Time.fixedDeltaTime;
                mRb.position = pos;
            }
        }
    }

    void lol()
    {
        if (mAIMap.GetWidth() <= 0)
            return;

        mBFS = new BFSMap(mAIMap.GetWidth(), mAIMap.GetHeight());
        IntPoint player = mAIMap.GetPosition(mTarget.transform.position);

        byte[] m1 = mAIMap.GetEnemies(), m2 = mAIMap.GetMap();
        byte[] mm = new byte[m1.Length];
        for (int i = 0; i < m1.Length; i++)
            mm[i] = (m1[i] != 0) || (m2[i] != 0) ? (byte)1 : (byte)0;
        mObstacleMap = mm;

        byte[] map = mBFS.StepAndGetMap(player, mm);
        for (int i = 0; i < map.Length; i++)
        {
            if (mm[i] == 0)
                map[i] = (byte)(255 - map[i]);
            else
                map[i] = 255;
        }

        map = mBFS.StepAndGetMap(new IntPoint(-1,-1), mm);
        mAwayMap = map;
    }
    void ChooseNewPoint()
    {
        
    }

    void Print(byte[] map, int w, int h)
    {
        string str = "";
        for (int y = h-1; y >= 0; y--)
        {
            for (int x = 0; x < w; x++)
                str += string.Format("{0,2:X}", map[x + y * w]);
            str += "\n";
        }
        Debug.Log(str);
    }
}
