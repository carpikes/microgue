using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour {

    [SerializeField]
    private Button mBeginButton;

    public void BeginThePath()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Options()
    {
        Debug.Log("Options not implemented yet.");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
