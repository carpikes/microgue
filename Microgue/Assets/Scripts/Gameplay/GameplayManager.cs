using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RoomMapGenerator;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;

public class GameplayManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool debugArena = false;

    public bool isInvincible = false;

    private WorldManager mWorldManager;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;

        string lname = "new";
        if (debugArena)
            lname = "debug";

        mWorldManager = new WorldManager(lname);
        mWorldManager.Load();
    }

    public WorldManager GetWorldManager()
    {
        return mWorldManager;
    }
}
