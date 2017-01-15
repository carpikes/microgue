using UnityEngine;
using System.Collections;
using RoomMapGenerator;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class DoorManager : MonoBehaviour
{
    private RoomMap.Door mLastDoor, mLastOpposite;
    private WorldManager mWorldManager = null;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_DOOR_TOUCH, OnDoorEnter);
        EventManager.StartListening(Events.ON_ENEMY_DEATH, TryUnlockDoors);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_DOOR_TOUCH, OnDoorEnter);
        EventManager.StopListening(Events.ON_ENEMY_DEATH, TryUnlockDoors);
    }

    private void UnlockDoor(Transform t)
    {
        if (t == null) return;
        GameObject g = t.gameObject;
        if (g == null) return;
        g.SetActive(false);
    }

    private void TryUnlockDoors(Bundle args)
    {
        mWorldManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();
        if (mWorldManager == null)
            return;

        // l'update viene fatto dopo, quindi al momento della morte ce n'è ancora uno vivo...
        if (mWorldManager.CountEnemies() > 1)
            return;

        GameObject world = mWorldManager.GetWorld();
        if (world == null)
            return;

        RoomInfo room = mWorldManager.GetMapGenerator().GetRoom(mWorldManager.GetCurrentRoomId());

        if (room.HasDoor(RoomMap.Door.UP))
            UnlockDoor(world.transform.FindChild("DBNorth"));
        if (room.HasDoor(RoomMap.Door.DOWN))
            UnlockDoor(world.transform.FindChild("DBSouth"));
        if (room.HasDoor(RoomMap.Door.LEFT))
            UnlockDoor(world.transform.FindChild("DBWest"));
        if (room.HasDoor(RoomMap.Door.RIGHT))
            UnlockDoor(world.transform.FindChild("DBEast"));

        EventManager.TriggerEvent(Events.ON_DOOR_UNLOCK, null);
    }

    IEnumerator FadeOut() {
        GetComponents<FMODUnity.StudioEventEmitter>()[2].Play();
        EventManager.TriggerEvent(Events.FADE_OUT, null);
        yield return new WaitForSeconds(0.2f);
        mWorldManager.OnDoorEnter(mLastDoor, mLastOpposite);
        EventManager.TriggerEvent(Events.FADE_IN, null);
    }

    public void OnDoorEnter(Bundle args)
    {
        mWorldManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();
        // skip control if flag enabled
        if (!mWorldManager.AreAllEnemiesKilled())
        {
            GetComponents<FMODUnity.StudioEventEmitter>()[1].Play();
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

        // avoiding fadeout if the door is closed
        RoomInfo room = mWorldManager.GetMapGenerator().GetRoom(mWorldManager.GetCurrentRoomId());
        if (!room.HasDoor(door))
            return;

        mLastDoor = door;
        mLastOpposite = opposite;
        StartCoroutine(FadeOut());
    }
}
