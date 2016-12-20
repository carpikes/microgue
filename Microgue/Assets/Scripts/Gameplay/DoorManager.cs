using UnityEngine;
using System.Collections;
using RoomMapGenerator;
using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class DoorManager : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.StartListening(Events.ON_DOOR_TOUCH, OnDoorEnter);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_DOOR_TOUCH, OnDoorEnter);
    }

    public void OnDoorEnter(Bundle args)
    {
        WorldManager wm = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();

        if (!wm.AreAllEnemiesKilled())
        {
            EventManager.TriggerEvent(Events.ON_STILL_ENEMIES_LEFT, null);
            return;
        }

        string type;
        RoomMap.Door door, opposite;

        if (!args.TryGetValue(DoorBehavior.DOOR_TYPE_TAG, out type))
            return;

        switch (type) {
            case DoorBehavior.DOOR_DOWN:  door = RoomMap.Door.DOWN;  opposite = RoomMap.Door.UP;    break;
            case DoorBehavior.DOOR_UP:    door = RoomMap.Door.UP;    opposite = RoomMap.Door.DOWN;  break;
            case DoorBehavior.DOOR_LEFT:  door = RoomMap.Door.LEFT;  opposite = RoomMap.Door.RIGHT; break;
            case DoorBehavior.DOOR_RIGHT: door = RoomMap.Door.RIGHT; opposite = RoomMap.Door.LEFT;  break;
            default: return;
        }

        wm.OnDoorEnter(door, opposite);
    }
}
