using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerStats : MonoBehaviour {

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

    private void SetStat(StatStates s, float min, float max)
    {
        stats[(int)s] = new Stat(s.ToString(), min, max);
    }

    public void Start()
    {
        stats = new Stat[ numberOfStates ];

        SetStat(StatStates.MAX_HEALTH, 3, 10);
        SetStat(StatStates.CURRENT_HEALTH, 0, stats[(int)StatStates.MAX_HEALTH].CurrentValue);
        SetStat(StatStates.DEFENCE, 1, 2);
        SetStat(StatStates.DAMAGE, 1, 2);
        SetStat(StatStates.TEMP_DISTORSION, 1, 2);

        stats[(int)StatStates.CURRENT_HEALTH].CurrentValue = stats[(int)StatStates.CURRENT_HEALTH].mMax;
    }

    public void setStatValue( StatStates s, float value )
    {
        stats[(int)s].CurrentValue = value;
    }

    public float getStatValue( StatStates s ) { return stats[(int)s].CurrentValue; }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyShot"))
        {
            stats[(int)StatStates.CURRENT_HEALTH].CurrentValue--;
            Destroy(other.gameObject);
        }
    }
}
