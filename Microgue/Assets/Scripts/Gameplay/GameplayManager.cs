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

    [Header("Worlds")]
    public SingleWorld[] mWorlds;
    public SingleWorld mDebugWorld;
    private int mCurWorld;

    [Header("DEBUG")]
    public bool debugArena = false;
    public bool isInvincible = false;
    public bool pressBToGoToBoss = false;
    public bool bypassDoors = false;

    private bool mGameRunning = true;

    private WorldManager mWorldManager = null;

    private RawImage mRawImage;
    private GameObject mMainChr, mShotPos, mAIMap;

    // Use this for initialization
    void Start()
    {
        mCurWorld = -1;
        Cursor.visible = false;

        mMainChr = GameObject.Find("/MainCharacter");
        mShotPos = GameObject.Find("/ShotPosition");
        //mAIMap = GameObject.Find("AIMap");
        mGameRunning = true;

        if (debugArena)
        {
            mWorlds = new SingleWorld[1];

            mWorlds[0] = mDebugWorld;
        }
        NextWorld();
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_BOSS_KILLED, OnBossKilled);
        EventManager.StartListening(Events.ON_BOSS_GOTO, LoadBossRoom);
        EventManager.StartListening(Events.ON_LOADING_SCREEN_COMPLETE, OnLoadingScreenComplete);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_BOSS_KILLED, OnBossKilled);
        EventManager.StopListening(Events.ON_BOSS_GOTO, LoadBossRoom);
        EventManager.StopListening(Events.ON_LOADING_SCREEN_COMPLETE, OnLoadingScreenComplete);
    }

    private void LoadBossRoom(Bundle arg0)
    {
        mWorldManager.LoadBossRoom();
    }

    void OnBossKilled(Bundle useless)
    {
        Debug.Log("OnBossKilled");
        StartCoroutine(WaitBeforeLoadingLevel());
    }

    IEnumerator WaitBeforeLoadingLevel() {
        yield return new WaitForSeconds(1.0f);
        EventManager.TriggerEvent(Events.FADE_OUT, null);
        yield return new WaitForSeconds(0.2f);
        NextWorld();
        EventManager.TriggerEvent(Events.FADE_IN, null);
    }

    // Load next world (it increments world counter before loading)
    // so, in Start() set the counter to -1
    void NextWorld()
    {
        mCurWorld++;
        if (mCurWorld >= mWorlds.Length)
        {
            // TODO: win screen
            return;
        }
        Bundle b = new Bundle();
        b.Add("Name", mWorlds[mCurWorld].mWorldName);
        EventManager.TriggerEvent(Events.ON_LEVEL_BEFORE_LOADING, b);

        if (mWorldManager != null)
            mWorldManager.Unload();

        mWorldManager = new WorldManager(mWorlds[mCurWorld], pressBToGoToBoss);
        mWorldManager.Load();
        GetComponent<TimerManager>().MAX_TIME = mWorlds[mCurWorld].mTimeInSeconds;
        GetComponent<TimerManager>().Start();
        StopGame();
        EventManager.TriggerEvent(Events.ON_LEVEL_AFTER_LOADING, null);
    }

    // called once after on_level_after_loading
    void OnLoadingScreenComplete(Bundle useless)
    {
        StartGame();
    }

    // goto pause, called also while loading
    void StopGame()
    {
        mGameRunning = false;
        Cursor.visible = true;
        mWorldManager.GetWorld().SetActive(false);
        mMainChr.SetActive(false);
        mShotPos.SetActive(false);
        //mAIMap.SetActive(false);

        GetComponent<AIMap>().enabled = false;
        GetComponent<TimerManager>().enabled = false;
    }

    // resume dalla pausa
    void StartGame()
    {
        mGameRunning = true;
        Cursor.visible = false;
        mMainChr.SetActive(true);
        mShotPos.SetActive(true);
        //mAIMap.SetActive(true);
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
