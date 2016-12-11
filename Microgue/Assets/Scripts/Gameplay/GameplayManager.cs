using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RoomMapGenerator;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;

public class GameplayManager : MonoBehaviour
{
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/";

    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool debugArena = false;

    public bool isInvincible = false;

    private MapGenerator mMapGenerator;
    private MapAssetManager mMapAssetManager;

    // "Single map" vars
    private Dictionary<int, Level> mWorlds = null;
    private RoomInfo mCurRoom = null;
    private Level mCurWorld = null;
    private int mCurWorldId = -1;

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
        if (debugArena)
            lname = "debug";

        // TRIGGER EVENT MAP_LOADING_STARTED
        //Debug.Log("Loading Started");
        FetchLevels(lname);
        mMapGenerator = new MapGenerator();
        mMapAssetManager = new MapAssetManager(mAvailableLevels);
        mWorlds = new Dictionary<int, Level>();
        mMapGenerator.GenerateMap();

        LoadWorld(mMapGenerator.GetStartRoomId());
        Debug.Log("Loading Completed");
        // TRIGGER EVENT MAP_LOADING_COMPLETED
    }

    private void LoadWorld(int n, int spawnPoint = 0)
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

    private void FetchLevels(string name)
    {
        mAvailableLevels = new List<string>();

        foreach (string file in Directory.GetFiles(PREFAB_PATH))
        {
            string fname = Path.GetFileNameWithoutExtension(file);
            if (file.EndsWith(".prefab")) // && fname.StartsWith(name + "_"))
            {
                //Debug.Log(file);
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
        if (debugArena)
            return;

        if (!AreAllEnemiesKilled())
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

        if (!mCurRoom.HasDoor(door))
            return;

        int newId = mCurRoom.GetRoomIdAt(door);

        LoadWorld(newId, (int)opposite);
    }

    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        mPlayer.transform.position = coords;
        p.MovePosition(coords);
    }

    public Vector3[] GetCameraBounds()
    {
        if (mCurWorld == null)
            return null;
        return mCurWorld.GetCameraBounds();
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
