using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerStats : MonoBehaviour {

    public enum StatStates
    {
        HEALTH,
        DEFENCE,
        DAMAGE,
        TEMP_DISTORSION
    };

    public Stat[] stats;

    public void setStatValue( StatStates s, float value )
    {
        stats[(int)s].CurrentValue = value;
    }

    public float getStatValue( StatStates s ) { return stats[(int)s].CurrentValue; }

    // Use this for initialization
    void Start () {
        foreach (Stat s in stats)
            s.ResetToMin();
	}

}
