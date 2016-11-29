using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using StatPair = System.Collections.Generic.KeyValuePair<PlayerStats.StatStates, float>;

public class ItemBehavior : MonoBehaviour {

    [HideInInspector]
    public String mCategory;

    [HideInInspector]
    public Vector2 mCenter;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "MainCharacter")
            return;

        // UPDATE STAT FOR PLAYER
    }
}
