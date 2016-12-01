using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class ItemBehavior : MonoBehaviour {

    [HideInInspector]
    public String mCategory;

    [HideInInspector]
    public Vector2 mCenter;

}
