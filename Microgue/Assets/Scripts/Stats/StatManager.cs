using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StatManager : MonoBehaviour {
    
    public enum StatStates
    {
        CURRENT_HEALTH,
        MAX_HEALTH,
        DEFENCE,
        DAMAGE,
        TEMP_DISTORSION,
        SPEED,

        NULL
    };

    const int numberOfStates = (int)StatStates.NULL;
    public Stat[] stats;

    bool isInvulnerable = false;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_MAIN_CHAR_HIT, DecreaseEnergy);
        EventManager.StartListening(Events.ON_MAIN_CHAR_INVULNERABLE_BEGIN, SetInvulnerable);
        EventManager.StartListening(Events.ON_MAIN_CHAR_INVULNERABLE_END, SetVulnerable);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_HIT, DecreaseEnergy);
        EventManager.StopListening(Events.ON_MAIN_CHAR_INVULNERABLE_BEGIN, SetInvulnerable);
        EventManager.StopListening(Events.ON_MAIN_CHAR_INVULNERABLE_END, SetVulnerable);

    }

    private void SetVulnerable(Dictionary<string, string> arg0)
    {
        isInvulnerable = false;
    }

    private void SetInvulnerable(Dictionary<string, string> arg0)
    {
        isInvulnerable = true;
    }

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
        SetupStat(StatStates.SPEED, 1, 10);

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
        if (!isInvulnerable)
        {
            UpdateStatValue(StatStates.CURRENT_HEALTH, -1);

            if( stats[(int)StatStates.CURRENT_HEALTH].CurrentValue <= 0 )
            {
                EventManager.TriggerEvent(Events.ON_MAIN_CHAR_DEATH, null);
            }
        }
    }
    
}
