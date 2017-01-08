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

    GameObject mAmbienceManager;
    GameObject mSnapshotManager;
    GameObject mMusicManager;

    FMODUnity.StudioEventEmitter ambienceEmitter = null;
    FMODUnity.StudioEventEmitter snapshotEmitter = null;
    FMODUnity.StudioEventEmitter musicEmitter = null;

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

        mAmbienceManager = GameObject.FindGameObjectWithTag("AmbienceManager");
        mSnapshotManager = GameObject.FindGameObjectWithTag("SnapshotManager");
        mMusicManager = GameObject.FindGameObjectWithTag("BGMusicManager");

        if (mAmbienceManager != null && mSnapshotManager != null && mMusicManager != null)
        {
            ambienceEmitter = mAmbienceManager.GetComponent<FMODUnity.StudioEventEmitter>();
            snapshotEmitter = mSnapshotManager.GetComponent<FMODUnity.StudioEventEmitter>();
            musicEmitter = mMusicManager.GetComponent<FMODUnity.StudioEventEmitter>();
        }

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
        EventManager.TriggerEvent(Events.ON_LEVEL_AFTER_LOADING, null);
    }

    private void AudioTransition()
    {
        if (mAmbienceManager == null || mSnapshotManager == null || mMusicManager == null)
        {
            Debug.Log("Something is wrong with the emitters. Skipping audio.");
            return;
        }

        ambienceEmitter.Stop();
        ambienceEmitter.Event = mWorlds[mCurWorld].mAmbiencePath;
        ambienceEmitter.Play();

        snapshotEmitter.Stop();
        snapshotEmitter.Event = mWorlds[mCurWorld].mMusicSnapshotPath;
        snapshotEmitter.Play();

        musicEmitter.Stop();
        musicEmitter.Event = mWorlds[mCurWorld].mBackgroundMusicPath;
        musicEmitter.Play();
    }

    // called once after on_level_after_loading
    void OnLoadingScreenComplete(Bundle useless)
    {
        StartGame();
    }

    public void StopGame()
    {
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
        ambienceEmitter.Stop();
        snapshotEmitter.Stop();
        musicEmitter.Stop();
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
