using System;
using System.Reflection;
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

        Invoke(item.OnUseMethod, item.OnUseParams);
    }

    public void StoreItem(ItemData item)
    {
        Debug.Assert(!item.IsPassive, "Trying to store a non-active item!");
        currentActiveItem = item;
    }

    void Invoke(string method, string param)
    {
        if (method.Length > 0)
        {
            MethodInfo mi = typeof(ItemActions).GetMethod(method, BindingFlags.Static | BindingFlags.Public);
            if (mi == null)
            {
                Debug.Log("Cannot invoke: ItemActions::" + method + "()");
                return;
            }
            object[] parameters = new object[] { param };
            mi.Invoke(null, parameters);
        }
    }
}
