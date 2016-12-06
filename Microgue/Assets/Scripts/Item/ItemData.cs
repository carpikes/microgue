using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class ItemData {

    public enum ItemRarities
    {
        Common,
        Rare,
        Mystic
    }

    public ItemData()
    {
        values = new List<StatPair>();
    }

    private string name;
    private ItemRarities rarity;
    private string image;
    private bool isPassive;
    private List<StatPair> values;

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public ItemRarities Rarity
    {
        get
        {
            return rarity;
        }

        set
        {
            rarity = value;
        }
    }

    public List<StatPair> Values
    {
        get
        {
            return values;
        }

        set
        {
            values = value;
        }
    }

    public string Image
    {
        get
        {
            return image;
        }

        set
        {
            image = value;
        }
    }

    public bool IsPassive
    {
        get
        {
            return isPassive;
        }

        set
        {
            isPassive = value;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("NAME: " + name + "\n");
        sb.Append("Rarity: " + rarity + "\n");
        sb.Append("IS PASSIVE? " + isPassive + "\n");
        sb.Append("IMAGE: " + image + "\n");

        foreach (StatPair sp in Values)
        {
            sb.Append("EFFECT: " + sp.Key + " (" + sp.Value + ")\n");
        }

        return sb.ToString();
    }
}
