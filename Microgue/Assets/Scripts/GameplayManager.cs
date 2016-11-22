using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;

public class GameplayManager : MonoBehaviour {

    public Transform aimTransform;
    public GameObject mEnemy;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        for (int i = 0; i < 50; i++)
        {
            GameObject obj = Instantiate(mEnemy);
            Vector2 pos = Random.insideUnitCircle * 10.0f;
            obj.transform.position = new Vector3(pos.x, pos.y, obj.transform.position.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
		
	}
}
