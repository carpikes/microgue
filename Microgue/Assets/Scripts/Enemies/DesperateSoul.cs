using UnityEngine;
using System.Collections;

public class DesperateSoul : Enemy
{
    [Header("Movement Parameters")]
    public float mMovementRadius = 3.0f;
    public float mMinStillTime = 8.0f;
    public float mMaxStillTime = 10.0f;
    public float mAcceleration = 0.5f;
    public float mFriction = 0.01f;

    private Rigidbody2D mRb;
    private Rigidbody2D mPlayerRb;
    private Vector2 mVelocity = Vector2.zero;

    private Vector2 mInitialPosition;
    private Vector2 mCurrentTarget;

    // State machine
    private StateMachine<DesperateSoul> mStateMachine;
    static State<DesperateSoul> mIdleState = new IdleState();
    static State<DesperateSoul> mMovingState = new MovingState();
    static State<DesperateSoul> mGlobalState = new GlobalState();

    protected override void SetupEnemy()
    {
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;

        mStateMachine = new StateMachine<DesperateSoul>(this, mIdleState, mGlobalState);
    }

    public void Update()
    {
        mStateMachine.Update();
    }

    public void FixedUpdate()
    {
        mStateMachine.FixedUpdate();
    }

    sealed class IdleState : State<DesperateSoul>
    {
        public void Update(DesperateSoul owner) { }

        public void FixedUpdate(DesperateSoul owner) { }

        public void OnEnter(DesperateSoul owner)
        {
            owner.StartCoroutine(IdleCoroutine(owner));
        }

        public void OnExit(DesperateSoul owner) { }

        IEnumerator IdleCoroutine(DesperateSoul owner)
        {
            float sleepTime = Random.Range(owner.mMinStillTime, owner.mMaxStillTime);
            yield return new WaitForSeconds(sleepTime);

            ChooseNewTarget(owner);
        }

        void ChooseNewTarget(DesperateSoul owner)
        {
            owner.mCurrentTarget = owner.mInitialPosition + Random.insideUnitCircle * owner.mMovementRadius;
            owner.mStateMachine.ChangeState(DesperateSoul.mMovingState);
        }
    }

    sealed class MovingState : State<DesperateSoul>
    {
        public void Update(DesperateSoul owner) { }

        public void FixedUpdate(DesperateSoul owner)
        {
            Debug.Log("Moving");
            Move(owner);
        }

        private static void Move(DesperateSoul owner)
        {
            Vector2 delta = owner.mCurrentTarget - new Vector2(owner.transform.position.x, owner.transform.position.y);
            if (Mathf.Abs(delta.sqrMagnitude) < 0.05f)
                owner.mStateMachine.ChangeState(DesperateSoul.mIdleState);
            else
                owner.mVelocity += delta.normalized * Time.fixedDeltaTime * owner.mAcceleration;
        }

        public void OnEnter(DesperateSoul owner) { }

        public void OnExit(DesperateSoul owner) { }
    }

    sealed class GlobalState : State<DesperateSoul>
    {
        public void FixedUpdate(DesperateSoul owner)
        {
            owner.mVelocity *= (1.0f - owner.mFriction); // * Time.fixedDeltaTime);
            owner.mRb.position += owner.mVelocity * Time.fixedDeltaTime;

            // owner.transform.localScale = new Vector3(owner.mRb.position.x >= owner.mPlayerRb.position.x ? -1 : 1, 1, 1);
        }

        public void OnEnter(DesperateSoul owner) { }

        public void OnExit(DesperateSoul owner) { }

        public void Update(DesperateSoul owner) { }
    }
}