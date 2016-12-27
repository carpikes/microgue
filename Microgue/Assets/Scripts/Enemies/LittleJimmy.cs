using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

using IntPoint = AIMap.IntPoint;
using System.Collections.Generic;

public class LittleJimmy : MonoBehaviour
{
    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private AIMap mAIMap;
    private byte[] mAwayMap = null;
    private byte[] mObstacleMap = null;

    private EnemyPosition mEnemyPosition;

    public int scale;

    // Use this for initialization
    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
        dist = Random.Range(2.0f, 3.0f);

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);
    }

    float wait = 0;

    public byte[] GetMap()
    {
        return mAwayMap;
    }

    IntPoint lastTile;
    bool ltset = false;

    public float Kconst = 8.0f;
    public float Friction = 0.05f;

    float timeout = 0;
    float dist, newdist;
    float tget;
    float newtget;
    Vector2 mSpeed = Vector2.zero;

    void OnTriggerEnter2D(Collider2D other)
    {
        timeout = 0;
    }

    void FixedUpdate()
    {
        SpringMove();
        mEnemyPosition.SetWorldPosition(mRb.position);
    }

    private void SpringMove()
    {
        if (Time.time > timeout)
        {
            timeout = Time.time + Random.Range(2, 3);
            newtget = Random.Range(0, 2.0f * Mathf.PI);
            newdist = Random.Range(2.0f, 3.0f);
        }

        if (newtget != tget)
            tget = Mathf.LerpAngle(tget, newtget, Time.fixedDeltaTime);
        if (newdist != dist)
            dist = Mathf.Lerp(newdist, dist, Time.fixedDeltaTime);

        Vector2 delta = mTarget.transform.position - transform.position;
        delta += dist * new Vector2(Mathf.Cos(tget), Mathf.Sin(tget));

        float dl = delta.magnitude;
        delta.Normalize();

        mSpeed += delta * dl * Kconst * Time.fixedDeltaTime;
        mSpeed *= (1.0f - Friction);

        Vector2 t = transform.position;
        t += mSpeed * Time.fixedDeltaTime;
        transform.position = t;

        transform.localScale = new Vector3(mRb.position.x >= mPlayerRb.position.x ? scale : -scale, scale, scale);
    }

    /*
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
            if (!ltset)
            {
                lastTile = mAIMap.GetPosition(mRb.position);
                ltset = true;
            }
            int q = mAwayMap[lastTile.x + w * lastTile.y];
            IntPoint where = lastTile;
            IntPoint p = lastTile;
            if (p.x < w-1 && mAwayMap[p.x + 1 + w * p.y]   <= q && mObstacleMap[p.x + 1 + w * p.y]   == 0) { q = mAwayMap[p.x + 1 + w * p.y]; where = new IntPoint(p.x + 1, p.y); }
            if (p.x > 0   && mAwayMap[p.x - 1 + w * p.y]   <= q && mObstacleMap[p.x - 1 + w * p.y]   == 0) { q = mAwayMap[p.x - 1 + w * p.y]; where = new IntPoint(p.x - 1, p.y); }
            if (p.y < h-1 && mAwayMap[p.x + w * (p.y + 1)] <= q && mObstacleMap[p.x + w * (p.y + 1)] == 0) { q = mAwayMap[p.x + w * (p.y+1)]; where = new IntPoint(p.x, p.y+1); }
            if (p.y > 0   && mAwayMap[p.x + w * (p.y - 1)] <= q && mObstacleMap[p.x + w * (p.y - 1)] == 0) { q = mAwayMap[p.x + w * (p.y-1)]; where = new IntPoint(p.x, p.y-1); }
            //Debug.Log("Passo dal tile" + p.x + "," + p.y + " al " + where.x + "," + where.y);
            Vector2 whereWS = mAIMap.GetWorldPosition(where);
            Vector2 dir = (whereWS - mRb.position);
            if (lastTile.x != where.x || lastTile.y != where.y)
            {
                //Vector2 pos = transform.position;
                //pos += dir.normalized * 3.0f * Time.deltaTime;
                mRb.position = mAIMap.GetWorldPosition(where);
                lastTile = where;
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
    */
}