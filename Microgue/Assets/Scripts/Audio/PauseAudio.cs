using UnityEngine;
using System.Collections;

public class PauseAudio : MonoBehaviour
{ 
    public void MouseOver()
    {
        GetComponents<AudioSource>()[0].Play();
    }
}
