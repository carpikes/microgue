using UnityEngine;
using System.Collections;
using POLIMIGameCollective;
using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class AudioManager : Singleton<AudioManager> {
    AudioSource mBackgroundSrc, mAmbientSrc, mSimpleSrc;
    public static AudioManager instance;
    private bool mFadeIn = false;
    private float mStartVolume = 0.0f;
    private float mTargetVolume = 1.0f;

    void OnEnable() {
    }

    void OnDisable() {
    }

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

    void OnItemPickup(Bundle b) {

    }
	
	public static void PlayAmbience(AudioClip music)
    {
        if (instance == null) return;
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
            if (vol > mTargetVolume)
            {
                vol = mTargetVolume;
                mFadeIn = false;
            }
            mStartVolume = vol;
            mBackgroundSrc.volume = (vol < 0.0f) ? 0.0f : vol;
        }
    }

    public static void PlayMusic(AudioClip music, bool fadeIn)
    {
        if (instance == null) return;
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
        if (instance == null) return;
        instance.mAmbientSrc.volume = v;
    }

    public static void SetMusicVolume(float v)
    {
        if (instance == null) return;
        instance.mTargetVolume = v;
        if ((instance.mBackgroundSrc.volume - instance.mTargetVolume) < Mathf.Epsilon)
            instance.mBackgroundSrc.volume = instance.mTargetVolume;
    }

    public static void Pause() {
        if (instance == null) return;
        instance.mBackgroundSrc.Pause();
        instance.mAmbientSrc.Pause();
    }

    public static void Resume() {
        if (instance == null) return;
        instance.mBackgroundSrc.UnPause();
        instance.mAmbientSrc.UnPause();
    }

    public static void Stop() {
        if (instance == null) return;
        instance.mBackgroundSrc.Stop();
        instance.mAmbientSrc.Stop();
    }
}
