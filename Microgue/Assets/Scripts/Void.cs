using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Void : MonoBehaviour {

    WorldManager mWorldManager = null;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_ENEMY_DEATH, StartActualGame);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_ENEMY_DEATH, StartActualGame);
    }

    void Start()
    {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        mWorldManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();
        Debug.Assert(mWorldManager != null, "no world manager found");
    }

    private void StartActualGame(Dictionary<string, string> arg0)
    {
        if( mWorldManager.CountEnemies() == 1 )
        {
            // hack: force to next level
            EventManager.TriggerEvent(Events.VOID_COMPLETED, null);
        }
    }
}
