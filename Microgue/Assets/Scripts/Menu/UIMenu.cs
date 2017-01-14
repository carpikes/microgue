using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class UIMenu : MonoBehaviour {

    public SettingsManager settingsManager;

    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField]
    Button[] mainButtons;

    public Toggle invToggle;
    public Toggle skipToggle;

    public void Awake()
    {
        ShowOptions(false);
        SetSettings();
    }

    private void SetSettings()
    {
        settingsManager.setInvincible(true);
        settingsManager.setSkipToBoss(false);

        invToggle.isOn = true;
        skipToggle.isOn = false;
    }

    public void ShowOptions(bool v)
    {
        DisableMainMenuButtons(v);
        optionsPanel.SetActive(v);
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
