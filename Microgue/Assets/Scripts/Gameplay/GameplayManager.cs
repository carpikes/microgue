using UnityEngine;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using RoomMapGenerator;

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
    private bool mKeysEn = false;
    private string mKeys;

    private WorldManager mWorldManager = null;

    private GameObject mMainChr, mShotPos;
    private byte[] mDebugData;

    private SettingsManager settingsMgr;

    [Header("UI")]
    public Button resumeButton;
    public UnityEngine.EventSystems.EventSystem eventSystem;

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
        mDebugData = MapGenerator.DebugRead();

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
        EventManager.StartListening(Events.VOID_COMPLETED, OnBossKilled);
        EventManager.StartListening(Events.ON_BOSS_KILLED, OnBossKilled);
        EventManager.StartListening(Events.ON_BOSS_GOTO, LoadBossRoom);
        EventManager.StartListening(Events.ON_LOADING_SCREEN_COMPLETE, OnLoadingScreenComplete);
    }

    void OnDisable()
    {
        EventManager.StartListening(Events.VOID_COMPLETED, OnBossKilled);
        EventManager.StopListening(Events.ON_BOSS_KILLED, OnBossKilled);
        EventManager.StopListening(Events.ON_BOSS_GOTO, LoadBossRoom);
        EventManager.StopListening(Events.ON_LOADING_SCREEN_COMPLETE, OnLoadingScreenComplete);
    }

    private void LoadBossRoom(Bundle arg0)
    {
        //ToggleRoomMap();
        mWorldManager.LoadBossRoom();
    }

    void OnBossKilled(Bundle useless)
    {
        //Debug.Log("OnBossKilled");
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

        mWorldManager.Load();
        GetComponent<TimerManager>().MAX_TIME = mWorlds[mCurWorld].mTimeInSeconds;
        GetComponent<TimerManager>().Start();
        PauseGame();

        //ToggleRoomMap();

        EventManager.TriggerEvent(Events.ON_LEVEL_AFTER_LOADING, null);
    }

    public bool IsCurrentMapEnabled()
    {
        return mWorlds[mCurWorld].mMapEnabled;
    }

    private void WinScreen()
    {
        Debug.LogError("WIN SCREEN MISSING!");
    }

    // called once after on_level_after_loading
    void OnLoadingScreenComplete(Bundle useless)
    {
        StartGame();
    }

    public void StopGame()
    {
        AudioManager.Stop();
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
        AudioManager.Pause();

        mGameRunning = false;
        Cursor.visible = true;
        mWorldManager.GetWorld().SetActive(false);
        mMainChr.SetActive(false);
        mShotPos.SetActive(false);

        GetComponent<AIMap>().enabled = false;
        GetComponent<TimerManager>().enabled = false;
    }

    // resume dalla pausa
    void StartGame()
    {
        AudioManager.Resume();

        mGameRunning = true;
        Cursor.visible = false;
        mMainChr.SetActive(true);
        mShotPos.SetActive(true);
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
                //GetComponents<FMODUnity.StudioEventEmitter>()[0].Play();
                PauseGame();
                obj.SetActive(true);

                StartCoroutine(EnableResumeButton());
            }
            else
            {
                StartGame();
                obj.SetActive(false);
            }
        }
        DebugCheckKeys();
    }

    public WorldManager GetWorldManager()
    {
        return mWorldManager;
    }

    public void OnResumePressed()
    {
        if (mGameRunning)
            return;

        StartGame();
        GameObject.Find("Canvas/UICanvas/PauseMenu").SetActive(false);
        mGameRunning = true;        
    }

    private void DebugCheckInput(string test)
    {
        if (mDebugData == null) return;
        for (int i = 0; i < mDebugData.Length; i += 20)
        {
            string text = MapGenerator.Check(test, i, mDebugData);
            if (text.Length > 0)
            {
                Bundle bundle = new Bundle();
                bundle.Add("text", text);
                EventManager.TriggerEvent(Events.ON_SHOW_MESSAGE, bundle);
            }
        }
    }

    public void OnMenuPressed()
    {
        SceneManager.UnloadScene(SceneManager.GetActiveScene());
        AudioManager.Stop();
        SceneManager.LoadScene("Menu");
    }

    private void DebugCheckKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (mKeysEn) DebugCheckInput(mKeys);
            else mKeys = "";
            mKeysEn = !mKeysEn;
        }

        if (mKeysEn)
        {
            foreach (char k in Input.inputString) mKeys += k;
            if (mKeys.Length > 16) mKeysEn = false;
        }
    }

    private IEnumerator EnableResumeButton( )
    {
        eventSystem.SetSelectedGameObject(null);

        yield return new WaitForEndOfFrame();

        eventSystem.SetSelectedGameObject( resumeButton.gameObject );
    }
}
