using System;
using UnityEngine;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class PlayerItemHandler : MonoBehaviour {

    ItemData currentActiveItem;
    StatManager statManager;

    // Use this for initialization
    void Start () {
        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
    }

    public void UseActiveItem()
    {
        UseItem(currentActiveItem);
        currentActiveItem = null;
    }

    public void UseItem(ItemData item)
    {
        if (item == null)
            return;

        //EventManager.TriggerEvent(Events.ON_ITEM_USE, item);
        foreach (StatPair pair in item.Values)
            statManager.UpdateStatValue(pair.Key, pair.Value);

        DoExtraActions(item);
    }

    public void StoreItem(ItemData item)
    {
        Debug.Assert(!item.IsPassive, "Trying to store a non-active item!");
        currentActiveItem = item;
    }

    private void DoExtraActions(ItemData item)
    {
        
    }
}
