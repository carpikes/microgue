using UnityEngine;
using System.Collections;

public class FrameRate : MonoBehaviour {

	// Use this for initialization
	void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
