﻿using UnityEngine;
using System.Collections;

public class BadEnemy : MonoBehaviour
{
    public float mMovementRadius = 3.0f;
    public float mMinStillTime = 8.0f;
    public float mMaxStillTime = 10.0f;
    public float mAcceleration = 0.5f;
    public float mFriction = 0.01f;

    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private Vector2 mInitialPosition;
    private Vector2 mTargetDist;

    private Vector2 mVelocity = Vector2.zero;
    private Vector2 mCurTarget;
    private enum EnemyStates
    {
        STILL,
        MOVING,
        TARGETING,
    };
    private EnemyStates mCurState;
    // Use this for initialization
    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        ChooseNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        Vector2 playerdelta = (mPlayerRb.transform.position - transform.position);
        float dist = playerdelta.magnitude;
        if (dist < 2.0f && mCurState != EnemyStates.TARGETING) {
            mCurState = EnemyStates.TARGETING;
            mTargetDist = Random.onUnitSphere;
            Debug.Log("Targeting player");
        }

        if (mCurState == EnemyStates.TARGETING)
        {
            mCurTarget = mPlayerRb.transform.position;
            mCurTarget += mTargetDist;
        }

        Vector2 delta = mCurTarget - new Vector2(transform.position.x, transform.position.y);
        if (mCurState == EnemyStates.MOVING || mCurState == EnemyStates.TARGETING) {
            if (Mathf.Abs(delta.sqrMagnitude) < 0.05f)
            {
                if(mCurState != EnemyStates.TARGETING)
                    mCurState = EnemyStates.STILL;
                StartCoroutine(stillCoroutine());
            }
            else
                mVelocity += delta.normalized * Time.fixedDeltaTime * mAcceleration;
        }

        mVelocity *= (1.0f - mFriction);
        mRb.position += mVelocity * Time.fixedDeltaTime;
    }

    IEnumerator stillCoroutine()
    {
        float sleepTime = Random.Range(mMinStillTime, mMaxStillTime);
        if (mCurState == EnemyStates.TARGETING)
            sleepTime = 0.0f;
        yield return new WaitForSeconds(sleepTime);
        ChooseNewTarget(); 
    }

    void ChooseNewTarget()
    {
        if (mCurState != EnemyStates.TARGETING)
        {
            mCurTarget = mInitialPosition + Random.insideUnitCircle * mMovementRadius;
            mCurState = EnemyStates.MOVING;
        } else {
            mTargetDist = Random.onUnitSphere;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
        {
            mCurState = EnemyStates.TARGETING;
            mTargetDist = Random.onUnitSphere;
        }
    }
}
