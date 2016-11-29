using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DebugManager : MonoBehaviour {

    public Text debugText;

    bool isDebugVisible = true;

    GameObject mainCharacter;
    StatManager playerStats;

	// Use this for initialization
	void Start () {

        if (!debugText)
            return;

        mainCharacter = GameObject.FindGameObjectWithTag("Player");
	}

    private void UpdateDebugText()
    {
        debugText.text = "DEBUG (Z to toggle)\n";
        ShowPlayerStats();
    }

    private void ShowPlayerStats()
    {
        playerStats = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
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

        UpdateDebugText();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            debugText.enabled = (isDebugVisible ? false : true);
            isDebugVisible = !isDebugVisible;
        }
    }
}
