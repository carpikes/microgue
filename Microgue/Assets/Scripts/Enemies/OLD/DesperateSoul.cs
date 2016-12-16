using UnityEngine;
using System.Collections;

public class DesperateSoulOLD : MonoBehaviour
{
    public float mMovementRadius = 3.0f;
    public float mMinStillTime = 8.0f;
    public float mMaxStillTime = 10.0f;
    public float mAcceleration = 0.5f;
    public float mFriction = 0.01f;

    private Rigidbody2D mRb;
    private Rigidbody2D mPlayerRb;
    private Vector2 mInitialPosition;
	private EnemyPosition mEnemyAI;
    private EnemyLife mEnemyLife;

    private Vector2 mVelocity = Vector2.zero;
    private Vector3 mCurTarget;
    private enum EnemyStates
    {
        STILL,
        MOVING,
    };
    private EnemyStates mCurState;
    // Use this for initialization
    void Start()
    {
        mPlayerRb = GameObject.Find("MainCharacter").GetComponent<Rigidbody2D>();
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
		mEnemyAI = GetComponent<EnemyPosition>();
        mEnemyLife = GetComponent<EnemyLife>();
        mEnemyAI.SetEnabled(false);
        ChooseNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void FixedUpdate()
    {
        Vector2 delta = mCurTarget - transform.position;

        if (mCurState == EnemyStates.MOVING)
        {
            if (Mathf.Abs(delta.sqrMagnitude) < 0.05f)
            {
                mCurState = EnemyStates.STILL;
                StartCoroutine(stillCoroutine());
            }
            else
                mVelocity += delta.normalized * Time.fixedDeltaTime * mAcceleration;
        }

        mVelocity *= (1.0f - mFriction); // * Time.fixedDeltaTime);
        mRb.position += mVelocity * Time.fixedDeltaTime;

        transform.localScale = new Vector3(mRb.position.x >= mPlayerRb.position.x ? -1 : 1, 1, 1);
    }

    IEnumerator stillCoroutine()
    {
        float sleepTime = Random.Range(mMinStillTime, mMaxStillTime);
        yield return new WaitForSeconds(sleepTime);
        ChooseNewTarget(); 
    }

    void ChooseNewTarget()
    {
        mCurTarget = mInitialPosition + Random.insideUnitCircle * mMovementRadius;
        mCurState = EnemyStates.MOVING;
    }
}
