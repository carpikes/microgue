using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;
using UnityEditor;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour {
    public GameObject mPlayer;
    public GameObject mAimCursor;

    private int mNumLevels;
    private int mCurLevelNum;
    private string[] mWorlds;
    private Vector3[] mCameraBounds;
    private Dictionary<string, ObjectPool> mObjectPools;
    private GameObject mCurWorld = null;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;

        mObjectPools = new Dictionary<string, ObjectPool>();
        string[] mAvailWorlds = { "ex1", "ex2" };

        mCameraBounds = new Vector3[2];
        mNumLevels = Random.Range(3, 8);
        mWorlds = new string[mNumLevels];
        for (int i = 0; i < mNumLevels; i++)
            mWorlds[i] = mAvailWorlds[Random.Range(0, mAvailWorlds.Length)];
        mCurLevelNum = 0;

        Debug.Log("NumLevels: " + mNumLevels);
        LoadLevel("Spawn");
    }

    void LoadLevel(string spawnPoint) {
        string levelAsset = "Assets/Tiled2Unity/Prefabs/" + mWorlds[mCurLevelNum] + ".prefab";

        GameObject worldPrefab = AssetDatabase.LoadAssetAtPath(levelAsset, typeof(GameObject)) as GameObject;
        if (worldPrefab == null) {
            Debug.LogError("Cannot load world: " + name);
            return;
        }
    
        if (mCurWorld != null)
            Destroy(mCurWorld);

        mCurWorld = Instantiate(worldPrefab);

        GetBounds();

        // Load enemies
        GameObject spawnerContainer = GameObject.Find(mCurWorld.name + "/Spawns");
        foreach (SpawnBehavior s in spawnerContainer.GetComponentsInChildren<SpawnBehavior>())
        {
            if (s.mWhat == "Player")
            {
                if(spawnPoint == "Spawn")
                    MovePlayerTo(s.mCenter);
                // altrimenti ignora lo spawn point. spawna poi dalla porta
            }
            else
                Spawn(s, mCurWorld);
        }
        Destroy(spawnerContainer);

        if (spawnPoint != "Spawn")
        {
            HandleDoors(spawnPoint);
            HandleItems(spawnPoint);

        }
    }

    private void HandleItems(string spawnPoint)
    {
        foreach (ItemBehavior db in GameObject.Find(mCurWorld.name + "/Items").GetComponentsInChildren<ItemBehavior>())
        {

        }
    }

    private void HandleDoors(string spawnPoint)
    {
        foreach (DoorBehavior db in GameObject.Find(mCurWorld.name + "/Doors").GetComponentsInChildren<DoorBehavior>())
        {
            if (spawnPoint == db.mType)
            {
                BoxCollider2D coll = db.GetComponentInParent<BoxCollider2D>();
                Transform t = db.GetComponentInParent<Transform>();
                Vector2 spawnPos = t.position;
                float delta = 0.1f;
                switch (db.mType)
                {
                    case "Down": spawnPos.y += coll.size.y + delta; spawnPos.x += coll.size.x / 2.0f; break;
                    case "Up": spawnPos.y -= coll.size.y + delta; spawnPos.x += coll.size.x / 2.0f; break;
                    default: MovePlayerTo(new Vector2(0, 0)); break;
                        //case "Left": spawnPos.x += coll.size.x + delta; break;
                        //case "Right":spawnPos.x -= coll.size.x + delta; break;
                }
                MovePlayerTo(spawnPos);
                break;
            }
        }
    }

    void MovePlayerTo(Vector2 coords)
    {
        Rigidbody2D p = mPlayer.GetComponent<Rigidbody2D>();
        p.MovePosition(coords);
    }

    void Spawn(SpawnBehavior s, GameObject childOf) {
        int n = Random.Range(s.mNumberMin, s.mNumberMax+1);
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

    public void OnDoorEnter(string type) {
        Debug.Log("OnDoorEnter: " + type + " - " + mCurLevelNum);
        if (mCurLevelNum == 0 && type == "Down")
            return;
        if (mCurLevelNum == mNumLevels - 1 && type == "Up")
            return;

        switch (type) {
            case "Down":
                mCurLevelNum--;
                LoadLevel("Up");
                break;
            case "Up":
                mCurLevelNum++;
                LoadLevel("Down");
                break;
        }
    }

    public Vector3[] GetCameraBounds() {
        return mCameraBounds;
    }

    void GetBounds() {
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
}
