using UnityEngine;
using System.Collections.Generic;
using RoomMapGenerator;

public class WorldManager
{
    public const string PREFAB_PATH = "Assets/Tiled2Unity/Prefabs/Resources";

    private SingleWorld mWorldConfig;

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

    // skip directly to boss
    private bool skipToBoss;

    public WorldManager(SingleWorld worldConfig, bool canSkipToBoss)
    {
        mWorldConfig = worldConfig;
        mPlayer = GameObject.Find("MainCharacter");
        skipToBoss = canSkipToBoss;
    }

    public void Load()
    {
        // TRIGGER EVENT MAP_LOADING_STARTED
        //Debug.Log("Loading Started");
        FetchLevels(mWorldConfig.mWorldAssetPrefix);
        mMapGenerator = new MapGenerator();
        mMapAssetManager = new MapAssetManager(mMapGenerator, mAvailableLevels);
        mLevels = new Dictionary<int, Level>();

        mMapGenerator.GenerateMap(mWorldConfig.mMinRooms, mWorldConfig.mMaxRooms); 
        mMapAssetManager.SetStartMap( mWorldConfig.mStartRoomName );

        LoadLevel(mMapGenerator.GetStartRoomId());
        AudioManager.PlayMusic(mWorldConfig.mBackgroundMusic, mWorldConfig.mMusicFadeIn);
        AudioManager.PlayAmbience(mWorldConfig.mAmbienceMusic);
        // TRIGGER EVENT MAP_LOADING_COMPLETED
    }

    public void Unload()
    {
        if (mCurWorld != null)
        {
            mCurWorld.Unload();
            mCurWorld = null;
        }
        GameObject parent = GameObject.Find("/WorldData");
        foreach (Transform t in parent.transform)
            GameObject.Destroy(t.gameObject);
    }

    /* TODO questa va in LevelLoader */
    public void LoadBossRoom()
    {
        if (!skipToBoss)
        {
            if (mMapAssetManager.GetNumOfLoadedMaps() != mMapGenerator.NumberOfRooms())
            {
                // non sono esplorate tutte le mappe
                EventManager.TriggerEvent(Events.ON_WORLD_UNEXPLORED, null);
                return;
            }

            if (!AreAllEnemiesKilled())
            {
                EventManager.TriggerEvent(Events.ON_STILL_ENEMIES_LEFT, null);
                return;
            }
        }

        Unload();

        Level l = new Level(-1, mWorldConfig.mBossRoomName);
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

        foreach( var go in Resources.LoadAll("TiledLevels") )
        {
            if( go.name.StartsWith(name + "_"))
            {
                mAvailableLevels.Add(go.name);
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

    public MapGenerator GetMapGenerator()
    {
        return mMapGenerator;
    }

    public int GetMapDoors(string mapname)
    {
        return mMapAssetManager.GetMapDoors(mapname);
    }
}
