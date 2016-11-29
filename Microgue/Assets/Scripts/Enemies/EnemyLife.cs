using UnityEngine;
using System.Collections;
using POLIMIGameCollective;

public class DamageOnShot : MonoBehaviour {

    public int hp = 2;
	// Use this for initialization
	void Start () {
        	
	}
	
	// Update is called once per frame
	void Update () {
        	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
        {
            EventManager.TriggerEvent("OnEnemyShot");
            hp--;
        }
        if (hp == 0)
        {
            EventManager.TriggerEvent("OnEnemyKilled");
            Destroy(gameObject);
        }
    }
}
