using UnityEngine;
using System.Collections;
using System;

public class ExplodingStatue : MonoBehaviour {

    public enum States
    {
        IDLE,
        EXPLODING,
    }

    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Collider2D mCollider;

    private EnemyTouch mEnemyTouch;
    private EnemyPosition mEnemyPosition;

    public States mState;
    public Animator mAnimator;

    public bool mSubjectToExplosion = false;

    // Use this for initialization
    void Start () {
        mRb = GetComponent<Rigidbody2D>();
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        mAnimator = GetComponent<Animator>();
        mEnemyTouch = GetComponent<EnemyTouch>();
        mEnemyTouch.mDamageEnabled = false;

        mCollider = GetComponent<Collider2D>();

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        mState = States.IDLE;
    }

    public void Explode()
    {
        if( mSubjectToExplosion )
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_HIT, null);
        }
    }

}
