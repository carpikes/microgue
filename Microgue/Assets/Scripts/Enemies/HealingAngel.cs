using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class HealingAngel : MonoBehaviour {
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    private List<GameObject> mProtectedEnemies;
    private GameObject mCurrentTargetEnemy = null;
    WorldManager mWorldManager;

    private Vector2 mVelocity = Vector2.zero;
    private float mRemainingTime = 0.0f;
    private float mAcceleration = 2.0f;
    private Vector2 mTargetPoint;

    private EnemyPosition mEnemyPosition;

    void Start()
    {
        mProtectedEnemies = new List<GameObject>();
        mRb = GetComponent<Rigidbody2D>();
        mPlayerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        mEnemyPosition = GetComponent<EnemyPosition>();
        mEnemyPosition.SetEnabled(true);

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        mWorldManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().GetWorldManager();
        Debug.Assert(mWorldManager != null, "no world manager found");

        mCurrentTargetEnemy = ChooseNewEnemy();
        ChooseNewPoint();
    }

    private GameObject ChooseNewEnemy()
    {
        if (mWorldManager.AreAllEnemiesKilled())
            return null;

        return mWorldManager.RandomEnemy();
    }

    void FixedUpdate()
    {
        // wait initialization of world manager first!
        if (mCurrentTargetEnemy == null)
            return;


        Vector2 delta = new Vector2(mCurrentTargetEnemy.transform.position.x, mCurrentTargetEnemy.transform.position.y)
            + mTargetPoint - new Vector2(mRb.position.x, mRb.position.y);

        mRemainingTime -= Time.fixedDeltaTime;
        if (mRemainingTime < 0)
            ChooseNewPoint();

        if (delta.magnitude > 0.3f)
        {
            mVelocity += delta.normalized * Time.fixedDeltaTime * mAcceleration;
            if (delta.magnitude > 0.6f)
                mVelocity.x *= 0.99f;
            mVelocity.y *= 0.95f;
        } 

        mRb.position += mVelocity * Time.fixedDeltaTime;
        mEnemyPosition.SetWorldPosition(mRb.position);
    }

    void ChooseNewPoint()
    {
        mTargetPoint = Random.onUnitSphere;
        mTargetPoint.y /= 3.0f;
        mRemainingTime = Random.Range(0.1f, 0.3f);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("ADDED");
            other.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            other.gameObject.GetComponent<EnemyLife>().mIsInvincible = true;
            mProtectedEnemies.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("REMOVED");
            other.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            other.gameObject.GetComponent<EnemyLife>().mIsInvincible = false;
            mProtectedEnemies.Remove(other.gameObject);
        }
    }

}
