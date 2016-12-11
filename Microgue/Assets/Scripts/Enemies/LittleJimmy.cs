using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;

public class LittleJimmy : MonoBehaviour {
    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
	private EnemyAI mEnemyAI;
    
    // Use this for initialization
    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
		mEnemyAI = GetComponent<EnemyAI>();
		mEnemyAI.SetEnabled(true);
    }

    void FixedUpdate()
    {
		if(mEnemyAI != null)
			mEnemyAI.SetPosition (transform.position);
    }

    void ChooseNewPoint() {
        
    }
}
