using UnityEngine;
using System.Collections;

public class EnemyTouch : MonoBehaviour {

    private bool mDamageEnabled = true;

    public bool DamageEnabled
    {
        get
        {
            return mDamageEnabled;
        }

        set
        {
            mDamageEnabled = value;
        }
    }

    // Use this for initialization
    void Start () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (mDamageEnabled && other.CompareTag("Player"))
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_HIT, null);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (mDamageEnabled && other.collider.CompareTag("Player"))
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_HIT, null);
        }
    }
}
