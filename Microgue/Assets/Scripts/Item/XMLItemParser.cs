using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class XMLItemParser : MonoBehaviour {

    public static XMLItemParser instance = null;

    [HideInInspector]
    public List<List<ItemData>> items;

    public static readonly string ITEMS_FILE = "Assets/Scripts/Item/it.xml";

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

        for (int i = 0; i < System.Enum.GetNames(typeof(ItemData.ItemRarities)).Length; ++i)
        {
            items.Add(new List<ItemData>());
        }
    }

    public void parseItemFile()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(ITEMS_FILE);

        // Get elements
        XmlNodeList itemList = xmlDoc.GetElementsByTagName("item");

        // Display the results
        XmlNode firstEl = itemList[0];

        
    }
}
