using UnityEngine;
using System.Collections;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class ItemEffector : MonoBehaviour {

    StatManager statManager;

    [HideInInspector]
    public ItemData item;

    void Start()
    {
        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
    }

	void OnTriggerEnter2D( Collider2D other )
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(item);

            // activate all effects on stat
            foreach (StatPair pair in item.Values)
            {
                StatManager.StatStates stat = pair.Key;
                float delta = pair.Value;

                statManager.updateStatValue(stat, delta);
            }

            // disable after pickup
            gameObject.SetActive(false);
        }
    }
}
