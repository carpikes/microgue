using UnityEngine;
using System.Collections;
using FMODUnity;

public class AudioMenu : MonoBehaviour {

    StudioEventEmitter[] emitters;

    void Start()
    {
        emitters = GetComponents<StudioEventEmitter>();
        Debug.Assert(emitters != null, "Audio for menu not working.");
    }

	public void MouseOver()
    {
        emitters[0].Play();
    }

    public void ConfirmChoice()
    {
        emitters[1].Play();
    }
}
