﻿using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;

public class ChasingBird : MonoBehaviour {
    private float mAcceleration = 5.0f;
    private float mFriction = 0.01f;
    private Vector2 mPoint;

    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Vector2 mVelocity = Vector2.zero;
    private Vector2 mCurTarget;
    private float mRemainingTime = 0.0f;

    // Use this for initialization
    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        ChooseNewPoint();
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

    void ChooseNewPoint() {
        Vector2 oldPoint = mPoint;
        mPoint = Random.onUnitSphere;
        mPoint.y /= 3.0f;
        mRemainingTime = Random.Range(0.1f, 0.3f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
        {
        }
    }
}