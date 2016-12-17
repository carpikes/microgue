using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class ItemData {

    public static int PROB_COMMON = 80;
    public static int PROB_RARE = 95;
    public static int PROB_MYSTIC = 100;

    public enum ItemRarities
    {
        Common,
        Rare,
        Mystic
    }

    public ItemData()
    {
        Values = new List<StatPair>();
    }

    public string Name;
    public ItemRarities Rarity;
    public List<StatPair> Values;
    public string Image;
    public bool IsPassive;
    public string OnUseMethod, OnUseParams;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("NAME: " + Name + "\n");
        sb.Append("RARITY: " + Rarity + "\n");
        sb.Append("IS PASSIVE? " + IsPassive + "\n");
        sb.Append("IMAGE: " + Image + "\n");
        sb.Append("ONUSEM: " + OnUseMethod + "\n");
        sb.Append("ONUSEP: " + OnUseParams + "\n");

        foreach (StatPair sp in Values)
        {
            sb.Append("EFFECT: " + sp.Key + " (" + sp.Value + ")\n");
        }

        return sb.ToString();
    }
}
