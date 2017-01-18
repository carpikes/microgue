using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

    void OnDestroy()
    {
        EventManager.TriggerEvent(Events.ON_BOSS_KILLED, null);
    }
}
