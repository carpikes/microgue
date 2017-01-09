using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigLittleJimmy : MonoBehaviour {
    int mState;
    float mTime;
    private List<GameObject> mEnemies;
    private Rigidbody2D mRB;
    private GameObject mPlayer;
    int mGoNearOrFar = 0;
    private Vector2 mTarget = Vector2.zero;
    private Vector2[] mTargets = new Vector2[] {
        new Vector2(1.0f, -3.4f),
        new Vector2(8.7f, -3.4f),
        new Vector2(2.51f, -1.8f),
        new Vector2(8.1f, -1.8f),
        new Vector2(5.0f, -6.9f),
    };

	// Use this for initialization
	void Start () {
        mTime = 0;
        mState = 0;
        mEnemies = new List<GameObject>();
        mPlayer = GameObject.Find("MainCharacter");
        mRB = GetComponent<Rigidbody2D>();
	}

    void FixedUpdate() {
        Vector2 ppos = mPlayer.transform.position;
        Vector2 vel = mRB.velocity;
        if (mGoNearOrFar == 0)
        {
            ppos = mPlayer.transform.position - transform.position;
            if (ppos.magnitude > 3.0f)
                vel += ppos.normalized * 2.0f * Time.deltaTime;
        }
        else
        {
            if (mTarget == Vector2.zero)
                mTarget = mTargets[Random.Range(0, mTargets.Length)];

            ppos = mTarget - new Vector2(transform.position.x, transform.position.y);
            if (ppos.magnitude < 1.0f)
                mTarget = Vector2.zero;
            else
                vel += ppos.normalized * 5.0f * Time.deltaTime;
        } 

        vel *= 0.99f;
        mRB.velocity = vel;
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 ppos = mPlayer.transform.position - transform.position;

        switch (mState) {
            case 0:
                mGoNearOrFar = 0;
                if (ppos.magnitude < 5.0f)
                {
                    mTime = Time.time + 2.0f;
                    mState = 1;
                }
                break;
            case 1:
                if (Time.time > mTime)
                    mState = 2;
                break;
            case 2:
                mState = 3;
                mGoNearOrFar = 0;
                SetInvincible(true);
                SpawnSouls();
                break;
            case 3:
                if (AliveSouls() <= 0)
                {
                    SetInvincible(false);
                    mGoNearOrFar = 1; // run away
                    mState = 4;
                }
                break;
            case 4: // run away
                mTime = Time.time + 6.0f;
                mState = 1;
                break;
            default:
                Debug.LogError("Invalid state");
                break;
        }	
	}

    void SpawnSouls() {
        Transform parent = GameObject.Find("/WorldData").transform;
        Vector2 pos = transform.position;

        GameObject el = Resources.Load("AngrySoul") as GameObject;

        for (int i = 0; i < 3; i++)
        {
            GameObject go = GameObject.Instantiate(el);
            go.transform.position = pos + Random.insideUnitCircle;
            go.transform.parent = parent;
            go.GetComponent<AngrySoul>().HackStartShooting();
            mEnemies.Add(go);
        }

        EnemyLife life = GetComponent<EnemyLife>();
        if (life.mCurrentHP < 0.5f * life.GetTotalHP())
        {
            GameObject st = Resources.Load("StompStomp") as GameObject;

            for (int i = 0; i < 2; i++)
            {
                GameObject go = GameObject.Instantiate(st);
                go.transform.position = pos + Random.insideUnitCircle * 3.0f;
                go.transform.parent = parent;
                go.GetComponent<StompStomp>().mHackAttackInstant = true;
                mEnemies.Add(go);
            }
                
        }
    }

    int AliveSouls() {
        if (mEnemies.Count == 0)
            return 0;

        int n = 0;
        for (int i = 0; i < mEnemies.Count; i++)
        {
            if (mEnemies[i] != null) {
                EnemyLife e = mEnemies[i].GetComponentInChildren<EnemyLife>();
                if (e != null && e.mCurrentHP > 0)
                    n++;
            }
        }
        if (n == 0)
            mEnemies.Clear();
        return n;
    }

    void SetInvincible(bool value)
    {
        GetComponent<EnemyLife>().mIsInvincible = value;
    }
}
