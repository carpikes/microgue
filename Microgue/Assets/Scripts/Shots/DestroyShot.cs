﻿using UnityEngine;
using System.Collections;

public class DestroyShot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D( Collider2D other )
    {
        if( other.CompareTag("Shot") )
        {
            Destroy(other.gameObject);
        }
    }
}
