using UnityEngine;
using System.Collections;

public class WanderingFlame : MonoBehaviour {

    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Animator mAnimator;

    private float mRemainingTime = 0.0f;
    public float mSpeed = 1.0f;
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
        if (mRemainingTime < 0 || (mTargetPoint - mRb.position).sqrMagnitude <= 1f )
            ChooseNewPoint();

        mEnemyPosition.SetWorldPosition(mRb.position);
    }

    void ChooseNewPoint()
    {
        mTargetPoint = mPlayerRb.position + Random.insideUnitCircle * 5.0f;

        mRb.velocity = (mTargetPoint - mRb.position).normalized * mSpeed;

        mRemainingTime = 3f;
    }
}
