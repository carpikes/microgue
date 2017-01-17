using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UIMenu : MonoBehaviour {

    public SettingsManager settingsManager;

    [SerializeField]
    private GameObject creditsPanel;

    [SerializeField]
    Button[] mainButtons;

    public Button resumeFromCredits;

    public EventSystem eventSystem;

    public void ShowCredits(bool v)
    {
        DisableMainMenuButtons(v);

        creditsPanel.SetActive(v);

        StartCoroutine(ChangeHighLightedBtn(v));
    }

    private IEnumerator ChangeHighLightedBtn(bool v)
    {
        eventSystem.SetSelectedGameObject(null);

        yield return new WaitForEndOfFrame();

        eventSystem.SetSelectedGameObject(v ? resumeFromCredits.gameObject : mainButtons[0].gameObject);
    }

    private void DisableMainMenuButtons(bool v)
    {
        foreach( Button b in mainButtons )
            b.enabled = !v;
    }

    public void BeginThePath()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
