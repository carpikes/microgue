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
    }

    public void setInvincible( bool v ) { invincible = v; }
    public void setSkipToBoss( bool v ) { skipToBoss = v; }

    public void SetMusicVolume(float v)
    {
        AudioManager.SetMusicVolume(v);
    }

    public void SetAmbienceVolume(float v)
    {
        AudioManager.SetAmbienceVolume(v);
    }
}
