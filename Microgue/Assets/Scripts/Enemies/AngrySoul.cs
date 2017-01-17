using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class AngrySoul : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float mMovementRadius = 3.0f;
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
    private Rigidbody2D mRb;
    private Vector2 mVelocity = Vector2.zero;

    private Vector2 mInitialPosition;
    private Vector2 mCurrentTarget;

    private SpriteRenderer mSpriteRenderer;

    private EnemyPosition mEnemyPosition;

    // TODO? Moving to a common shooting script?
    private float lastShootTime = 0.0f;
    private const float shotCooldownTime = 1.0f;
    public GameObject darkBall;

    // State machines
    StateMachine<AngrySoul> mStateMachine;
    static State<AngrySoul> mIdleState = new IdleState();
    static State<AngrySoul> mMovingState = new MovingState();

    StateMachine<AngrySoul> mShootingStateMachine;
    static State<AngrySoul> mNotShootingState = new NotShootingState();
    static State<AngrySoul> mShootingState = new ShootingState();
    static State<AngrySoul> mGlobalState = new GlobalState();

    public void HackStartShooting()
    {
        mTargetingEntryDistance = 500000.0f;
    }

    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        mStateMachine = new StateMachine<AngrySoul>(this, mIdleState, mGlobalState);
        mShootingStateMachine = new StateMachine<AngrySoul>(this, mNotShootingState, null);

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
            GameObject lb = Instantiate(darkBall);
            lb.transform.position = transform.position;

            Vector2 direction = (mPlayer.transform.position - transform.position).normalized;
            (lb.GetComponent<Rigidbody2D>()).velocity = direction * 5.0f;

            lastShootTime = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
            mShootingStateMachine.ChangeState(mShootingState);
    }

    void ChangeSpriteColor( Color color )
    {
        mSpriteRenderer.color = color;
    }

    sealed class IdleState : State<AngrySoul>
    {
        public void Update(AngrySoul owner) { }

        public void FixedUpdate(AngrySoul owner) { }

        public void OnEnter(AngrySoul owner)
        {
            owner.StartCoroutine(IdleCoroutine(owner));
        }

        public void OnExit(AngrySoul owner) { }

        IEnumerator IdleCoroutine(AngrySoul owner)
        {
            float sleepTime = Random.Range(owner.mMinStillTime, owner.mMaxStillTime);
            yield return new WaitForSeconds(sleepTime);

            ChooseNewTarget(owner);
        }

        void ChooseNewTarget(AngrySoul owner)
        {
            if( owner.mShootingStateMachine.IsCurrentState(mShootingState) )
                owner.mCurrentTarget = new Vector2(owner.mPlayer.transform.position.x, owner.mPlayer.transform.position.y) 
                    + Random.insideUnitCircle * owner.mMovementRadius;
            else
                owner.mCurrentTarget = owner.mInitialPosition + Random.insideUnitCircle * owner.mMovementRadius;

            owner.mStateMachine.ChangeState(AngrySoul.mMovingState);
        }
    }

    sealed class MovingState : State<AngrySoul>
    {
        public void Update(AngrySoul owner) { }

        public void FixedUpdate(AngrySoul owner)
        {
            Move(owner);
        }

        private static void Move(AngrySoul owner)
        {
            Vector2 delta = owner.mCurrentTarget - new Vector2(owner.transform.position.x, owner.transform.position.y);

            if (delta.sqrMagnitude < 0.05f)
                owner.mStateMachine.ChangeState(AngrySoul.mIdleState);
            else
                owner.mVelocity += delta.normalized * Time.fixedDeltaTime * owner.mAcceleration;
        }

        public void OnEnter(AngrySoul owner) { }

        public void OnExit(AngrySoul owner) { }
    }

    sealed class NotShootingState : State<AngrySoul>
    {
        public void Update(AngrySoul owner) {
            Vector2 delta = owner.mPlayer.transform.position - owner.transform.position;

            if (Mathf.Abs(delta.sqrMagnitude) < owner.mTargetingEntryDistance)
                owner.mShootingStateMachine.ChangeState(mShootingState);
        }

        public void FixedUpdate(AngrySoul owner) { }

        public void OnEnter(AngrySoul owner) {
            owner.ChangeSpriteColor(owner.mNotShootingColor);
        }

        public void OnExit(AngrySoul owner) { }
    }

    sealed class ShootingState : State<AngrySoul>
    {
        public void Update(AngrySoul owner) { }

        public void FixedUpdate(AngrySoul owner) {
            owner.Shoot();
        }

        public void OnEnter(AngrySoul owner) {
            owner.ChangeSpriteColor(owner.mShootingColor);
        }

        public void OnExit(AngrySoul owner) { }
    }

    sealed class GlobalState : State<AngrySoul>
    {
        public void FixedUpdate(AngrySoul owner)
        {
            owner.mVelocity *= (1.0f - owner.mFriction); // * Time.fixedDeltaTime);
            owner.mRb.position += owner.mVelocity * Time.fixedDeltaTime;

            owner.mEnemyPosition.SetWorldPosition(owner.mRb.position);

        }

        public void OnEnter(AngrySoul owner) { }

        public void OnExit(AngrySoul owner) { }

        public void Update(AngrySoul owner) { }
    }
}
