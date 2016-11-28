using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

using StatPair = System.Collections.Generic.KeyValuePair<PlayerStats.StatStates, float>;

public class ItemManager : MonoBehaviour {

    public static ItemManager instance = null;

    [HideInInspector]
    public List<List<Item>> items;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        InitializeItemLists();
        parseItemFile();

        foreach (Item i in items[(int)Item.ItemCategories.Hearts])
            Debug.Log(i);
    }

    private void InitializeItemLists()
    {
        items = new List<List<Item>>();

        for( int i = 0; i < Enum.GetNames(typeof(Item.ItemCategories)).Length; ++i)
        {
            items.Add(new List<Item>());
        }
    }

    public void parseItemFile()
    {
        StreamReader reader = null;

        try {
            reader = File.OpenText("Assets\\items.csv");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] item_info = line.Split(',');

                // the file is structured like this
                // itemname, category, (enum, effect)+

                // Create item
                Item item = new Item();
                item.Name = item_info[0];
                item.Category = (Item.ItemCategories)Enum.Parse(typeof(Item.ItemCategories), item_info[1]);
                for (int i = 2; i < item_info.Length; i += 2)
                {
                    PlayerStats.StatStates stat = (PlayerStats.StatStates)Enum.Parse(typeof(PlayerStats.StatStates), item_info[i]);
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
