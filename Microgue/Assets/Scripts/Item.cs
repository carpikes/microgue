using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

using StatPair = System.Collections.Generic.KeyValuePair<PlayerStats.StatStates, float>;

public class Item {

    public enum ItemCategories
    {
        Hearts
    }

    public Item()
    {
        values = new List<StatPair>();
    }

    private string name;
    private ItemCategories category;

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

    public ItemCategories Category
    {
        get
        {
            return category;
        }

        set
        {
            category = value;
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

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("NAME: " + name + "\n");
        sb.Append("CATEGORY: " + category + "\n");

        foreach (StatPair sp in Values)
        {
            sb.Append("EFFECT: " + sp.Key + " (" + sp.Value + ")\n");
        }

        return sb.ToString();
    }
}
