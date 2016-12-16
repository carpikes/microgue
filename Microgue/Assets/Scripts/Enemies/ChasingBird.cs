using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;
using System;

public class ChasingBird : Enemy {
    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Vector2 mVelocity = Vector2.zero;
    private Vector2 mCurTarget;
    private float mRemainingTime = 0.0f;
    private float mAcceleration = 5.0f;
    private Vector2 mPoint;

    protected override void SetupEnemy()
    {
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 delta = mPlayerRb.position + mPoint - new Vector2(mRb.position.x, mRb.position.y);

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
    }

    void ChooseNewPoint()
    {
        mPoint = Random.onUnitSphere;
        mPoint.y /= 3.0f;
        mRemainingTime = Random.Range(0.1f, 0.3f);
    }
}
