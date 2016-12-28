using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealingArea : MonoBehaviour {

    HealingAngel mEnemy;
    Collider2D mTrigger;

    void Start()
    {
        mEnemy = (HealingAngel)transform.parent.gameObject.GetComponent<HealingAngel>();
        Debug.Assert(mEnemy != null, "Cannot find parent enemy for healing angel");

        mTrigger = GetComponent<Collider2D>();
        mTrigger.enabled = false;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.3f);
        mTrigger.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && mEnemy != null && other.gameObject != transform.parent.gameObject )
        {
            other.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            other.gameObject.GetComponent<EnemyLife>().mIsInvincible = true;
            mEnemy.mProtectedEnemies.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && mEnemy != null)
        {
            ResetEnemyToNormal(other.gameObject);
            mEnemy.mProtectedEnemies.Remove(other.gameObject);
        }
    }

    private void ResetEnemyToNormal(GameObject go)
    {
        if (go == null)
            return;

        go.GetComponent<SpriteRenderer>().color = Color.white;
        go.GetComponent<EnemyLife>().mIsInvincible = false;
    }

    void OnDestroy()
    {
        foreach (var go in mEnemy.mProtectedEnemies)
            ResetEnemyToNormal(go);

        mEnemy.mProtectedEnemies.Clear();
    }
}
