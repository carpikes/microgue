using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {

    public bool invincible;
    public bool skipToBoss;
    public float musicVolume, ambienceVolume;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);

        // get preferences
        FetchPlayerPrefs();
    }

    public void FetchPlayerPrefs()
    {
        invincible = bool.Parse(PlayerPrefs.GetString("invincible", "true"));
        skipToBoss = bool.Parse(PlayerPrefs.GetString("skipBoss", "false"));
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        ambienceVolume = PlayerPrefs.GetFloat("ambienceVolume", 1f);
    }

    public void setInvincible( bool v ) { invincible = v; PlayerPrefs.SetString("invincible", v.ToString()); }
    public void setSkipToBoss( bool v ) { skipToBoss = v; PlayerPrefs.SetString("skipBoss", v.ToString());  }

    public void SetMusicVolume(float v)
    {
        AudioManager.SetMusicVolume(v);
        PlayerPrefs.SetFloat("musicVolume", v);
    }

    public void SetAmbienceVolume(float v)
    {
        AudioManager.SetAmbienceVolume(v);
        PlayerPrefs.SetFloat("ambienceVolume", v);
    }

    public void SaveToDisk()
    {
        PlayerPrefs.Save();
    }
}
