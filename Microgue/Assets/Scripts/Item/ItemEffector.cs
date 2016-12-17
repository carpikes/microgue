using UnityEngine;
using System.Collections;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;
using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class ItemEffector : MonoBehaviour {

    PlayerItemHandler playerManager;

    [HideInInspector]
    public ItemData item;

    public static readonly string ITEM_PICKUP_TAG = "ITEM_PICKUP";

    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerItemHandler>();
    }

	void OnTriggerEnter2D( Collider2D other )
    {
        if (other.CompareTag("Player"))
        {
            Bundle itemBundle = new Bundle();
            itemBundle.Add(ITEM_PICKUP_TAG, item.Name);
            EventManager.TriggerEvent(Events.ON_ITEM_PICKUP, itemBundle);

            if ( item.IsPassive )
                playerManager.UseItem(item);
            else
                playerManager.StoreItem(item);

            gameObject.SetActive(false);
        }
    }
}
