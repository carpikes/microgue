using UnityEngine;
using System.Collections;
using POLIMIGameCollective;

public class AudioManager : Singleton<AudioManager> {

    protected AudioManager() { }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	public static void Play()
    {
        
    }
}
