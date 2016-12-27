using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Random = UnityEngine.Random;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;

public class Level
{
    //private List<GameObject> mSpawnedItems;
    private string mName, mAssetPath;
    private GameObject mCurrentRoom;

    private Vector3[] mCameraBounds;
    private Dictionary<string, Vector2> mSpawnPoints;

    System.Random rnd = new System.Random();

    public readonly static string LEVEL_NAME_TAG = "LEVEL_NAME";
    public readonly static string LEVEL_UNLOAD_TAG = "LEVEL_UNLOAD";

    public GameObject GetWorld()
    {
        return mCurrentRoom;
    }

    public Level(int num, string name)
    {
        mAssetPath = name;
        mName = name + " - " + num;

        //Debug.Log("CREATING LEVEL: " + mAssetPath);

        mCurrentRoom = null;
        mSpawnPoints = new Dictionary<string, Vector2>();
        //mSpawnedItems = new ArrayList<GameObject>();
        mCameraBounds = new Vector3[2];
    }

    public void Load()
    {
        if (mCurrentRoom == null)
        {
            GameObject worldPrefab = Resources.Load(mAssetPath) as GameObject;
            Debug.Assert(worldPrefab != null, "Cannot load world at: " + mAssetPath);

            Bundle levelEventInfo = new Bundle();
            levelEventInfo.Add(LEVEL_NAME_TAG, mName);
            
            EventManager.TriggerEvent(Events.ON_LEVEL_BEFORE_LOADING, levelEventInfo);
            mCurrentRoom = GameObject.Instantiate(worldPrefab);
            mCurrentRoom.name = mName;
            LoadStuff();
            EventManager.TriggerEvent(Events.ON_LEVEL_BEFORE_LOADING, levelEventInfo);
        } else {
            mCurrentRoom.SetActive(true);
        }

    }

    private void LoadStuff() {
        GetBoundsOnLoad();

        // Load enemies
        GameObject spawnerContainer = GameObject.Find(mCurrentRoom.name + "/Spawns");
        GameObject enemies = new GameObject("Enemies");
        enemies.transform.parent = mCurrentRoom.transform;
        foreach (SpawnBehavior s in spawnerContainer.GetComponentsInChildren<SpawnBehavior>())
        {
            if (s.mWhat == "Player")
                mSpawnPoints["Spawn"] = s.mCenter;
            else
                SpawnEnemy(s, enemies);
        }
        GameObject.Destroy(spawnerContainer);

        GameObject itemContainer = GameObject.Find(mCurrentRoom.name + "/Items");
        GameObject items = new GameObject("Items");
        items.transform.parent = mCurrentRoom.transform;

        if (itemContainer)
        {
            foreach (ItemBehavior s in itemContainer.GetComponentsInChildren<ItemBehavior>())
                SpawnItem(s, items);
            GameObject.Destroy(itemContainer);
        }
        LoadDoors();

        SetMaterialOnWalls();
    }

    private void SetMaterialOnWalls()
    {
        PhysicsMaterial2D mat = Resources.Load<PhysicsMaterial2D>("Materials/Wall");
        PolygonCollider2D[] poly = GameObject.Find(mCurrentRoom.name + "/Collision").GetComponentsInChildren<PolygonCollider2D>();

        foreach (var p in poly)
            p.sharedMaterial = mat;
    }

    private void SpawnEnemy(SpawnBehavior s, GameObject childOf)
    {
        int n = Random.Range(s.mNumberMin, s.mNumberMax + 1);
        for (int i = 0; i < n; i++)
        {
            string enemy = GetEnemyFromType(s.mWhat);
            string prefabName = enemy;
            GameObject el = Resources.Load(prefabName) as GameObject;
            Debug.Assert(el != null, "Cannot load enemy: " + prefabName);

            GameObject go = GameObject.Instantiate(el);
            go.transform.position = Random.insideUnitCircle * s.mRadius + s.mCenter;
            go.transform.parent = childOf.transform;
        }
    }

