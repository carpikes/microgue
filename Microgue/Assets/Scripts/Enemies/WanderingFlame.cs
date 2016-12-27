using UnityEngine;
using System.Collections;

public class WanderingFlame : MonoBehaviour {

    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;

    private Vector2 mVelocity = Vector2.zero;
    private float mRemainingTime = 0.0f;
    float mAcceleration = 50.0f;
    private Vector2 mTargetPoint;

    private EnemyPosition mEnemyPosition;

    // Use this for initialization
    void Start () {
        mRb = GetComponent<Rigidbody2D>();
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        ChooseNewPoint();
    }

    void FixedUpdate()
    {
        Vector2 delta = mTargetPoint;

        mRemainingTime -= Time.fixedDeltaTime;
        if (mRemainingTime < 0)
            ChooseNewPoint();

        if (delta.magnitude > 0.3f)
        {
            mVelocity = delta.normalized * Time.fixedDeltaTime * mAcceleration;
        }

        mRb.position += mVelocity * Time.fixedDeltaTime;
        mEnemyPosition.SetWorldPosition(mRb.position);
    }
    void ChooseNewPoint()
    {
        float w = AIMap.GetWidth();
        float h = AIMap.GetHeight();

        mTargetPoint = new Vector2(Random.Range(-w/2, w/2), Random.Range(-h/2, h/2));

        mRemainingTime = Random.Range(0.1f, 0.3f);
    }
}
