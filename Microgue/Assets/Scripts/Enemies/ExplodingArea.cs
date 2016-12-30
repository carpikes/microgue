using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodingArea : MonoBehaviour {

    ExplodingStatue mEnemy;
    Collider2D mTrigger;
    Animator mAnimator;
    ExplodingStatue.States mState;

    void Start()
    {
        mEnemy = (ExplodingStatue)transform.parent.gameObject.GetComponent<ExplodingStatue>();
        Debug.Assert(mEnemy != null, "Cannot find parent enemy for exploding statue");

        mTrigger = GetComponent<Collider2D>();
        mTrigger.enabled = false;

        mAnimator = mEnemy.mAnimator;
        mState = mEnemy.mState;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.3f);
        mTrigger.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("DENTRO?");

        if (mAnimator == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (mState == ExplodingStatue.States.IDLE)
        {
            mState = ExplodingStatue.States.EXPLODING;
            mAnimator.SetTrigger("enemy_death");
        }

        mEnemy.mSubjectToExplosion = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        mEnemy.mSubjectToExplosion = false;
    }
}
