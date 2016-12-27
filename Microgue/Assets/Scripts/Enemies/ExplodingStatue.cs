using UnityEngine;
using System.Collections;
using System;

public class ExplodingStatue : MonoBehaviour {

    enum States
    {
        IDLE,
        EXPLODING,
    }

    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Collider2D mCollider;

    private EnemyTouch mEnemyTouch;
    private EnemyPosition mEnemyPosition;

    private States mState;
    private Animator mAnimator;

    bool mSubjectToExplosion = false;

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

    void OnTriggerEnter2D( Collider2D other )
    {
        if (mState == States.IDLE)
        {
            mState = States.EXPLODING;
            mAnimator.SetTrigger("enemy_death");
        }

        mSubjectToExplosion = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        mSubjectToExplosion = false;
    }

    public void Explode()
    {
        if( mSubjectToExplosion )
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_HIT, null);
        }
    }

}
