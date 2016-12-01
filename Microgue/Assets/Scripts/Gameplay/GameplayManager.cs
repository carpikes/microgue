using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class GameplayManager : MonoBehaviour
{
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/";
    public const string DEBUG_ARENA = PREFAB_PATH + "arena.prefab";

    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool mDebugArena = false;

    private int mNumLevels;
    private int mCurLevelNum;
    private Level[] mLevels;

    List<string> mAvailableLevels;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_DOOR_TOUCH, OnDoorEnter);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_DOOR_TOUCH, OnDoorEnter);
    }

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;

        FetchLevels();
        GenerateLevels(3, 8);
        LoadFirstLevel();
    }

    private void LoadFirstLevel()
    {
        mCurLevelNum = 0;
        mLevels[mCurLevelNum].Load();

        // TODO?
        MovePlayerTo(mLevels[mCurLevelNum].GetPlayerStartPos("Down"));
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

        if (!mDebugArena)
        {
            foreach (string file in Directory.GetFiles(PREFAB_PATH))
            {
                if (file.EndsWith(".prefab") && file != DEBUG_ARENA)
                {
                    Debug.Log(file);
                    mAvailableLevels.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        else
        {
            mAvailableLevels.Add(Path.GetFileNameWithoutExtension(DEBUG_ARENA));
        }
    }

    public void OnDoorEnter(Bundle args)
    {
        if (mDebugArena)
            return;

        string type;

        if (!args.TryGetValue(DoorBehavior.DOOR_TYPE_TAG, out type))
            return;

        if (mCurLevelNum == 0 && type == DoorBehavior.DOOR_DOWN)
            return;
        if (mCurLevelNum == mNumLevels - 1 && type == DoorBehavior.DOOR_UP)
            return;

        mLevels[mCurLevelNum].Unload();

        Debug.Log(type);
        switch (type)
        {
            case DoorBehavior.DOOR_DOWN: mLevels[--mCurLevelNum].Load(); break;
            case DoorBehavior.DOOR_UP: mLevels[++mCurLevelNum].Load(); break;
            default: return;
        }

        // TODO sistemare questo if temporaneo
        MovePlayerTo(mLevels[mCurLevelNum].GetPlayerStartPos(
            type == DoorBehavior.DOOR_DOWN
            ? DoorBehavior.DOOR_UP
            : DoorBehavior.DOOR_DOWN));
    }

    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        p.MovePosition(coords);
    }

    public Vector3[] GetCameraBounds()
    {
        if (mLevels == null)
            return null;
        return mLevels[mCurLevelNum].GetCameraBounds();
    }
}
