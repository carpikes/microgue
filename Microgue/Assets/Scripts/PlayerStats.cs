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
        stats[(int)StatStates.CURRENT_HEALTH].CurrentValue = stats[(int)StatStates.MAX_HEALTH].mMin;
	}

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyShot"))
        {
            stats[(int)StatStates.CURRENT_HEALTH].CurrentValue--;
            Destroy(other.gameObject);
        }
    }
}
