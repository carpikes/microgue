using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class WaterBlob : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float mMovementRadius = 1.0f;
    public float mMinStillTime = 8.0f;
    public float mMaxStillTime = 10.0f;
    public float mAcceleration = 0.5f;
    public float mFriction = 0.01f;

    [Header("Targeting Params")]
    public float mTargetingEntryDistance = 5f;

    [Header("Sprite Colors")]
    public Color mNotShootingColor;
    public Color mShootingColor;

    private GameObject mPlayer;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private Vector2 mVelocity = Vector2.zero;

    private Vector2 mInitialPosition;
    private Vector2 mCurrentTarget;

    private SpriteRenderer mSpriteRenderer;

    private EnemyPosition mEnemyPosition;

    // TODO? Moving to a common shooting script?
    private float lastShootTime = 0.0f;
    private const float shotCooldownTime = 2.0f;
    public GameObject darkBall;

    // State machines
    StateMachine<WaterBlob> mStateMachine;
    static State<WaterBlob> mIdleState = new IdleState();
    static State<WaterBlob> mMovingState = new MovingState();

    StateMachine<WaterBlob> mShootingStateMachine;
    static State<WaterBlob> mNotShootingState = new NotShootingState();
    static State<WaterBlob> mShootingState = new ShootingState();
    static State<WaterBlob> mGlobalState = new GlobalState();

    public void HackStartShooting()
    {
        mTargetingEntryDistance = 500000.0f;
    }

    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mPlayerRb = mPlayer.GetComponent<Rigidbody2D>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        mStateMachine = new StateMachine<WaterBlob>(this, mIdleState, mGlobalState);
        mShootingStateMachine = new StateMachine<WaterBlob>(this, mNotShootingState, null);

        mEnemyPosition = GetComponent<EnemyPosition>();

        mEnemyPosition.SetEnabled(true);
    }

    // Update is called once per frame
    void Update()
    {
        mStateMachine.Update();
        mShootingStateMachine.Update();
    }

    void FixedUpdate()
    {
        mStateMachine.FixedUpdate();
        mShootingStateMachine.FixedUpdate();
    }

    void Shoot()
    {
        if (Time.time - lastShootTime > shotCooldownTime)
        {
            for( int i = 0; i < 20; ++i )
            {
                GameObject lb = Instantiate(darkBall);
                lb.transform.localScale = new Vector3((float)(lb.transform.localScale.x * 0.7f), (float)(lb.transform.localScale.y * 0.7f), 0);
                lb.transform.position = transform.position;

                Vector3 noise = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                Vector2 direction = (mPlayer.transform.position - transform.position + noise).normalized;
                (lb.GetComponent<Rigidbody2D>()).velocity = direction * Random.Range(1f, 3f);
            }

            lastShootTime = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shot"))
            mShootingStateMachine.ChangeState(mShootingState);
    }

    void ChangeSpriteColor(Color color)
    {
        mSpriteRenderer.color = color;
    }

    sealed class IdleState : State<WaterBlob>
    {
        public void Update(WaterBlob owner) { }

        public void FixedUpdate(WaterBlob owner) { }

        public void OnEnter(WaterBlob owner)
        {
            owner.StartCoroutine(IdleCoroutine(owner));
        }

        public void OnExit(WaterBlob owner) { }

        IEnumerator IdleCoroutine(WaterBlob owner)
        {
            float sleepTime = Random.Range(owner.mMinStillTime, owner.mMaxStillTime);
            yield return new WaitForSeconds(sleepTime);

            ChooseNewTarget(owner);
        }

        void ChooseNewTarget(WaterBlob owner)
        {
            if (owner.mShootingStateMachine.IsCurrentState(mShootingState))
                owner.mCurrentTarget = new Vector2(owner.mPlayer.transform.position.x, owner.mPlayer.transform.position.y)
                    + Random.insideUnitCircle * owner.mMovementRadius;
            else
                owner.mCurrentTarget = owner.mInitialPosition + Random.insideUnitCircle * owner.mMovementRadius;

            owner.mStateMachine.ChangeState(WaterBlob.mMovingState);
        }
    }

    sealed class MovingState : State<WaterBlob>
    {
        public void Update(WaterBlob owner) { }

        public void FixedUpdate(WaterBlob owner)
        {
            Move(owner);
        }

        private static void Move(WaterBlob owner)
        {
            Vector2 delta = owner.mCurrentTarget - new Vector2(owner.transform.position.x, owner.transform.position.y);

            if (delta.sqrMagnitude < 0.05f)
                owner.mStateMachine.ChangeState(WaterBlob.mIdleState);
            else
                owner.mVelocity += delta.normalized * Time.fixedDeltaTime * owner.mAcceleration;
        }

        public void OnEnter(WaterBlob owner) { }

        public void OnExit(WaterBlob owner) { }
    }

    sealed class NotShootingState : State<WaterBlob>
    {
        public void Update(WaterBlob owner)
        {
            Vector2 delta = owner.mPlayer.transform.position - owner.transform.position;

            if (Mathf.Abs(delta.sqrMagnitude) < owner.mTargetingEntryDistance)
                owner.mShootingStateMachine.ChangeState(mShootingState);
        }

        public void FixedUpdate(WaterBlob owner) { }

        public void OnEnter(WaterBlob owner)
        {
            owner.ChangeSpriteColor(owner.mNotShootingColor);
        }

        public void OnExit(WaterBlob owner) { }
    }

    sealed class ShootingState : State<WaterBlob>
    {
        public void Update(WaterBlob owner) { }

        public void FixedUpdate(WaterBlob owner)
        {
            owner.Shoot();
        }

        public void OnEnter(WaterBlob owner)
        {
            owner.ChangeSpriteColor(owner.mShootingColor);
        }

        public void OnExit(WaterBlob owner) { }
    }

    sealed class GlobalState : State<WaterBlob>
    {
        public void FixedUpdate(WaterBlob owner)
        {
            owner.mVelocity *= (1.0f - owner.mFriction); // * Time.fixedDeltaTime);
            owner.mRb.position += owner.mVelocity * Time.fixedDeltaTime;

            owner.mEnemyPosition.SetWorldPosition(owner.mRb.position);

            //owner.transform.localScale = new Vector3(owner.mRb.position.x >= owner.mPlayerRb.position.x ? -1 : 1, 1, 1);
        }

        public void OnEnter(WaterBlob owner) { }

        public void OnExit(WaterBlob owner) { }

        public void Update(WaterBlob owner) { }
    }
}
