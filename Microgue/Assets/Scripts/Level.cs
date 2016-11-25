using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;
using UnityEditor;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

public class Level : MonoBehaviour {
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
                Debug.LogError("Cannot load world: " + name);
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

        LoadDoors();
        LoadItems();
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

    private void LoadItems()
    {
        GameObject itemsContainer = GameObject.Find(mCurWorld.name + "/Items");
        if (itemsContainer == null) return; 
        ItemBehavior[] items = itemsContainer.GetComponentsInChildren<ItemBehavior>();
        if (items == null) return;

        foreach (ItemBehavior db in items)
        {

        }
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

    private void Spawn(SpawnBehavior s, GameObject childOf)
    {
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

}