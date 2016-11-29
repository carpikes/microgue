using UnityEngine;
using POLIMIGameCollective;
using UnityEditor;
using System.Collections.Generic;

using Random = UnityEngine.Random;
using System;

// TODO: TUTTO DA REFACTORARE!
public class Level : ScriptableObject {
    //private List<GameObject> mSpawnedItems;
    private string mName, mAssetPath;
    private GameObject mCurWorld;

    private Vector3[] mCameraBounds;
    private Dictionary<string, Vector2> mSpawnPoints;

    public Level(int num, string name)
    {
        mAssetPath = "Assets/Tiled2Unity/Prefabs/" + name + ".prefab";
        mName = name + " - " + num;

        mCurWorld = null;
        mSpawnPoints = new Dictionary<string, Vector2>();
        //mSpawnedItems = new ArrayList<GameObject>();
        mCameraBounds = new Vector3[2];
    }

    public void Load()
    {
        if (mCurWorld == null)
        {
            GameObject worldPrefab = AssetDatabase.LoadAssetAtPath(mAssetPath, typeof(GameObject)) as GameObject;
            if (worldPrefab == null)
            {
                Debug.LogError("Cannot load world: " + mName);
                return;
            }

            EventManager.TriggerEvent("BeforeLoadLevel");
            mCurWorld = Instantiate(worldPrefab);
            mCurWorld.name = mName;
            LoadStuff();
            EventManager.TriggerEvent("AfterLoadLevel");
        } else {
            mCurWorld.SetActive(true);
        }

    }

    private void LoadStuff() {
        GetBoundsOnLoad();

        // Load enemies
        GameObject spawnerContainer = GameObject.Find(mCurWorld.name + "/Spawns");
        foreach (SpawnBehavior s in spawnerContainer.GetComponentsInChildren<SpawnBehavior>())
        {
            if (s.mWhat == "Player")
            {
                mSpawnPoints["Spawn"] = s.mCenter;
                // altrimenti ignora lo spawn point. spawna poi dalla porta
            }
            else
                Spawn(s, mCurWorld);
        }
        Destroy(spawnerContainer);

        GameObject itemContainer = GameObject.Find(mCurWorld.name + "/Items");
        GameObject items = new GameObject("Items");
        items.transform.parent = mCurWorld.transform;

        if (itemContainer)
        {
            //Debug.Log("item container found");// OK
            foreach (ItemBehavior s in itemContainer.GetComponentsInChildren<ItemBehavior>())
            {
                SpawnItem(s, items);
            }
            Destroy(itemContainer);
        }
        LoadDoors();
        //LoadItems();
    }

    private void Spawn(SpawnBehavior s, GameObject childOf)
    {
        if (s.mWhat == "Enemy")
            s.mWhat = "BadEnemy";
        int n = Random.Range(s.mNumberMin, s.mNumberMax + 1);
        for (int i = 0; i < n; i++)
        {
            string prefabName = "Assets/Prefab/" + s.mWhat + ".prefab";
            GameObject el = AssetDatabase.LoadAssetAtPath(prefabName, typeof(GameObject)) as GameObject;
            if (el != null)
            {
                GameObject go = Instantiate(el);
                go.transform.position = Random.insideUnitCircle * s.mRadius + s.mCenter;
                go.transform.parent = childOf.transform;
            }
            else
                Debug.LogError("I'm trying to spawn an invalid object: " + prefabName + " !");
        }
    }

    private ItemData PickItemFromCategory(string str_category)
    {
        // retrieve list of items for the specified category
        ItemData.ItemCategories category = (ItemData.ItemCategories)Enum.Parse(typeof(ItemData.ItemCategories), str_category);
        List<ItemData> category_items = ItemManager.instance.items[(int)category];

        // pick a random item
        ItemData chosen_item = category_items[Random.Range(0, category_items.Count)];

        return chosen_item;
    }

    private void SpawnItem(ItemBehavior s, GameObject childOf)
    {
        string category = s.mCategory;
        string prefabName = "Assets/Prefab/Item.prefab";

        ItemData item = PickItemFromCategory(category);
        GameObject el = AssetDatabase.LoadAssetAtPath(prefabName, typeof(GameObject)) as GameObject;

        if (el != null)
        {
            GameObject go = Instantiate(el);

            // set image
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            Sprite sprite = AssetDatabase.LoadAssetAtPath("Assets/Images/Sprites/Items/" + item.Image, typeof(Sprite)) as Sprite;
            sr.sprite = sprite;

            // load item info: this will be needed to retrieve stat and effect
            ItemEffector itemInfo = go.GetComponent<ItemEffector>();
            itemInfo.item = item;

            go.transform.position = s.mCenter;
            go.transform.parent = childOf.transform;
        }
        else
            Debug.LogError("I'm trying to spawn an invalid item: " + prefabName + " !");
    }

    public void Unload()
    {
        mCurWorld.SetActive(false);
    }

    private void GetBoundsOnLoad()
    {
        Renderer r = GameObject.Find(mCurWorld.name + "/Background/water").GetComponent<Renderer>();
        if (r == null)
        {
            Debug.LogError("Cannot get renderer");
            return;
        }
        float ratio = Camera.main.aspect * 2.0f;
        mCameraBounds[0] = new Vector3(r.bounds.min.x + ratio, r.bounds.min.y + 2.0f);
        mCameraBounds[1] = new Vector3(r.bounds.max.x - ratio, r.bounds.max.y - 2.0f);
    }

    

    

    private void LoadDoors()
    {
        GameObject doorContainer = GameObject.Find(mCurWorld.name + "/Doors");
        if (doorContainer == null) return;
        DoorBehavior[] doors = doorContainer.GetComponentsInChildren<DoorBehavior>();
        if (doors == null) return;

        foreach (DoorBehavior db in doors)
        {
            BoxCollider2D coll = db.GetComponentInParent<BoxCollider2D>();
            Transform t = db.GetComponentInParent<Transform>();
            Vector2 spawnPos = t.position;
            float delta = 0.1f;
            switch (db.mType)
            {
                case "Down":
                    spawnPos.y += coll.size.y + delta;
                    spawnPos.x += coll.size.x / 2.0f;
                    break;
                case "Up":
                    spawnPos.y -= coll.size.y + delta;
                    spawnPos.x += coll.size.x / 2.0f;
                    break;
                    //case "Left": spawnPos.x += coll.size.x + delta; break;
                    //case "Right":spawnPos.x -= coll.size.x + delta; break;
            }
            mSpawnPoints[db.mType] = spawnPos;
        }
    }

    public Vector3[] GetCameraBounds() {
        return mCameraBounds;
    }

    public Vector2 GetPlayerStartPos(string spawnPoint)
    {
        if (!mSpawnPoints.ContainsKey(spawnPoint))
        {
            Debug.LogError("Cannot find player spawn point (" + spawnPoint + ")");
            return new Vector2(0, 0);
        }

        return mSpawnPoints[spawnPoint];
    }

    

}