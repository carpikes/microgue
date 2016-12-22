using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using RoomMapGenerator;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool debugArena = false;

    public bool isInvincible = false;

    private WorldManager mWorldManager;

    private RawImage mRawImage;
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

    void Update() {
    }

    public WorldManager GetWorldManager()
    {
        return mWorldManager;
    }
}
