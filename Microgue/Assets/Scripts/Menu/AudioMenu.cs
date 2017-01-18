using UnityEngine;
using System.Collections;

public class AudioMenu : MonoBehaviour {

    AudioSource[] sources;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        sources = GetComponents<AudioSource>();
        Debug.Assert(sources != null, "Audio for menu not working.");
    }

    public void MouseOver()
    {
        sources[0].Play();
    }

    public void ConfirmChoice()
    {
        Debug.Log(sources[1].clip.name);
        sources[1].Play();
    }
}
