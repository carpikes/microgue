using UnityEngine;
using System.Collections;
using System;

public class Siren : MonoBehaviour
{
    private GameObject mPlayer;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private float mSpeed = 0.5f;
    private float mAttractionSpeed = 5f;

    private Vector2 mInitialPosition;

    public States mState;
    public Animator mAnimator;

    InputManager mInputMgr;

    int mScreamingCount = 0;

    private float mRemainingTime = 0.0f;
    private float mAcceleration = 1.0f;

    private Vector2 mTargetPoint;

    private GameObject mCurrentTargetEnemy = null;

    WorldManager mWorldManager;

    private Vector2 noise;

    float maxDistance = 1f;

    int shotCount = 0;
    int shotLimit = 10;
    public GameObject darkBall;
    private float mShotPhase = 0.0f;

    public enum States
    {
        MOVING,
        SCREAMING,
    }

    // Use this for initialization
    void Start()
    {
        noise = new Vector2(UnityEngine.Random.Range(0.1f, 0.3f), UnityEngine.Random.Range(0.1f, 0.3f));
        mRb = GetComponent<Rigidbody2D>();
        mInitialPosition = transform.position;
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        mPlayerRb = mPlayer.GetComponent<Rigidbody2D>();

        mInputMgr = mPlayer.GetComponent<InputManager>();

        mState = States.MOVING;
        mAnimator = GetComponent<Animator>();

        mTargetPoint = mRb.position;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        mWorldManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();
        Debug.Assert(mWorldManager != null, "no world manager found");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if( mState == States.MOVING ) {
            noise = new Vector2(UnityEngine.Random.Range(0.1f, 0.3f), UnityEngine.Random.Range(0.1f, 0.3f));
            mRb.velocity = (mPlayerRb.position - mRb.position) * mSpeed;
        } else if (mState == States.SCREAMING) {
            if (mScreamingCount > 0)
            {
                if ( (mRb.position - mPlayerRb.position).magnitude >= maxDistance )
                    mPlayerRb.velocity = (mRb.position - mPlayerRb.position + noise).normalized * mAttractionSpeed;

                ++shotCount;
                if( shotCount >= shotLimit )
                {
                    Shot();
                    shotCount = 0;
                }
            }
        }
    }

    public void StartScreaming()
    {
        ++mScreamingCount;

        mAnimator.SetTrigger("attack");
        mState = States.SCREAMING;
    }

    internal void StopScreaming()
    {
        mScreamingCount--;

        if (mScreamingCount == 0) {
            mAnimator.SetTrigger("idle");
            mState = States.MOVING;
        }
    }

    void Shot()
    {
        int n = 3;

        float starting = UnityEngine.Random.Range(0, (float)(2f * Math.PI)) ;

        float phaseInc = 2 * Mathf.PI / n / 2.0f;
        for (int i = 0; i < n; i++)
        {
            float a = starting + 2 * Mathf.PI / n * i;
            GameObject lb = Instantiate(darkBall);
            lb.transform.position = transform.position;

            Vector2 direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            lb.GetComponent<Rigidbody2D>().velocity = direction * 2.0f;
            lb.GetComponent<ShotProperties>().SetDuration(UnityEngine.Random.Range(2.0f, 3.0f));
        }

        mShotPhase += phaseInc;
    }
}
