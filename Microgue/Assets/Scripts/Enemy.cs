using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float mMovementRadius = 100.0f;
    public float mMinStillTime = 5.0f;
    public float mMaxStillTime = 20.0f;
    public float mAcceleration = 0.5f;
    public float mFriction = 0.01f;

    private Rigidbody2D mRb;
    private Vector2 mInitialPosition;

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
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
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

        mVelocity *= 1.0f - (mFriction * Time.fixedDeltaTime);
        mRb.position += mVelocity * Time.fixedDeltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OUCH");
    }

    IEnumerator stillCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(mMinStillTime, mMaxStillTime));
        ChooseNewTarget(); 
    }

    void ChooseNewTarget()
    {
        mCurTarget = mInitialPosition + Random.insideUnitCircle * mMovementRadius;
        mCurState = EnemyStates.MOVING;
    }
}
