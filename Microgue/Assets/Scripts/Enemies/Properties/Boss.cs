using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

    void Start()
    {
    }

    void OnDestroy()
    {
        EventManager.TriggerEvent(Events.ON_BOSS_KILLED, null);
    }
}
