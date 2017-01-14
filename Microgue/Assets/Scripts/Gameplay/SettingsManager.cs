using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {

    public bool invincible;
    public bool skipToBoss;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
    public void setInvincible( bool v ) { invincible = v; }
    public void setSkipToBoss( bool v ) { skipToBoss = v; }


}
