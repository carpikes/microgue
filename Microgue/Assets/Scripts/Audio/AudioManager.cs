using UnityEngine;
using System.Collections;
using POLIMIGameCollective;

public class AudioManager : Singleton<AudioManager> {
    AudioSource mAudioSrc;

    public AudioClip mSoundtrack;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        mAudioSrc = GetComponent<AudioSource>();
	}
	
	public void Play()
    {
        mAudioSrc.Play();
    }
}
