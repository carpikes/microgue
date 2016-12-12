using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace RoomMapGenerator
{

    public class MapAssetManager
    {
        private MapGenerator mMapGenerator = null;
        private string[] mMaps;
        private List<int>[] mDoorsCatalog;
        private Dictionary<int, int> mMapCache;
        private int mStartMap = -1;

        public MapAssetManager(MapGenerator mapGen, List<string> maps)
        {
            mMapGenerator = mapGen;
            mMaps = maps.ToArray();
            mMapCache = new Dictionary<int,int>();

            mDoorsCatalog = new List<int>[16];
            for (int i = 0; i < 16; i++)
                mDoorsCatalog[i] = new List<int>();

            for (int i = 0; i < mMaps.Length; i++)
                mDoorsCatalog[GetMapDoors(mMaps[i]) & 0x0f].Add(i);
        }

        // la mappa name verra` usata SOLO come starting point
        public void SetStartMap(string name)
        {
            for (int i = 0; i < mMaps.Length; i++)
                if (mMaps[i] == name)
                {
                    mStartMap = i;
                    return;
                }

            Debug.LogError("Cannot find start map in my array.");
        }

        private int GetMapDoors(string mapname)
        {
            string assetPath = "Assets/Tiled2Unity/Prefabs/" + mapname + ".prefab";

            GameObject worldPrefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (worldPrefab == null)
                return 0;

            DoorBehavior[] doors = worldPrefab.GetComponentsInChildren<DoorBehavior>();
            if (doors == null)
            {
                // FIXME: dice che c'e` un data loss :(
//                GameObject.Destroy(worldPrefab);
                return 0;
            }

            int ret = 0;
            foreach (DoorBehavior db in doors)
            {
                switch (db.mType)
                {
                    case "Down":  ret |= (int)RoomMap.Door.DOWN; break;
                    case "Up":    ret |= (int)RoomMap.Door.UP; break;
                    case "Left":  ret |= (int)RoomMap.Door.LEFT; break;
                    case "Right": ret |= (int)RoomMap.Door.RIGHT; break;
                }
            }
//            GameObject.Destroy(worldPrefab);
            return ret;
        }

        public string GetMap(int n, int doors)
        {
            byte[] numOfOnes = { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };

            if (mMapCache.ContainsKey(n))
                return mMaps[mMapCache[n]];

            doors &= 0x0f;

            int chosen;

            if (mMapGenerator.GetStartRoomId() == n && mStartMap >= 0)
                chosen = mStartMap;
            else
            {
                List<int> validDoors = new List<int>();

                for (int i = 0; i < 16; i++)
                    if ((i & doors) == doors)
                        for (int j = 0; j < mDoorsCatalog[i].Count; j++)
                            for (int k = 0; k < 5 - numOfOnes[i & 0x0f]; k++)
                                validDoors.Add(mDoorsCatalog[i][j]);

                if (validDoors.Count == 0)
                {
                    Debug.LogError("Cannot load a map with at least these doors: " + doors);
                    return "";
                }
                chosen = validDoors[Random.Range(0, validDoors.Count)];
            }

            mMapCache[n] = chosen;
            return mMaps[chosen];
        }
    };
}
