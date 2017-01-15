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

    [Header("Room map")]
    public GameObject mRoomMap;
    public GameObject mBlankRoomMap;

    [Header("Worlds")]
    public SingleWorld[] mWorlds;
    public SingleWorld mDebugWorld;
    public int startFromWorld = 0;

    private int mCurWorld = -1;

    [Header("DEBUG")]
    public bool debugArena = false;
    public bool isInvincible = false;
    public bool pressBToGoToBoss = false;
    public bool bypassDoors = false;

    private bool mGameRunning = true;
    private bool mGameOver = false; //se true, pause non va piu`

    private WorldManager mWorldManager = null;

    private RawImage mRawImage;
    private GameObject mMainChr, mShotPos, mAIMap;

    public GameObject settingsMgrGo;
    private SettingsManager settingsMgr;

    FMOD.Studio.EventInstance mMusicInstance = null;
    FMOD.Studio.EventInstance mAmbienceInstance = null;
    FMOD.Studio.EventInstance mSnapshotInstance = null;

    public GameObject resumeButton;

    void Awake()
    {
        try {
            settingsMgr = GameObject.FindGameObjectWithTag("SettingsManager").GetComponent<SettingsManager>();

            pressBToGoToBoss = settingsMgr.skipToBoss;
            isInvincible = settingsMgr.invincible;
        } catch( Exception e )
        {
            Debug.Log("You must start the game from the intro!");
        }

        Debug.Log("boss: " + pressBToGoToBoss);
    }

    // Use this for initialization
    void Start()
    {
        mCurWorld = startFromWorld - 1;
        Cursor.visible = false;

        mMainChr = GameObject.Find("/MainCharacter");
        mShotPos = GameObject.Find("/ShotPosition");
        //mAIMap = GameObject.Find("AIMap");
        mGameRunning = true;
        mGameOver = false;

        

        if (debugArena)
        {
            mWorlds = new SingleWorld[1];

            mWorlds[0] = mDebugWorld;
            mCurWorld = -1;
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
        ToggleRoomMap();
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

        // HACK FINAL BOSS
        if (mCurWorld >= mWorlds.Length)
        {
            WinScreen();

            return;
        }

        Bundle b = new Bundle();
        b.Add("Name", mWorlds[mCurWorld].mWorldName);
        EventManager.TriggerEvent(Events.ON_LEVEL_BEFORE_LOADING, b);

        if (mWorldManager != null)
            mWorldManager.Unload();

        GameObject parent = GameObject.Find("/WorldData");
        foreach (Transform t in parent.transform)
            GameObject.Destroy(t.gameObject);

        mWorldManager = new WorldManager(mWorlds[mCurWorld], pressBToGoToBoss);

        // handle audio transition
        AudioTransition();

        mWorldManager.Load();
        GetComponent<TimerManager>().MAX_TIME = mWorlds[mCurWorld].mTimeInSeconds;
        GetComponent<TimerManager>().Start();
        PauseGame();

        ToggleRoomMap();

        EventManager.TriggerEvent(Events.ON_LEVEL_AFTER_LOADING, null);
    }

    private void ToggleRoomMap()
    {
        mRoomMap.SetActive(mWorlds[mCurWorld].mMapEnabled);
    }

    private void WinScreen()
    {
        Debug.LogError("WIN SCREEN MISSING!");
    }

    private void AudioTransition()
    {
        /*if (mAmbienceManager == null || mSnapshotManager == null || mMusicManager == null)
        {
            Debug.Log("Something is wrong with the emitters. Skipping audio.");
            return;
        }*/

        if (mSnapshotInstance != null) mSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mSnapshotInstance = FMODUnity.RuntimeManager.CreateInstance(mWorlds[mCurWorld].mMusicSnapshotPath);
        mSnapshotInstance.start();

        if (mMusicInstance != null) mMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mMusicInstance = FMODUnity.RuntimeManager.CreateInstance(mWorlds[mCurWorld].mBackgroundMusicPath);
        mMusicInstance.start();

        if (mAmbienceInstance != null) mAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mAmbienceInstance = FMODUnity.RuntimeManager.CreateInstance(mWorlds[mCurWorld].mAmbiencePath);
        mAmbienceInstance.start();


        /*//ambienceEmitter.Stop();
        ambienceEmitter.Event = mWorlds[mCurWorld].mAmbiencePath;
        ambienceEmitter.Play();

        //snapshotEmitter.Stop();
        snapshotEmitter.Event = mWorlds[mCurWorld].mMusicSnapshotPath;
        snapshotEmitter.Play();

        //musicEmitter.Stop();
        musicEmitter.Event = mWorlds[mCurWorld].mBackgroundMusicPath;
        musicEmitter.Play();*/
    }

    // called once after on_level_after_loading
    void OnLoadingScreenComplete(Bundle useless)
    {
        StartGame();
    }

    public void StopGame()
    {
        mMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        mGameRunning = false;
        mGameOver = true;
        Cursor.visible = true;
        mWorldManager.GetWorld().SetActive(false);
        mMainChr.SetActive(false);
        mShotPos.SetActive(false);
    }

    // goto pause, called also while loading
    void PauseGame()
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
        if (Input.GetButtonDown("Escape"))
        {
            if (mGameOver)
                return; 

            GameObject obj = GameObject.Find("Canvas/UICanvas/PauseMenu");
            if (mGameRunning)
            {
                GetComponents<FMODUnity.StudioEventEmitter>()[0].Play();
                PauseGame();
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

        mMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        /*ambienceEmitter.Stop();
        snapshotEmitter.Stop();
        musicEmitter.Stop();*/
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
