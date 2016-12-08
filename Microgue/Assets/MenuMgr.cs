using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuMgr : MonoBehaviour {

	public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
