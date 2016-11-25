using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour {

    // ------- to be refactored! :) ------------
    // Dictionary<string, Stat> stats;

    [Header("Ranges for each stat")]
    public int minHealth;
    public int maxHealth;

    public int minDefence;
    public int maxDefence;

    public float minDamage;
    public float maxDamage;

    public float minTemporalDistortion;
    public float maxTemporalDistortion;

    private int health;
    private float defence;
    private float damage;
    private float temporalDistortion;

    // How much life the player has
    public int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
        }
    }

    // How much the attacks are reduced according to the formula
    // loss = floor( enemy_atk * (MAX_DEFENCE - Defence))
    public float Defence
    {
        get
        {
            return defence;
        }

        set
        {
            defence = value;
        }
    }

    // how much an attack by the player is amplified
    // dmg_points = attack * Damage
    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    // multiplier for the time
    public float TemporalDistortion
    {
        get
        {
            return temporalDistortion;
        }

        set
        {
            temporalDistortion = value;
        }
    }

    // Use this for initialization
    void Start () {
        InitStats();
	}
	
    void InitStats()
    {
        health = minHealth;
        defence = minDefence;
        damage = minDamage;
        temporalDistortion = minTemporalDistortion;
    }
}
