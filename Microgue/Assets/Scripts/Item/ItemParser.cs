using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;
using System.Text.RegularExpressions;

public class ItemParser : MonoBehaviour {

    public static ItemParser instance = null;

    [HideInInspector]
    public List<List<ItemData>> items;

    public static readonly string ITEMS_FILE = "Assets/Scripts/Item/items.csv";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        InitializeItemLists();
        parseItemFile();
    }

    private void InitializeItemLists()
    {
        items = new List<List<ItemData>>();

        for( int i = 0; i < Enum.GetNames(typeof(ItemData.ItemRarities)).Length; ++i)
        {
            items.Add(new List<ItemData>());
        }
    }

    public void parseItemFile()
    {
        TextAsset file = Resources.Load("items") as TextAsset;
        String[] lines = Regex.Split(file.text, "\n|\r|\r\n");

        foreach (string line in lines)
        {
            string line2 = line.Trim();
            if (line2.StartsWith("#") || line2 == "")
                continue;

            string[] item_info = line2.Split(',');

            for (int i = 0; i < item_info.Length; ++i)
                item_info[i] = item_info[i].Trim();

            // Create item
            ItemData item = new ItemData();
            item.Name = item_info[0];
            item.Rarity = (ItemData.ItemRarities)Enum.Parse(typeof(ItemData.ItemRarities), item_info[1]);
            item.Image = item_info[2];
            item.IsPassive = bool.Parse(item_info[3]);
            item.OnUseMethod = item_info[4].Trim();
            item.OnUseParams = item_info[5].Trim();

            for (int i = 6; i < item_info.Length; i += 2)
            {
                StatManager.StatStates stat = (StatManager.StatStates)Enum.Parse(typeof(StatManager.StatStates), item_info[i]);
                item.Values.Add(new StatPair(stat, float.Parse(item_info[i + 1])));
            }

            // populate correct sublist
            items[(int)item.Rarity].Add(item);

            //Debug.Log(item.Name + " added!");
        }
    }
}
