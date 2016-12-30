using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

    GameplayManager mGameManager;

    void Start()
    {
        mGameManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
    }

    void OnDestroy()
    {
        //mGameManager.CreateNewWorld(1);
    }
}
