using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;
using UnityEditor;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour {
    public GameObject mPlayer;
    public GameObject mAimCursor;

    private int mNumLevels;
    private int mCurLevelNum;
    private Level[] mWorlds;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;

        string[] mAvailWorlds = { "ex1", "ex2" };

        mNumLevels = Random.Range(3, 8);

        mWorlds = new Level[mNumLevels];

        for (int i = 0; i < mNumLevels; i++)
            mWorlds[i] = new Level(i + 1, mAvailWorlds[Random.Range(0, mAvailWorlds.Length)]);
        mCurLevelNum = 0;

        Debug.Log("NumLevels: " + mNumLevels);
        mWorlds[mCurLevelNum].Load();
        MovePlayerTo(mWorlds[mCurLevelNum].GetPlayerStartPos("Spawn"));
    }

    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        p.MovePosition(coords);
    }

    public void OnDoorEnter(string type) {
        if (mCurLevelNum == 0 && type == "Down")
            return;
        if (mCurLevelNum == mNumLevels - 1 && type == "Up")
            return;

        mWorlds[mCurLevelNum].Unload();
        switch (type) {
            case "Down": mWorlds[--mCurLevelNum].Load(); break;
            case "Up": mWorlds[++mCurLevelNum].Load(); break;
            default: return;
        }
        Debug.Log("Loading Level: " + type + " - " + mCurLevelNum);

        // TODO sistemare questo if temporaneo
        MovePlayerTo(mWorlds[mCurLevelNum].GetPlayerStartPos(type == "Down" ? "Up" : "Down"));
    }

    public Vector3[] GetCameraBounds() {
        if (mWorlds == null)
            return null;
        return mWorlds[mCurLevelNum].GetCameraBounds();
    }
}
