using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;
using UnityEditor;

public class GameplayManager : MonoBehaviour {

    public Transform aimTransform;
    public GameObject mSpawnerContainer;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        foreach (Spawner s in mSpawnerContainer.GetComponentsInChildren<Spawner>())
            Spawn(s);
        Destroy(mSpawnerContainer);
    }
	
	// Update is called once per frame
	void Update () {
	
		
	}

    void Spawn(Spawner s) {
        int n = Random.Range(s.mNumberMin, s.mNumberMax);
        for (int i = 0; i < n; i++)
        {
            string prefabName = "Assets/Prefab/" + s.mWhat + ".prefab";
            GameObject el = AssetDatabase.LoadAssetAtPath(prefabName, typeof(GameObject)) as GameObject;
            if (el != null) {
                GameObject go = Instantiate(el);
                go.transform.position = Random.insideUnitCircle * s.mRadius + s.mCenter;
            }
        }
    }
}
