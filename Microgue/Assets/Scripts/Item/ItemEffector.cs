using UnityEngine;
using System.Collections;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class ItemEffector : MonoBehaviour {

    PlayerManager playerManager;

    [HideInInspector]
    public ItemData item;

    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerManager>();
    }

	void OnTriggerEnter2D( Collider2D other )
    {
        if (other.CompareTag("Player"))
        {
            //EventManager.TriggerEvent(Events.ON_ITEM_PICKUP, item);
            if ( item.IsPassive )
                playerManager.UseItem(item);
            else
                playerManager.StoreItem(item);

            gameObject.SetActive(false);
        }
    }
}