    private string GetEnemyFromType(string enemyType)
    {
        List<string> enemyList;
        if( EnemyManager.enemyDictionary.TryGetValue(enemyType, out enemyList) )
        {
            return enemyList[ Random.Range(0, enemyList.Count) ];
        }

        Debug.LogError("Cannot retrieve enemy type in my dictionary: " + enemyType);
        return "ERROR";
    }

    private ItemData PickItemFromCategory(string str_category)
    {
        // retrieve list of items for the specified category
        ItemData.ItemRarities category = (ItemData.ItemRarities)Enum.Parse(typeof(ItemData.ItemRarities), str_category);
        List<ItemData> category_items = ItemParser.instance.items[(int)category];

        // pick a random item
        ItemData chosen_item = category_items[Random.Range(0, category_items.Count)];

        return chosen_item;
    }

    private void SpawnItem(ItemBehavior s, GameObject childOf)
    {
        string prefabName = "Item";

        ItemData item = PickItemFromCategory( ChooseCategory() );
        GameObject el = Resources.Load(prefabName) as GameObject;
        Debug.Assert(el != null, "Cannot spawn object: " + prefabName);

        GameObject go = GameObject.Instantiate(el);

        // set image
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        Sprite sprite = Resources.Load<Sprite>(item.Image);
        sr.sprite = sprite;

        // load item info: this will be needed to retrieve stat and effect
        ItemPrefabProperties itemInfo = go.GetComponent<ItemPrefabProperties>();
        itemInfo.item = item;

        go.transform.position = s.mCenter;
        go.transform.parent = childOf.transform;
        
    }

    private string ChooseCategory()
    {
        int n = rnd.Next(0, 100);

        if (n < ItemData.PROB_COMMON) return "Common";
        else if (n < ItemData.PROB_RARE) return "Rare";
        else return "Mystic";
    }

    public void Unload()
    {
        Bundle b = new Bundle();
        b.Add(LEVEL_UNLOAD_TAG, mCurrentRoom.ToString());
        EventManager.TriggerEvent(Events.ON_LEVEL_UNLOADING, b);

        mCurrentRoom.SetActive(false);
    }

    private void GetBoundsOnLoad()
    {
        Renderer r = GameObject.Find(mCurrentRoom.name + "/Background/water").GetComponent<Renderer>();
        if (r == null)
        {
            Debug.LogError("Cannot find 'water' layer used to get map size");
            return;
        }
        float ratio = Camera.main.aspect * 2.0f;
        mCameraBounds[0] = new Vector3(r.bounds.min.x + ratio, r.bounds.min.y + 2.0f);
        mCameraBounds[1] = new Vector3(r.bounds.max.x - ratio, r.bounds.max.y - 2.0f);
    }

    private void LoadDoors()
    {
        GameObject doorContainer = GameObject.Find(mCurrentRoom.name + "/Doors");
        if (doorContainer == null) return;
        DoorBehavior[] doors = doorContainer.GetComponentsInChildren<DoorBehavior>();
        if (doors == null) return;

        // load material for wall
        PhysicsMaterial2D mat = Resources.Load<PhysicsMaterial2D>("Materials/Wall");

        foreach (DoorBehavior db in doors)
        {
            BoxCollider2D coll = db.GetComponentInParent<BoxCollider2D>();
            coll.sharedMaterial = mat;
            Transform t = db.GetComponentInParent<Transform>();
            Vector2 spawnPos = t.position;
            float delta = 0.4f;
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
                case "Left":
                    spawnPos.x += coll.size.x + delta;
                    spawnPos.y -= coll.size.y / 2.0f;
                    break;
                case "Right":
                    spawnPos.x -= coll.size.x + delta;
                    spawnPos.y -= coll.size.y / 2.0f;
                    break;
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

    public int CountEnemies()
    {
        return GameObject.Find(mCurrentRoom.name + "/Enemies").transform.childCount - 
            GameObject.Find(mCurrentRoom.name + "/Enemies").GetComponentsInChildren<NoEnemyCount>().Length; // do not consider Enemies itself
    }
}