using UnityEngine;
using System.Collections;
using System;

using IntPoint = AIMap.IntPoint;
using System.Collections.Generic;
using System.Linq;

public class LittleJimmy : MonoBehaviour
{
    private AIMap mAIMap;
    private Rigidbody2D mRb;
    private Rigidbody2D mPlayerRb;

    private IntPoint mNextPosition;
    private EnemyPosition mEnemyPosition;

    int cnt = 0;

    void Start()
    {
        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        mRb = GetComponent<Rigidbody2D>();
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
    }

    void FixedUpdate()
    {
        ++cnt;

        if (mAIMap.IsMapReady && cnt % 5 == 0)
        {
            cnt = 0;
            UpdatePosition();
        }

        mEnemyPosition.SetWorldPosition(mRb.position);
    }

    private void UpdatePosition()
    {
        int[,] bfsMap = null;

        IntPoint pos = AIMap.WorldToTileCoordinates(mPlayerRb.position);
        bfsMap = BFS.CalculateBFSMap(mAIMap.GetMap(), pos);

        // look for neighbours
        mNextPosition = PickMaxNeighbour(bfsMap, AIMap.WorldToTileCoordinates(mRb.position));

        mRb.position = AIMap.TileToWorldCoordinates(mNextPosition);
    }

    private IntPoint PickMaxNeighbour(int[,] map, IntPoint pos)
    {
        int x = pos.x;
        int y = pos.y;

        IntPoint maxPos = new IntPoint(-5, -10);
        int max = -1;

        Dictionary<IntPoint, int> v = new Dictionary<AIMap.IntPoint, int>();
        for (int i = -1; i <= 1; ++i)
            for (int j = -1; j <= 1; ++j)
                if (IsInside(map, x + i, y + j)) v.Add(new IntPoint(x + i, y + j), map[x + i, y + j]);

        if (v.Count == 0)
            return pos;

        foreach (var pair in v)
            if (pair.Value > max)
            {
                max = pair.Value;
                maxPos = pair.Key;
            }

        return maxPos;
    }

    private bool IsInside(int[,] map, int x, int y)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        return (x >= 0 && x < rows) && (y >= 0 && y < cols);
    }
}