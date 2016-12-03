using UnityEngine;
using System.Collections;

public class EnemyShot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_HIT, null);
        }
    }
}
