using UnityEngine;
using System.Collections;

public class StompStompBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy() {
        Destroy(transform.parent.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Stomp hit " + other.name + " " + Time.time + ": " + gameObject.transform.position);
    }
}
