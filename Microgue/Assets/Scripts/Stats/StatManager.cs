﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StatManager : MonoBehaviour {

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_MAIN_CHAR_HIT, DecreaseEnergy);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_HIT, DecreaseEnergy);
    }

    public enum StatStates
    {
        CURRENT_HEALTH,
        MAX_HEALTH,
        DEFENCE,
        DAMAGE,
        TEMP_DISTORSION,


        NULL
    };

    const int numberOfStates = (int)StatStates.NULL;

    public Stat[] stats;

    private void SetupStat(StatStates s, float min, float max)
    {
        stats[(int)s] = new Stat(s.ToString(), min, max);
    }

    public void Start()
    {
        stats = new Stat[ numberOfStates ];

        SetupStat(StatStates.MAX_HEALTH, 3, 10);
        SetupStat(StatStates.CURRENT_HEALTH, 0, stats[(int)StatStates.MAX_HEALTH].CurrentValue);
        SetupStat(StatStates.DEFENCE, 1, 10);
        SetupStat(StatStates.DAMAGE, 1, 10);
        SetupStat(StatStates.TEMP_DISTORSION, 1, 10);

        stats[(int)StatStates.CURRENT_HEALTH].CurrentValue = stats[(int)StatStates.CURRENT_HEALTH].mMax;
    }

    public void UpdateStatValue( StatStates s, float delta )
    {
        SetStatValue(s, stats[(int)s].CurrentValue + delta);
    }

    public void SetStatValue( StatStates s, float v )
    {
        Stat currentStat = stats[(int)s];

        currentStat.CurrentValue = v;
        Mathf.Clamp(currentStat.CurrentValue, currentStat.mMin, currentStat.mMax);
        
        if( s == StatStates.MAX_HEALTH )
        {
            Stat currHealth = stats[(int)StatStates.CURRENT_HEALTH];
            currHealth.mMax = currentStat.CurrentValue;

            // if I decrease the max health, maybe the current healt is invalid now, fix with clamp
            Mathf.Clamp(currHealth.CurrentValue, currHealth.mMin, currHealth.mMax);
        }

        EventManager.TriggerEvent(Events.ON_STAT_CHANGED, null);
    }

    public float GetStatValue( StatStates s ) { return stats[(int)s].CurrentValue; }

    private void DecreaseEnergy(Dictionary<string, string> arg0)
    {
        UpdateStatValue(StatStates.CURRENT_HEALTH, -1);
    }
       
}
