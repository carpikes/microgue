using UnityEngine;
using System.Collections;

public class AudioMenu : MonoBehaviour {

    AudioSource[] sources;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        sources = GetComponents<AudioSource>();
        Debug.Assert(sources != null, "Audio for menu not working.");
        sources[1].Play();
        sources[1].Pause();
    }

    public void MouseOver()
    {
        sources[0].Play();
    }

    public void ConfirmChoice()
    {
        sources[1].UnPause();
        GetComponent<UIMenu>().BeginThePath();
    }
}
