using UnityEngine;
using System.Collections;

public class StompStomp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 a = gameObject.transform.position;
        float theta = Time.time * 7.0f;
        a.y = -5.0f + Mathf.Pow(Mathf.Abs(Mathf.Cos(theta)),1.5f) / 0.5f;
        a.z = 10 * Mathf.Abs(Mathf.Cos(theta));
        gameObject.transform.position = a;
	}

    void OnTriggerEnter2D(Collider2D other) {
        Vector3 a = gameObject.transform.position;
        if (a.z < 3.0f)
        {
            Debug.Log("Stomp hit " + other.name + " " + Time.time + ": " + gameObject.transform.position);
        }
    }
}
