using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DoorBehavior : MonoBehaviour {
    public String mType;
    public String mNeedItem;
    public Vector2 mPosition;

    public void OnTriggerEnter2D()
    {
        if (mNeedItem == "")
        {
            GameObject gm = GameObject.Find("GameplayManager");
            GameplayManager ggm = gm.GetComponent<GameplayManager>();
            ggm.OnDoorEnter(mType);
        }
    }
}
