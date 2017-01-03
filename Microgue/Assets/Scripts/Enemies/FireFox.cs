using UnityEngine;
using System.Collections;

public class FireFox : MonoBehaviour {

    enum FirefoxStates
    {
        IDLE,
        ROLLING,
        SHOOTING
    }

    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private AIMap mAIMap;

    private EnemyPosition mEnemyPosition;

    private Animator mAnimator;

    // STAGES
    // 1 - idle/rolling
    // 2 - idle/shooting/rolling
    // 3 - idle/shooting/rolling
    FirefoxStates state;
    int stage = 1;

    int[] rollingSpeeds = new int[] { 10, 20, 30 };
    int[] shootingSpeeds = new int[] { 5, 10 };

    Vector2[] targetPoints = new Vector2[]
    {
        new Vector2(3,-2), // up left
        new Vector2(7,-2), // up right
        new Vector2(3,-5), // bottom left
        new Vector2(7,-5), // bottom right
    };

    // State machines
    StateMachine<FireFox> mStateMachine;
    static State<FireFox> mIdleState = new IdleState();
    static State<FireFox> mChoosingDirState = new ChoosingDirState();
    static State<FireFox> mRollingState = new RollingState();
    static State<FireFox> mShootingState = new ShootingState();

    // Use this for initialization
    void Start () {
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        mAnimator = GetComponent<Animator>();

        mStateMachine = new StateMachine<FireFox>(this, mIdleState, null);
    }
	
	// Update is called once per frame
	void Update () {
        mStateMachine.Update();
	}

    void FixedUpdate()
    {
        mStateMachine.FixedUpdate();
    }

    sealed class IdleState : State<FireFox>
    {
        public int limit = 60;
        int cnt = 0;

        public void Update(FireFox owner) {
            ++cnt;

            if( cnt >= limit )
            {
                cnt = 0;
                owner.mStateMachine.ChangeState(FireFox.mChoosingDirState);
            }
        }

        public void FixedUpdate(FireFox owner) {

        }

        public void OnEnter(FireFox owner)
        {
            owner.mAnimator.SetTrigger("idle");
        }

        public void OnExit(FireFox owner) { }
    }

    sealed class ChoosingDirState : State<FireFox>
    {
        public float mSpeed = 2.0f;

        public void Update(FireFox owner)
        {
            
        }

        public void FixedUpdate(FireFox owner)
        {

        }

        public void OnEnter(FireFox owner)
        {
            owner.mAnimator.SetTrigger("idle");
            Rigidbody2D rb = owner.mRb;

            Vector2 targetPos = new Vector2(Random.Range(3, 7), Random.Range(-5, -2));
            rb.velocity = (rb.position - targetPos).normalized * mSpeed;

            owner.mStateMachine.ChangeState(FireFox.mRollingState);
        }

        public void OnExit(FireFox owner) { }
    }

    sealed class RollingState : State<FireFox>
    {
        Rigidbody2D rb;
        public int limit = 60;
        int cnt = 0;

        public void Update(FireFox owner) {
            ++cnt;

            if (cnt >= limit)
            {
                cnt = 0;
                owner.mStateMachine.ChangeState(FireFox.mIdleState);
            }
        }

        public void FixedUpdate(FireFox owner)
        {
           
        }

        public void OnEnter(FireFox owner) {
            rb = owner.mRb;
            owner.mAnimator.SetTrigger("rolling");
        }

        public void OnExit(FireFox owner) { }
    }

    sealed class ShootingState : State<FireFox>
    {
        public void Update(FireFox owner) { }

        public void FixedUpdate(FireFox owner)
        {

        }

        public void OnEnter(FireFox owner)
        {
            
        }

        public void OnExit(FireFox owner) { }
    }

}
