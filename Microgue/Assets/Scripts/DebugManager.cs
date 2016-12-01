using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class DebugManager : MonoBehaviour {

    public Canvas debugCanvas;
    public Text debugText;

    public bool isDebugVisible = true;

    GameObject mainCharacter;
    StatManager playerStats;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_STAT_CHANGED, OnStatChanged);
    }

    private void OnStatChanged(Bundle args)
    {
        UpdateDebugText();
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_STAT_CHANGED, OnStatChanged);
    }

    // Use this for initialization
    void Start () {
        if (!isDebugVisible)
        {
            debugCanvas.enabled = false;
            enabled = false;
            return;
        }

        mainCharacter = GameObject.FindGameObjectWithTag("Player");
        UpdateDebugText();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateDebugText();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            debugText.enabled = (isDebugVisible ? false : true);
            isDebugVisible = !isDebugVisible;
        }
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
}
