﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DebugManager : MonoBehaviour {

    public Text debugText;

    bool isDebugVisible = true;

    GameObject mainCharacter;
    PlayerStats playerStats;

	// Use this for initialization
	void Start () {

        if (!debugText)
            return;

        mainCharacter = GameObject.FindGameObjectWithTag("Player");
	}

    private void UpdateDebugText()
    {
        debugText.text = "!!! DA REFACTORARE (Vedi keep)!!!\n\n\nDEBUG (X to update, Z to toggle)\n";
        ShowPlayerStats();
    }

    private void ShowPlayerStats()
    {
        playerStats = mainCharacter.GetComponent<PlayerStats>();
        if ( playerStats )
        {
            foreach( Stat s in playerStats.stats )
            {
                debugText.text += s.mName + ": " + s.CurrentValue + "\n";
            }
        } else
        {
            Debug.LogError("Cannot retrieve player stats component");
        }
    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKey(KeyCode.X))
        UpdateDebugText();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            debugText.enabled = (isDebugVisible ? false : true);
            isDebugVisible = !isDebugVisible;
        }
    }
}
