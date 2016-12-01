using UnityEngine;
using System.Collections;

public class StompStompShadow : MonoBehaviour {
    private StompStomp mMaster;

	// Use this for initialization
	void Start () {
        mMaster = transform.parent.GetComponentInChildren<StompStomp>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag("Player"))
            mMaster.OnShadowTouch();   
    }
}
