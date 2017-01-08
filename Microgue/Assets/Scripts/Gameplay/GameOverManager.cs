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
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_DEATH, GameOver);
        EventManager.StopListening(Events.ON_TIME_ENDED, GameOver);
    }

    // Use this for initialization
    void Start () {
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mAimCursor = GameObject.FindGameObjectWithTag("AimCursor");
    }

    void Update() {
        if (Input.GetButtonDown("Escape"))
            ReloadMenu();
    }
	
    private void GameOver(Bundle args)
    {
        GameObject.Find("GameplayManager").GetComponent<GameplayManager>().StopGame();
        mPlayer.SetActive(false);
        mAimCursor.SetActive(false);

        UICanvas.SetActive(false);

        Cursor.visible = true;
        gameOverCanvas.SetActive(true);
    }

    public void ReloadMenu()
    {
        SceneManager.UnloadScene(SceneManager.GetActiveScene());
        SceneManager.LoadScene("Menu");
    }
}
