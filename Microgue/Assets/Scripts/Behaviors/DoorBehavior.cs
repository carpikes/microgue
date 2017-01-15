using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class DoorBehavior : MonoBehaviour {
    public String mType;
    public String mNeedItem;
    public Vector2 mPosition;
    public const string DOOR_TYPE_TAG = "DOOR_TYPE";
    public const string DOOR_ITEM_TAG = "DOOR_ITEM";
    public const string DOOR_UP = "Up";
    public const string DOOR_DOWN = "Down";
    public const string DOOR_LEFT = "Left";
    public const string DOOR_RIGHT = "Right";

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name != "MainCharacter")
            return;

        if (mNeedItem == "")
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GameController");

            Bundle doorEventData = new Bundle();
            doorEventData.Add(DOOR_TYPE_TAG, mType);
            doorEventData.Add(DOOR_ITEM_TAG, mNeedItem);
            EventManager.TriggerEvent(Events.ON_DOOR_TOUCH, doorEventData);
        }
    }
}
