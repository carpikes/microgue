using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour {
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/";
    public const string DEBUG_ARENA = PREFAB_PATH + "arena.prefab";

    public GameObject mPlayer;
    public GameObject mAimCursor;
    public bool mDebugArena = false;

    private int mNumLevels;
    private int mCurLevelNum;
    private Level[] mLevels;

    List<string> mAvailableLevels;

    // Use this for initialization
    void Start ()
    {
        Cursor.visible = false;

        FetchLevels();
        GenerateLevels(3, 8);
        LoadFirstLevel();

        // REFACTOR: notifica caricamento primo livello tramite evento
        MovePlayerTo(mLevels[mCurLevelNum].GetPlayerStartPos("Spawn"));
    }

    private void LoadFirstLevel()
    {
        mCurLevelNum = 0;
        mLevels[mCurLevelNum].Load();
    }

    private void GenerateLevels(int min, int max)
    {
        mNumLevels = Random.Range(min, max);
        mLevels = new Level[mNumLevels];

        for (int i = 0; i < mNumLevels; i++)
            mLevels[i] = new Level(i + 1, mAvailableLevels[Random.Range(0, mAvailableLevels.Count)]);

        Debug.Log("NumLevels: " + mNumLevels);
    }

    private void FetchLevels()
    {
        mAvailableLevels = new List<string>();

        if( !mDebugArena ) { 
            foreach (string file in Directory.GetFiles(PREFAB_PATH))
            {
                if (file.EndsWith(".prefab") && file != DEBUG_ARENA)
                {
                    Debug.Log(file);
                    mAvailableLevels.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        } else
        {
            mAvailableLevels.Add(Path.GetFileNameWithoutExtension(DEBUG_ARENA));
        }
    }

    // REFACTOR
    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        p.MovePosition(coords);
    }

    // REFACTOR
    public void OnDoorEnter(string type) {
        if (mCurLevelNum == 0 && type == "Down")
            return;
        if (mCurLevelNum == mNumLevels - 1 && type == "Up")
            return;

        mLevels[mCurLevelNum].Unload();
        switch (type) {
            case "Down": mLevels[--mCurLevelNum].Load(); break;
            case "Up": mLevels[++mCurLevelNum].Load(); break;
            default: return;
        }
        //Debug.Log("Loading Level: " + type + " - " + mCurLevelNum);

        // TODO sistemare questo if temporaneo
        MovePlayerTo(mLevels[mCurLevelNum].GetPlayerStartPos(type == "Down" ? "Up" : "Down"));
    }

    // REFACTOR
    public Vector3[] GetCameraBounds() {
        if (mLevels == null)
            return null;
        return mLevels[mCurLevelNum].GetCameraBounds();
    }
}
