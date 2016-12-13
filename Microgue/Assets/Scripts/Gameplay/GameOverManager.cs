using UnityEngine;
using System.Collections;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    GameObject mPlayer;
    GameObject mAimCursor;

    public GameObject gameOverCanvas;
    public GameObject UICanvas;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_MAIN_CHAR_DEATH, GameOver);
        EventManager.StartListening(Events.ON_TIME_ENDED, GameOver);
        EventManager.StartListening(Events.ON_WIN, OnWin);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_DEATH, GameOver);
        EventManager.StopListening(Events.ON_TIME_ENDED, GameOver);
        EventManager.StopListening(Events.ON_WIN, OnWin);
    }

    // Use this for initialization
    void Start () {
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mAimCursor = GameObject.FindGameObjectWithTag("AimCursor");
    }
	
    private void GameOver(Bundle args)
    {
        mPlayer.SetActive(false);
        mAimCursor.SetActive(false);

        UICanvas.SetActive(false);

        Cursor.visible = true;
        gameOverCanvas.SetActive(true);
    }

    private void OnWin(Bundle args)
    {
        Debug.Log("Win");
    }

    public void ReloadMenu()
    {
        SceneManager.UnloadScene(SceneManager.GetActiveScene());
        SceneManager.LoadScene("Menu");
    }
}
