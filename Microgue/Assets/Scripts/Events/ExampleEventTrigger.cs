using UnityEngine;
using System.Collections;

public class ExampleEventTrigger : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.S))
			EventManager.TriggerEvent (Events.ON_GAME_QUIT);
	}
}
