using UnityEngine;
using System.Collections;
using POLIMIGameCollective;

public class AudioManager : Singleton<AudioManager> {
    AudioSource mBackgroundSrc, mAmbientSrc;
    public static AudioManager instance;
    private bool mFadeIn = false;
    private float mStartVolume = 0.0f;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mBackgroundSrc = GetComponents<AudioSource>()[0];
            mAmbientSrc = GetComponents<AudioSource>()[1];
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
	}

    void Start()
    {
    }
	
	public static void PlayAmbience(AudioClip music)
    {
        instance.mAmbientSrc.Stop();
        if (music != null)
        {
            instance.mAmbientSrc.loop = true;
            instance.mAmbientSrc.clip = music;
            instance.mAmbientSrc.volume = 1.0f;
            instance.mAmbientSrc.Play();
        }
    }

    void Update() {
        if (mFadeIn)
        {
            float vol = mStartVolume;
            vol += Time.deltaTime;
            if (vol > 1.0f)
            {
                vol = 1.0f;
                mFadeIn = false;
            }
            mStartVolume = vol;
            mBackgroundSrc.volume = (vol < 0.0f) ? 0.0f : vol;
        }
    }

    public static void PlayMusic(AudioClip music, bool fadeIn)
    {
        instance.mBackgroundSrc.Stop();
        if (music != null)
        {
            instance.mFadeIn = fadeIn;
            instance.mBackgroundSrc.loop = true;
            instance.mBackgroundSrc.clip = music;
            if (fadeIn)
            {
                instance.mBackgroundSrc.volume = 0;
                instance.mStartVolume = -1.0f;
            }
            instance.mBackgroundSrc.Play();
        } 
    }

    public static void SetAmbienceVolume(float v)
    {
        instance.mAmbientSrc.volume = v;
    }

    public static void SetMusicVolume(float v)
    {
        instance.mBackgroundSrc.volume = v;
    }
}
