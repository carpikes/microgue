using UnityEngine;
using System.Collections;
using System;

public class ExplodingStatue : MonoBehaviour {

    public enum States
    {
        IDLE,
        EXPLODING,
    }

    private EnemyTouch mEnemyTouch;
    private EnemyPosition mEnemyPosition;

    public States mState;
    public Animator mAnimator;

    public bool mSubjectToExplosion = false;

    // Use this for initialization
    void Start () {
        mAnimator = GetComponent<Animator>();
        mEnemyTouch = GetComponent<EnemyTouch>();
        mEnemyTouch.mDamageEnabled = false;

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

        GetComponent<EnemyLife>().Die();
    }

}
