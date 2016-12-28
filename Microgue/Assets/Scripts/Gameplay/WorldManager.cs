using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomMapGenerator;

using Bundle = System.Collections.Generic.Dictionary<string, string>;
using Random = UnityEngine.Random;
using System.IO;

public class WorldManager
{
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/Resources";
    public static readonly string STARTING_MAP = "new_start";

    private string mWorldName;
    private MapGenerator mMapGenerator;
    private MapAssetManager mMapAssetManager;
    private GameObject mPlayer;

    // "Single map" vars
    private Dictionary<int, Level> mLevels = null;
    private RoomInfo mCurRoom = null;
    private Level mCurWorld = null;
    private int mCurWorldId = -1;

    // list of available .prefab files
    private List<string> mAvailableLevels;

    public int mMinRooms = 5;
    public int mMaxRooms = 7;

    public WorldManager(string worldName)
    {
        mWorldName = worldName;
        mPlayer = GameObject.Find("MainCharacter");
    }

    public void Load()
    {
        // TRIGGER EVENT MAP_LOADING_STARTED
        Debug.Log("Loading Started");
        FetchLevels(mWorldName);
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

    // TODO sposta
    private void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        mPlayer.transform.position = coords;
        p.MovePosition(coords);
    }

    public int CountEnemies()
    {
        if (mCurWorld != null)
            return mCurWorld.CountEnemies();

        return -1;
    }

    public bool AreAllEnemiesKilled()
    {
        if (mCurWorld != null)
            return mCurWorld.CountEnemies() == 0;

        return false;
    }

    public GameObject RandomEnemy()
    {
        if (mCurWorld != null)
            return mCurWorld.RandomEnemy();

        return null;
    }

    public void OnDoorEnter(RoomMap.Door door, RoomMap.Door opposite)
    {
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

    public int GetCurrentRoomId() {
        return mCurWorldId;
    }

    public int GetEndRoomId()
    {
        return mMapGenerator.GetEndRoomId();
    }

    public GameObject GetWorld() {
        return mCurWorld.GetWorld();
    }

    // TODO spostare?
    public Vector3[] GetCameraBounds()
    {
        if (mCurWorld == null)
            return null;
        return mCurWorld.GetCameraBounds();
    }

    public RoomMap GetMap() {
        return mMapGenerator.GetMap();
    }

}
