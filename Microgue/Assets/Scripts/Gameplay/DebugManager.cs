using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class DebugManager : MonoBehaviour {

    public Canvas debugCanvas;
    public Text statText;
    public Text timerText;

    public bool isDebugVisible = true;

    GameObject mainCharacter;
    StatManager playerStats;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_STAT_CHANGED, OnStatChanged);
        EventManager.StartListening(Events.ON_TICK, OnSecondPassed);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_STAT_CHANGED, OnStatChanged);
        EventManager.StopListening(Events.ON_TICK, OnSecondPassed);
    }

    private void OnSecondPassed(Bundle args)
    {
        //timerText.text = args.
    }

    private void OnStatChanged(Bundle args)
    {
        UpdateStatText();
    }


    // Use this for initialization
    void Start () {
        if (!isDebugVisible)
        {
            debugCanvas.enabled = false;
            return;
        }

        mainCharacter = GameObject.FindGameObjectWithTag("Player");
        UpdateStatText();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateDebugText();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            statText.enabled = (isDebugVisible ? false : true);
            isDebugVisible = !isDebugVisible;
        }
    }

    private void UpdateStatText()
    {
        statText.text = "DEBUG (Z to toggle)\n";
        ShowPlayerStats();
    }

    private void ShowPlayerStats()
    {
        playerStats = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
        if ( playerStats )
        {
            foreach( Stat s in playerStats.stats )
            {
                statText.text += s.mName + ": " + s.CurrentValue + "\n";
            }
        } else
        {
            Debug.LogError("Cannot retrieve player stats component");
        }
    }
}
