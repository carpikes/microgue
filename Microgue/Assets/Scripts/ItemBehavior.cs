using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ItemBehavior : MonoBehaviour {
    public String mCategory;
    public Vector2 mCenter;
    List< KeyValuePair<Stat, float> > effects;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "MainCharacter")
            return;

        foreach( KeyValuePair<Stat, float> entry in effects )
        {
            entry.Key.CurrentValue += entry.Value;
        }
    }
}
