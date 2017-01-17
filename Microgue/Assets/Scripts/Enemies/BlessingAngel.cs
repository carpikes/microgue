using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class BlessingAngel : MonoBehaviour {
    enum States
    {
        SETUP,
        ENEMY_TARGETED,
        LOOKING_FOR_ENEMY,
        DYING
    }

    private Rigidbody2D mRb;
    public List<GameObject> mProtectedEnemies;
    private GameObject mCurrentTargetEnemy = null;
    WorldManager mWorldManager;
    EnemyLife mEnemyLife;

    private Vector2 mVelocity = Vector2.zero;
    private float mRemainingTime = 0.0f;
    private float mAcceleration = 1.0f;
    private Vector2 mTargetPoint;

    private EnemyPosition mEnemyPosition;

    private States mState;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_ENEMY_DEATH, DisableCurrentEnemy);
    }

    void Disable()
    {
        EventManager.StopListening(Events.ON_ENEMY_DEATH, DisableCurrentEnemy);
    }

    private void DisableCurrentEnemy(Dictionary<string, string> arg0)
    {
        mCurrentTargetEnemy = null;
        Debug.Log("Enemy disabled");
    }

    void Start()
    {
        mProtectedEnemies = new List<GameObject>();
        mRb = GetComponent<Rigidbody2D>();

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        mEnemyLife = GetComponent<EnemyLife>();
        mState = States.SETUP;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        mWorldManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();
        Debug.Assert(mWorldManager != null, "no world manager found");

        ChooseNewEnemy();
        ChooseNewPoint();
    }

    private GameObject ChooseNewEnemy()
    {
        if (mWorldManager == null)
            return null;

        if (mWorldManager.AreAllEnemiesKilled())
            return null;

        // ok chosen new enemy
        mState = States.ENEMY_TARGETED;
        return mWorldManager.RandomEnemy();
    }

    void FixedUpdate()
    {
        // wait initialization of world manager first!
        if (mState == States.SETUP || mState == States.DYING)
            return;

        if (mCurrentTargetEnemy == null)
        {
            mState = States.LOOKING_FOR_ENEMY;

            if (mWorldManager.CountEnemies() == 1)
            {
                mState = States.DYING;
                mEnemyLife.InstaKill();
            }
            else {
                mCurrentTargetEnemy = ChooseNewEnemy();
            }
        }

        if (mState == States.DYING)
            return;

        Vector2 delta = new Vector2(mCurrentTargetEnemy.transform.position.x, mCurrentTargetEnemy.transform.position.y)
            + mTargetPoint - new Vector2(mRb.position.x, mRb.position.y);

        mRemainingTime -= Time.fixedDeltaTime;
        if (mRemainingTime < 0)
            ChooseNewPoint();

        if (delta.magnitude > 0.3f)
        {
            mVelocity += delta.normalized * Time.fixedDeltaTime * mAcceleration;
            if (delta.magnitude > 0.6f)
                mVelocity.x *= 0.99f;
            mVelocity.y *= 0.95f;
        } 

        mRb.position += mVelocity * Time.fixedDeltaTime;
        mEnemyPosition.SetWorldPosition(mRb.position);
    }

    void ChooseNewPoint()
    {
        mTargetPoint = Random.onUnitSphere;
        mTargetPoint.y /= 3.0f;
        mRemainingTime = Random.Range(0.1f, 0.3f);
    }

    void Update()
    {
        //Debug.Log(mState.ToString());
    }
}
