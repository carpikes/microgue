﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RoomMapGenerator;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class GameplayManager : MonoBehaviour
{
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/";

    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool mDebugArena = false;

    private MapGenerator mMapGenerator;
    private MapAssetManager mMapAssetManager;

    // "Single map" vars
    private Dictionary<int, Level> mWorlds;
    private RoomInfo mCurRoom;
    private Level mCurWorld;
    private int mCurWorldId;

    // list of available .prefab files
    private List<string> mAvailableLevels;

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

        string lname = "demo";
        if (mDebugArena)
            lname = "debug";

        // TRIGGER EVENT MAP_LOADING_STARTED
        Debug.Log("Loading Started");
        FetchLevels(lname);
        mMapGenerator = new MapGenerator();
        mMapAssetManager = new MapAssetManager(mAvailableLevels);
        mWorlds = new Dictionary<int, Level>();
        mMapGenerator.GenerateMap();

        LoadWorld(mMapGenerator.GetStartRoomId());
        Debug.Log("Loading Completed");
        // TRIGGER EVENT MAP_LOADING_COMPLETED
    }

    private void LoadWorld(int n)
    {
        if (n == mCurWorldId)
            return;

        if(mCurWorld != null)
            mCurWorld.Unload();

        mCurRoom = mMapGenerator.GetRoom(n);

        if (!mWorlds.ContainsKey(n))
        {
            Level l = new Level(n, mMapAssetManager.GetMap(n, mCurRoom.GetDoors()));
            mWorlds.Add(n, l);
            mCurWorld = l;
        }

        mCurWorld = mWorlds[n];
        mCurWorldId = n;

        mCurWorld.Load();

        string door = DoorNumToString(mCurRoom.GetStartOrEndDoor());
        MovePlayerTo(mCurWorld.GetPlayerStartPos(door));
    }

    private void FetchLevels(string name)
    {
        mAvailableLevels = new List<string>();

        foreach (string file in Directory.GetFiles(PREFAB_PATH))
        {
            string fname = Path.GetFileNameWithoutExtension(file);
            if (file.EndsWith(".prefab")) // && fname.StartsWith(name + "_"))
            {
                Debug.Log(file);
                mAvailableLevels.Add(fname);
            }
        }
    }

    private string DoorNumToString(int door)
    {
        switch (door)
        {
            case (int) RoomMap.Door.DOWN:  return DoorBehavior.DOOR_DOWN;
            case (int) RoomMap.Door.UP:    return DoorBehavior.DOOR_UP;
            case (int) RoomMap.Door.LEFT:  return DoorBehavior.DOOR_LEFT;
            case (int) RoomMap.Door.RIGHT: return DoorBehavior.DOOR_RIGHT;
        }

        return "";
    }

    public void OnDoorEnter(Bundle args)
    {
        if (mDebugArena)
            return;

        string type, opposite;
        RoomMap.Door door;

        if (!args.TryGetValue(DoorBehavior.DOOR_TYPE_TAG, out type))
            return;

        switch (type) {
            case DoorBehavior.DOOR_DOWN:  door = RoomMap.Door.DOWN;  opposite = DoorBehavior.DOOR_UP;    break;
            case DoorBehavior.DOOR_UP:    door = RoomMap.Door.UP;    opposite = DoorBehavior.DOOR_DOWN;  break;
            case DoorBehavior.DOOR_LEFT:  door = RoomMap.Door.LEFT;  opposite = DoorBehavior.DOOR_RIGHT; break;
            case DoorBehavior.DOOR_RIGHT: door = RoomMap.Door.RIGHT; opposite = DoorBehavior.DOOR_LEFT;  break;
            default: return;
        }

        if (!mCurRoom.HasDoor(door))
            return;

        int newId = mCurRoom.GetRoomIdAt(door);

        LoadWorld(newId);

        MovePlayerTo(mCurWorld.GetPlayerStartPos(opposite));
    }

    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        p.MovePosition(coords);
    }

    public Vector3[] GetCameraBounds()
    {
        if (mCurWorld == null)
            return null;
        return mCurWorld.GetCameraBounds();
    }
}
