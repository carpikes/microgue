using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StompStomp : MonoBehaviour
{
    enum EnemyStatus
    {
        WAITING,
        FALLING,
        IDLE,
        JUMPING,
    };

    Transform mStompStompEnemy;
    Transform mStompStompShadow;

    private EnemyTouch mEnemyTouch;

    private EnemyStatus mStatus;
    private Vector2 mVelocity = Vector2.zero;
    private Vector2 mCurTarget = Vector2.zero;
    private Vector2 mRenderingOffset = Vector2.zero;

    public float mGravity = 9.80665f;
    public float mJumpAccel = 3.0f;
    public float mMinWait = 0.1f;
    public float mMaxWait = 0.5f;
    public float mJumpSize = 1.0f;

    private Rigidbody2D mEnemyRb;
    private Collider2D mEnemyCollider;
    private Transform mPlayerTransform;
    private InputManager mInputManager;

    //private Animator mAnimator;

    private EnemyPosition mEnemyPosition;

    // Used for shadow projection
    private Vector2 mMovingDirection, mJumpStartPosition, mShadowOffset;
    private bool mDontMoveShadow;
    public bool mHackAttackInstant = false; // used in da boss

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_STILL_ENEMIES_LEFT, DoorTouch);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_STILL_ENEMIES_LEFT, DoorTouch);
    }

    private void DoorTouch(Dictionary<string, string> arg0)
    {
        OnShadowTouch();
    }

    // Use this for initialization
    public void Start () 
	{
        mStompStompEnemy = transform.FindChild("StompStompEnemy");
        mStompStompShadow = transform.FindChild("StompStompShadow");

        mEnemyRb = mStompStompEnemy.GetComponent<Rigidbody2D>();
        mEnemyCollider = mStompStompEnemy.GetComponent<Collider2D>();
        mPlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //mAnimator = mStompStompEnemy.GetComponent<Animator>();
        //Debug.Assert(mAnimator != null, "Cannot find animator");

        mShadowOffset = mStompStompShadow.localPosition;
        mRenderingOffset = mEnemyRb.transform.localPosition;

        mEnemyTouch = mStompStompEnemy.GetComponent<EnemyTouch>();
        Debug.Assert(mEnemyTouch != null, "no enemy touch found!");

        mJumpStartPosition = mEnemyRb.transform.position;
        mJumpStartPosition -= mRenderingOffset;

        mMovingDirection = new Vector2(0, 0);
        mStatus = EnemyStatus.WAITING;
        mEnemyTouch.mDamageEnabled = false;
        mInputManager = GameObject.Find("MainCharacter").GetComponent<InputManager>();

        Vector2 pos = new Vector2(0.0f, 10.0f);
        mEnemyRb.position = mEnemyRb.position + pos + mRenderingOffset;
        mStompStompEnemy.position = mEnemyRb.position + pos + mRenderingOffset;

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(false);

        if (mHackAttackInstant)
            OnShadowTouch();
        // mStateMachine = new StateMachine<StompStomp>(this, mIdleState, null); // mGlobalState);
    }

    void Update()
    {
        // mStateMachine.Update();
    }

    IEnumerator JumpCoroutine()
    {
        float waitTime = UnityEngine.Random.Range(mMinWait, mMaxWait);
        yield return new WaitForSeconds(waitTime);
        mEnemyCollider.enabled = false;
        BeginJumpToTarget();
    }

    void BeginJumpToTarget()
    {
        Vector2 newTarget = mPlayerTransform.position;
        Vector2 pos = mEnemyRb.transform.position;
        pos -= mRenderingOffset;

        Vector2 ds = newTarget - pos;

        // Clamp jump magnitude
        if (ds.magnitude > mJumpSize)
        {
            ds = ds.normalized * mJumpSize;
            newTarget = pos + ds;
        }

        float angle = Mathf.PI / 4.0f;
        float ax = mJumpAccel * Mathf.Cos(angle);
        float ay = mJumpAccel * Mathf.Sin(angle);
        float jumpTime = 2 * (ay / mGravity + Mathf.Sqrt(ay * ay + 2 * mGravity * Mathf.Abs(ds.y))) / mGravity;
        float jumpX = ax * jumpTime;
        
        // If jump is too far using PI/4 as angle, change the angle
        if (ds.x * ds.x < jumpX * jumpX)
        { 
            // The loop is because jumpTime depends on the angle.. and the angle depends on jumpTime.
            // It converges up to the 4th decimal if looped for 10 times, 5 is enough for us :)
            for (int i = 0; i < 5; i++)
            {
                angle = Mathf.Acos(Mathf.Abs(ds.x) / (mJumpAccel * jumpTime));
                ay = mJumpAccel * Mathf.Sin(angle);
                jumpTime = 2 * (ay / mGravity + Mathf.Sqrt(ay * ay + 2 * mGravity * Mathf.Abs(ds.y))) / mGravity;
            }
        }

        mVelocity.x = mJumpAccel * Mathf.Cos(angle) * Mathf.Sign(ds.x);
        mVelocity.y = mJumpAccel * Mathf.Sin(angle);
        mCurTarget = newTarget;
        mMovingDirection = ds.normalized;
        mJumpStartPosition = mEnemyRb.position - mRenderingOffset;

        mDontMoveShadow = ((mJumpStartPosition - mCurTarget).magnitude < 0.1);
        mStatus = EnemyStatus.JUMPING;
        mEnemyTouch.mDamageEnabled = false;
        //mEnemyPosition.SetEnabled(false);
        //mAnimator.SetTrigger("jumping");
    }   

    // Update is called once per frame
    void FixedUpdate ()
	{
        // mStateMachine.FixedUpdate();

        if (mStatus != EnemyStatus.JUMPING && mStatus != EnemyStatus.FALLING)
            return;

        mVelocity.y -= mGravity * Time.fixedDeltaTime;

        Vector2 newPosition = mEnemyRb.transform.position;
        newPosition -= mRenderingOffset;
        newPosition.x += mVelocity.x * Time.fixedDeltaTime;
        newPosition.y += mVelocity.y * Time.fixedDeltaTime;

        if (newPosition.y <= mCurTarget.y && mVelocity.y < 0.05)
        {
            if (mStatus == EnemyStatus.FALLING)
            {
                // INITIAL FALLING
                newPosition.y = mCurTarget.y;
                mInputManager.ShakeCamera(0.15f, 2.0f);
            }
            else {
                // falling from usual jump
                mInputManager.ShakeCamera(0.13f, 1.0f);
                //mAnimator.SetTrigger("end_jump");
            }

            mVelocity = Vector2.zero;
            mStatus = EnemyStatus.IDLE;
            mEnemyTouch.mDamageEnabled = true;
            mEnemyPosition.SetEnabled(true);
            mEnemyCollider.enabled = true;
            //mAnimator.SetTrigger("idle");

            StartCoroutine(JumpCoroutine());
        }

        if (mStatus == EnemyStatus.JUMPING || mStatus == EnemyStatus.IDLE)
        {
            // Shadow Projection
            float dot = Vector2.Dot(newPosition - mJumpStartPosition, mMovingDirection);
            if(!mDontMoveShadow)
                mStompStompShadow.position = mJumpStartPosition + dot * mMovingDirection + mShadowOffset;
        }

        mEnemyRb.transform.position = newPosition + mRenderingOffset;
        mEnemyPosition.SetWorldPosition(mEnemyRb.position);
    }

    public void OnShadowTouch() {
        if (mStatus == EnemyStatus.WAITING)
        {
            mStatus = EnemyStatus.FALLING;
            mCurTarget = mStompStompShadow.position;
            mCurTarget -= mShadowOffset;
            //mAnimator.SetTrigger("falling");
        }
    }
    
}
