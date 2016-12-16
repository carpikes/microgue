using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class AngrySoul : Enemy
{
    [Header("Movement Parameters")]
    public float mMovementRadius = 3.0f;
    public float mMinStillTime = 8.0f;
    public float mMaxStillTime = 10.0f;
    public float mAcceleration = 0.5f;
    public float mFriction = 0.01f;

    [Header("Targeting Params")]
    public float mTargetingDistance = 2.0f;

    private GameObject mPlayer;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private Vector2 mVelocity = Vector2.zero;

    private Vector2 mInitialPosition;
    private Vector2 mCurrentTarget;

    // TODO? Moving to a common shooting script?
    private float lastShootTime = 0.0f;
    private const float shotCooldownTime = 1.0f;
    public GameObject darkBall;

    // State machine
    private StateMachine<AngrySoul> mStateMachine;
    static State<AngrySoul> mIdleState = new IdleState();
    static State<AngrySoul> mMovingState = new MovingState();
    static State<AngrySoul> mTargetingState = new TargetingState();
    static State<AngrySoul> mGlobalState = new GlobalState();

    protected override void SetupEnemy()
    {
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mPlayerRb = mPlayer.GetComponent<Rigidbody2D>();

        mStateMachine = new StateMachine<AngrySoul>(this, mIdleState, mGlobalState);
    }

    // Update is called once per frame
    void Update()
    {
        mStateMachine.Update();
    }

    void FixedUpdate()
    {
        Debug.Log(mStateMachine.CurrentState.ToString());
        mStateMachine.FixedUpdate();
    }

    void CheckIfStillReached()
    {
        Vector2 delta = mCurrentTarget - new Vector2(transform.position.x, transform.position.y);

        if (Mathf.Abs(delta.sqrMagnitude) < 0.05f)
            if ( !mStateMachine.IsCurrentState(mTargetingState) )
                mStateMachine.ChangeState(mIdleState);
        else
            mVelocity += delta.normalized * Time.fixedDeltaTime * mAcceleration;
    }

    void CheckIfTargetingPlayer()
    {
        Vector2 playerDelta = mPlayerRb.transform.position - transform.position;
        float distance = playerDelta.magnitude;

        if (distance < mTargetingDistance)
        {
            mStateMachine.ChangeState(mTargetingState);
        }
    }

    void Shoot()
    {
        // --------- SEPARATE SCRIPT ------------
        if (Time.time - lastShootTime > shotCooldownTime)
        {
            GameObject lb = Instantiate(darkBall);
            lb.transform.position = transform.position;

            Vector2 direction = (mPlayer.transform.position - transform.position).normalized;
            (lb.GetComponent<Rigidbody2D>()).velocity = direction * 5.0f;

            lastShootTime = Time.time;
        }
        // --------- SEPARATE SCRIPT ------------
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
            mStateMachine.ChangeState(mTargetingState);
    }

    sealed class IdleState : State<AngrySoul>
    {
        public void FixedUpdate(AngrySoul owner)
        {
            owner.CheckIfTargetingPlayer();
        }

        public void OnEnter(AngrySoul owner)
        {
            owner.StartCoroutine(IdleCoroutine(owner));
        }

        IEnumerator IdleCoroutine(AngrySoul owner)
        {
            float sleepTime = Random.Range(owner.mMinStillTime, owner.mMaxStillTime);
            yield return new WaitForSeconds(sleepTime);
            ChooseNewTarget(owner);
        }

        void ChooseNewTarget(AngrySoul owner)
        {
            owner.mCurrentTarget = owner.mInitialPosition + Random.insideUnitCircle * owner.mMovementRadius;
            owner.mStateMachine.ChangeState(mMovingState);
        }

        public void OnExit(AngrySoul owner)
        {
            // ;
        }

        public void Update(AngrySoul owner)
        {
            owner.Shoot();
        }
    }

    sealed class MovingState : State<AngrySoul>
    {
        public void FixedUpdate(AngrySoul owner)
        {
            owner.CheckIfTargetingPlayer();
            owner.CheckIfStillReached();
        }

        public void OnEnter(AngrySoul owner)
        {
            // ;
        }

        public void OnExit(AngrySoul owner)
        {
            // ;
        }

        public void Update(AngrySoul owner)
        {
            // ;
        }
    }

    sealed class TargetingState : State<AngrySoul>
    {
        public void FixedUpdate(AngrySoul owner)
        {
            owner.mCurrentTarget = owner.mPlayerRb.transform.position + Random.onUnitSphere * 2.0f;
            owner.CheckIfStillReached();
        }

        public void OnEnter(AngrySoul owner)
        {
            owner.StartCoroutine(EntryTargetCoroutine(owner));
        }

        IEnumerator EntryTargetCoroutine(AngrySoul owner)
        {
            float sleepTime = Random.Range(0.4f, 0.5f);
            yield return new WaitForSeconds(sleepTime);
        }

        public void OnExit(AngrySoul owner)
        {
            // ;
        }

        public void Update(AngrySoul owner)
        {
            // ;
        }
    }

    sealed class GlobalState : State<AngrySoul>
    {
        public void FixedUpdate(AngrySoul owner)
        {
            owner.mVelocity *= (1.0f - owner.mFriction);
            owner.mRb.position += owner.mVelocity * Time.fixedDeltaTime;

            // transform.localScale = new Vector3(mRb.position.x >= mPlayerRb.position.x ? -1 : +1, 1, 1);
        }

        public void OnEnter(AngrySoul owner)
        {
            // ;
        }

        public void OnExit(AngrySoul owner)
        {
            // ;
        }

        public void Update(AngrySoul owner)
        {
            // ;
        }
    }
}
