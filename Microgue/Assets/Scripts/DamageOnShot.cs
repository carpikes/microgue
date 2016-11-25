using UnityEngine;
using System.Collections;

public class DamageOnShot : MonoBehaviour {
    public int mShotsToDie = 2;
	// Use this for initialization
	void Start () {
        	
	}
	
	// Update is called once per frame
	void Update () {
        	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
            mShotsToDie--;
        if (mShotsToDie == 0)
        {
            Destroy(gameObject);
        }
    }
}
