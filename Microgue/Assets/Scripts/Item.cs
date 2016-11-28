using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class Item {

    public enum ItemCategories
    {
        Hearts
    }

    public Item()
    {
        values = new List<KeyValuePair<PlayerStats.StatStates, float>>();
    }

    private string name;
    private ItemCategories category;

    private List<KeyValuePair<PlayerStats.StatStates, float>> values;

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

    public List<KeyValuePair<PlayerStats.StatStates, float>> Values
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
        sb.Append("CATEGORY: " + Enum.Parse(typeof(Item.ItemCategories), category + "\n"));

        return sb.ToString();
    }
}
