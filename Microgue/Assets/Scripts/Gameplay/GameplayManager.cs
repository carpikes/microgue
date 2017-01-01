using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using RoomMapGenerator;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject mPlayer;
    public GameObject mAimCursor;

    [Header("Load debug arena?")]
    public bool debugArena = false;

    public bool isInvincible = false;

    private bool mGameRunning = true;

    private WorldManager mWorldManager;

    private RawImage mRawImage;
    private GameObject mMainChr, mShotPos, mAIMap;
    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;

        string lname = "new";
        if (debugArena)
            lname = "debug";

        mWorldManager = new WorldManager(lname);
        mWorldManager.Load();

        mMainChr = GameObject.Find("/MainCharacter");
        mShotPos = GameObject.Find("/ShotPosition");
        mAIMap = GameObject.Find("AIMap");
        mGameRunning = true;
    }

    void StopGame()
    {
        mGameRunning = false;
        Cursor.visible = true;
        mWorldManager.GetWorld().SetActive(false);
        mMainChr.SetActive(false);
        mShotPos.SetActive(false);
        mAIMap.SetActive(false);

        GetComponent<AIMap>().enabled = false;
        GetComponent<TimerManager>().enabled = false;
    }

    void StartGame()
    {
        mGameRunning = true;
        Cursor.visible = false;
        mMainChr.SetActive(true);
        mShotPos.SetActive(true);
        mAIMap.SetActive(true);
        mWorldManager.GetWorld().SetActive(true);

        GetComponent<AIMap>().enabled = true;
        GetComponent<TimerManager>().enabled = true;
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameObject obj = GameObject.Find("Canvas/UICanvas/PauseMenu");
            if (mGameRunning)
            {
                StopGame();
                obj.SetActive(true);
            }
            else
            {
                StartGame();
                obj.SetActive(false);
            }
        }
    }

    public WorldManager GetWorldManager()
    {
        return mWorldManager;
    }

    public void OnMenuPressed()
    {
        SceneManager.UnloadScene(SceneManager.GetActiveScene());
        SceneManager.LoadScene("Menu");
    }

    public void OnResumePressed()
    {
        if (mGameRunning)
            return;

        StartGame();
        GameObject.Find("Canvas/UICanvas/PauseMenu").SetActive(false);
        mGameRunning = true;        
    }
}
