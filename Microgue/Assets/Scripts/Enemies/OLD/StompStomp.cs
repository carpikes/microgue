using UnityEngine;
using System.Collections;

public class StompStomp : MonoBehaviour
{

    enum EnemyStatus
    {
        WAITING,
        FALLING,
        STILL,
        JUMPING,
    };

    private EnemyStatus mStatus;
    private Vector2 mVelocity = Vector2.zero;
    private Vector2 mCurTarget = Vector2.zero;
    private Vector2 mRenderingOffset = Vector2.zero;

    public float mGravity = 9.80665f;
    public float mJumpAccel = 3.0f;
    public float mMinWait = 0.1f;
    public float mMaxWait = 0.5f;
    public float mJumpSize = 1.0f;

    private Rigidbody2D mRigidBody;
    private Transform mShadowTransform;
    private Collider2D mCollider;
    private Transform mPlayerTransform;
    private InputManager mInputManager;
	private EnemyPosition mEnemyAI;

    // Used for shadow projection
    private Vector2 mMovingDirection, mJumpStartPosition, mShadowOffset;
    private bool mDontMoveShadow;

	// Use this for initialization
	void Start () 
	{
        mRigidBody = transform.GetChild(0).GetComponent<Rigidbody2D>();
        mCollider = transform.GetChild(0).GetComponent<Collider2D>();
        mPlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mShadowTransform = transform.GetChild(1).transform;
        mShadowOffset = mShadowTransform.localPosition;

        mRenderingOffset = mRigidBody.transform.localPosition;

        mJumpStartPosition = mRigidBody.transform.position;
        mJumpStartPosition -= mRenderingOffset;

        mMovingDirection = new Vector2(0, 0);
        mStatus = EnemyStatus.WAITING;
        mInputManager = GameObject.Find("MainCharacter").GetComponent<InputManager>();

        Vector2 pos = new Vector2(0.0f, 10.0f);
        mRigidBody.position = mRigidBody.position + pos + mRenderingOffset;
        transform.GetChild(0).GetComponent<Transform>().position = mRigidBody.position + pos + mRenderingOffset;
		mEnemyAI = GetComponent<EnemyPosition>();
		mEnemyAI.SetEnabled(false);
	}

    IEnumerator JumpCoroutine()
    {
        float waitTime = Random.Range(mMinWait, mMaxWait);
        yield return new WaitForSeconds(waitTime);
        mCollider.enabled = false;
        BeginJumpToTarget();
    }

    void BeginJumpToTarget()
    {
        Vector2 newTarget = mPlayerTransform.position;
        Vector2 pos = mRigidBody.transform.position;
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
        mJumpStartPosition = mRigidBody.position - mRenderingOffset;

        mDontMoveShadow = ((mJumpStartPosition - mCurTarget).magnitude < 0.1);
        mStatus = EnemyStatus.JUMPING;
    }   

    // Update is called once per frame
    void FixedUpdate ()
	{
        if (mStatus != EnemyStatus.JUMPING && mStatus != EnemyStatus.FALLING)
            return;

        mVelocity.y -= mGravity * Time.fixedDeltaTime;

        Vector2 newPosition = mRigidBody.transform.position;
        newPosition -= mRenderingOffset;
        newPosition.x += mVelocity.x * Time.fixedDeltaTime;
        newPosition.y += mVelocity.y * Time.fixedDeltaTime;

        if (newPosition.y <= mCurTarget.y && mVelocity.y < 0.05)
        {
            if (mStatus == EnemyStatus.FALLING)
            {
                newPosition.y = mCurTarget.y;
                mInputManager.ShakeCamera(0.15f, 2.0f);
            } else 
                mInputManager.ShakeCamera(0.13f, 1.0f);

            mVelocity = Vector2.zero;
            mStatus = EnemyStatus.STILL;
            mCollider.enabled = true;
            StartCoroutine(JumpCoroutine());
        }

        if (mStatus == EnemyStatus.JUMPING || mStatus == EnemyStatus.STILL)
        {
            // Shadow Projection
            float dot = Vector2.Dot(newPosition - mJumpStartPosition, mMovingDirection);
            if(!mDontMoveShadow)
                mShadowTransform.position = mJumpStartPosition + dot * mMovingDirection + mShadowOffset;
        }

		if(mEnemyAI != null)
			mEnemyAI.SetPosition (mShadowTransform.position);
        mRigidBody.transform.position = newPosition + mRenderingOffset;
	}

    public void OnShadowTouch() {
        if (mStatus == EnemyStatus.WAITING)
        {
			if (mEnemyAI != null)
			{
				mEnemyAI.SetEnabled (true);
				mEnemyAI.SetPosition (mShadowTransform.position);
			}
            mStatus = EnemyStatus.FALLING;
            mCurTarget = mShadowTransform.position;
            mCurTarget -= mShadowOffset;
            //Debug.Log("Changed status to Falling");
        }
    }
}
