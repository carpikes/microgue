using UnityEngine;
using System.Collections;

public class FireFox : MonoBehaviour {

    enum FirefoxStates
    {
        IDLE,
        ROLLING,
        SHOOTING
    }

    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private AIMap mAIMap;

    private EnemyPosition mEnemyPosition;

    // STAGES
    // 1 - idle/rolling
    // 2 - idle/shooting/rolling
    // 3 - idle/shooting/rolling
    FirefoxStates state;
    int stage = 1;

    int[] rollingSpeeds = new int[] { 10, 20, 30 };
    int[] shootingSpeeds = new int[] { 5, 10 };

    int cnt = 0;
    int limit = 60;

    // Use this for initialization
    void Start () {
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        state = FirefoxStates.IDLE;
    }
	
	// Update is called once per frame
	void Update () {
        ++cnt;

	    if( stage == 1 )
        {
            // update status
            if( cnt >= limit )
            {
                if(state == FirefoxStates.IDLE)
                {
                    state = FirefoxStates.ROLLING;
                } else
                {
                    state = FirefoxStates.IDLE;
                }
            }
        } else if( stage == 2 )
        {

        } else
        {

        }
	}
}
