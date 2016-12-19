﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RoomMapGenerator;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;

public class GameplayManager : MonoBehaviour
{
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/";
    public static readonly string STARTING_MAP = "new_start";

    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool debugArena = false;

    public bool isInvincible = false;

    private MapGenerator mMapGenerator;
    private MapAssetManager mMapAssetManager;

    // "Single map" vars
    private Dictionary<int, Level> mLevels = null;
    private RoomInfo mCurRoom = null;
    private Level mCurWorld = null;
    private int mCurWorldId = -1;

    // list of available .prefab files
    private List<string> mAvailableLevels;

    [Header("Min Rooms for this level")]
    public int mMinRooms = 3;

    [Header("Max Rooms for this level")]
    public int mMaxRooms = 5;

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

        string lname = "new";
        if (debugArena)
            lname = "debug";

        // TRIGGER EVENT MAP_LOADING_STARTED
        //Debug.Log("Loading Started");
        FetchLevels(lname);
        mMapGenerator = new MapGenerator();
        mMapAssetManager = new MapAssetManager(mMapGenerator, mAvailableLevels);
        mLevels = new Dictionary<int, Level>();

        mMapGenerator.GenerateMap(mMinRooms, mMaxRooms); // TODO: sistemare con ciclo
        mMapAssetManager.SetStartMap( STARTING_MAP );

        LoadLevel(mMapGenerator.GetStartRoomId());
  
        // TRIGGER EVENT MAP_LOADING_COMPLETED
    }

    /* TODO questa va in LevelLoader */
    private void LoadBossRoom()
    {
        if (mMapAssetManager.GetNumOfLoadedMaps() != mMapGenerator.NumberOfRooms())
        {
            // non sono esplorate tutte le mappe
            EventManager.TriggerEvent(Events.ON_WORLD_UNEXPLORED, null);
            return;
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            EventManager.TriggerEvent(Events.ON_STILL_ENEMIES_LEFT, null);
            return;
        }

        if(mCurWorld != null)
            mCurWorld.Unload();

        Level l = new Level(-1, "new_boss");
        mCurWorld = l;
        mCurWorldId = -1;
        mPlayer.transform.position = new Vector2(-9000, -9000);
        mCurWorld.Load();
        MovePlayerTo(mCurWorld.GetPlayerStartPos("Spawn"));
    }

    /* TODO LevelLoader */
    private void LoadLevel(int n, int spawnPoint = 0)
    {
        if (n == mCurWorldId)
            return;

        if(mCurWorld != null)
            mCurWorld.Unload();

        mCurRoom = mMapGenerator.GetRoom(n);

        if (!mLevels.ContainsKey(n))
        {
            Level l = new Level(n, mMapAssetManager.GetMap(n, mCurRoom.GetDoors()));
            mLevels.Add(n, l);
            mCurWorld = l;
        }

        mMapGenerator.GetMap().AddDoors(n, (int)RoomMap.Door.VISITED);

        mCurWorld = mLevels[n];
        mCurWorldId = n;

        mPlayer.transform.position = new Vector2(-9000, -9000);
        mCurWorld.Load();

        if (spawnPoint == 0)
            MovePlayerTo(mCurWorld.GetPlayerStartPos("Spawn"));
        else
        {
            string door = DoorNumToString(spawnPoint);
            MovePlayerTo(mCurWorld.GetPlayerStartPos(door));
        }
    }

    /* TODO va in LevelFileFetcher */
    private void FetchLevels(string name)
    {
        mAvailableLevels = new List<string>();

        foreach (string file in Directory.GetFiles(PREFAB_PATH))
        {
            string fname = Path.GetFileNameWithoutExtension(file);
            if (file.EndsWith(".prefab") && fname.StartsWith(name + "_"))
            {
                mAvailableLevels.Add(fname);
            }
        }
    }

    /* TODO gestore porte */
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

    /* TODO gestore porte */
    public void OnDoorEnter(Bundle args)
    {
        if (debugArena)
            return;

        if (!AreAllEnemiesKilled() && !isInvincible)
        {
            EventManager.TriggerEvent(Events.ON_STILL_ENEMIES_LEFT, null);
            return;
        }

        string type;
        RoomMap.Door door, opposite;

        if (!args.TryGetValue(DoorBehavior.DOOR_TYPE_TAG, out type))
            return;

        switch (type) {
            case DoorBehavior.DOOR_DOWN:  door = RoomMap.Door.DOWN;  opposite = RoomMap.Door.UP;    break;
            case DoorBehavior.DOOR_UP:    door = RoomMap.Door.UP;    opposite = RoomMap.Door.DOWN;  break;
            case DoorBehavior.DOOR_LEFT:  door = RoomMap.Door.LEFT;  opposite = RoomMap.Door.RIGHT; break;
            case DoorBehavior.DOOR_RIGHT: door = RoomMap.Door.RIGHT; opposite = RoomMap.Door.LEFT;  break;
            default: return;
        }

        if (mCurRoom.GetId() == mMapGenerator.GetEndRoomId())
        {
            if (mCurRoom.GetStartOrEndDoor() == (int)door)
            {
                Debug.Log("Boss door");
                LoadBossRoom();
                return;
            }
        }

        if (!mCurRoom.HasDoor(door))
            return;

        int newId = mCurRoom.GetRoomIdAt(door);

        LoadLevel(newId, (int)opposite);
    }

    // TODO sposta
    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        mPlayer.transform.position = coords;
        p.MovePosition(coords);
    }

    // TODO spostare?
    public Vector3[] GetCameraBounds()
    {
        if (mCurWorld == null)
            return null;
        return mCurWorld.GetCameraBounds();
    }

    public MapGenerator GetMapGen()
    {
        return mMapGenerator;
    }

    public RoomMap GetMap() {
        return mMapGenerator.GetMap();
    }

    public GameObject GetWorld() {
        return mCurWorld.GetWorld();
    }

    public int GetCurrentRoomId() {
        return mCurWorldId;
    }

    public int GetStartRoomId()
    {
        return mMapGenerator.GetStartRoomId();
    }

    public int GetEndRoomId()
    {
        return mMapGenerator.GetEndRoomId();
    }

    private bool AreAllEnemiesKilled()
    {
        if (mCurWorld != null)
            return mCurWorld.CountEnemies() == 0;

        return false;
    }
}
