using UnityEngine;
using System.Collections;

public class WanderingFlame : MonoBehaviour {

    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Animator mAnimator;

    private Vector2 mVelocity = Vector2.zero;
    private float mRemainingTime = 0.0f;
    public float mSpeed = 1f;
    private Vector2 mTargetPoint;

    private EnemyPosition mEnemyPosition;

    // Use this for initialization
    void Start () {
        mRb = GetComponent<Rigidbody2D>();
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        mAnimator = GetComponent<Animator>();
        mAnimator.speed = Random.Range(0.9f, 1.1f);

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        ChooseNewPoint();
    }

    void FixedUpdate()
    {
        mRemainingTime -= Time.fixedDeltaTime;
        if (mRemainingTime < 0 || (mTargetPoint - mRb.position).sqrMagnitude <= .01f )
            ChooseNewPoint();

        mRb.position += mVelocity * Time.fixedDeltaTime;
        mEnemyPosition.SetWorldPosition(mRb.position);
    }

    void ChooseNewPoint()
    {
        float w = AIMap.GetWidth();
        float h = AIMap.GetHeight();

        mTargetPoint = new Vector2(Random.Range(0, w), -Random.Range(0, h));
        mVelocity = (mTargetPoint - mRb.position).normalized * mSpeed * Time.deltaTime;

        //Debug.Log(mTargetPoint);

        mRemainingTime = 3f;
    }
}
