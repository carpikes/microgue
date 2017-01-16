using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIMenu : MonoBehaviour {

    public SettingsManager settingsManager;

    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField]
    private GameObject creditsPanel;

    [SerializeField]
    Button[] mainButtons;

    public Toggle invToggle;
    public Toggle skipToggle;
    public Slider musicSlider;
    public Slider ambienceSlider;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Assert(scene.name == "Menu", "WHY IS THIS SCRIPT NOT IN THE MENU??");

        SetupMenuElements();
    }

    private void SetupMenuElements()
    {
        settingsManager.FetchPlayerPrefs();

        invToggle.isOn = settingsManager.invincible;
        skipToggle.isOn = settingsManager.skipToBoss;
        musicSlider.value = settingsManager.musicVolume;
        ambienceSlider.value = settingsManager.ambienceVolume;
    }

    public void Awake()
    {
        ShowOptions(false);
    }

    public void ShowOptions(bool v)
    {
        DisableMainMenuButtons(v);
        optionsPanel.SetActive(v);
    }

    public void ShowCredits(bool v)
    {
        Debug.Log("Credits");
        DisableMainMenuButtons(v);
        creditsPanel.SetActive(v);
    }

    private void DisableMainMenuButtons(bool v)
    {
        foreach( Button b in mainButtons )
            b.enabled = !v;
    }

    public void BeginThePath()
    {
        // save prefs to disk and load level
        settingsManager.SaveToDisk();
        SceneManager.LoadScene("Gameplay");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
