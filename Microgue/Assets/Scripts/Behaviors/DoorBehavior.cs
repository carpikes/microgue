using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DoorBehavior : MonoBehaviour {
    public String mType;
    public String mNeedItem;
    public Vector2 mPosition;

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name != "MainCharacter")
            return;

        if (mNeedItem == "")
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameController");
            GameplayManager ggm = gm.GetComponent<GameplayManager>();
            ggm.OnDoorEnter(mType);
        }
    }
}
