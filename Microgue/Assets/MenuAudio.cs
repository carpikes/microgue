using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class MenuAudio : MonoBehaviour {

    FMODUnity.StudioEventEmitter[] emitters;

    // Use this for initialization
    void Start () {
        emitters = GetComponents<FMODUnity.StudioEventEmitter>();
    }

    public void OnPointerEnter()
    {
        emitters[0].Play();
    }

    public void OnPointerClick()
    {
        emitters[1].Play();
    }
}
