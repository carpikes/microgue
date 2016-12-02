using UnityEngine;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class PlayerManager : MonoBehaviour {

    ItemData currentActiveItem;
    StatManager statManager;

    
    // Use this for initialization
    void Start () {
        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
    }

    public void UseActiveItem()
    {
        PickUpItem(currentActiveItem);
        currentActiveItem = null;
    }

    public void PickUpItem(ItemData item)
    {
        if (item == null)
            return;

        //EventManager.TriggerEvent(Events.ON_ITEM_USE, item);
        foreach (StatPair pair in item.Values)
            statManager.updateStatValue(pair.Key, pair.Value);
    }

    public void StoreItem(ItemData item)
    {
        if (!item.IsPassive)
            currentActiveItem = item;
        else
            Debug.LogError("Trying to store a non-active item!");
    }
}
