using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;

public class ItemParser : MonoBehaviour {

    public static ItemParser instance = null;

    [HideInInspector]
    public List<List<ItemData>> items;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        InitializeItemLists();
        parseItemFile();
    }

    private void InitializeItemLists()
    {
        items = new List<List<ItemData>>();

        for( int i = 0; i < Enum.GetNames(typeof(ItemData.ItemCategories)).Length; ++i)
        {
            items.Add(new List<ItemData>());
        }
    }

    public void parseItemFile()
    {
        StreamReader reader = null;

        try {
            reader = File.OpenText("Assets/items.csv");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line == "")
                    continue;

                string[] item_info = line.Split(',');

                for (int i = 0; i < item_info.Length; ++i)
                    item_info[i] = item_info[i].Trim();

                // Create item
                ItemData item = new ItemData();
                item.Name = item_info[0];
                item.Category = (ItemData.ItemCategories)Enum.Parse(typeof(ItemData.ItemCategories), item_info[1]);
                item.Image = item_info[2];
                item.IsPassive = bool.Parse(item_info[3]);

                for (int i = 4; i < item_info.Length; i += 2)
                {
                    StatManager.StatStates stat = (StatManager.StatStates)Enum.Parse(typeof(StatManager.StatStates), item_info[i]);
                    item.Values.Add(new StatPair(stat, float.Parse(item_info[i + 1])));
                }

                // populate correct sublist
                items[(int)item.Category].Add(item);
            }
        } catch( Exception e )
        {
            Debug.LogError("Error while parsing item file: " + e);
            Application.Quit(); // applies only in build file!
        }   
    }
}
