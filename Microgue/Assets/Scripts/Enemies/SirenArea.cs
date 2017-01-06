using UnityEngine;
using System.Collections;

public class SirenArea : MonoBehaviour {

    Siren mEnemy;
    Collider2D mTrigger;

    // Use this for initialization
    void Start () {
        mEnemy = (Siren)transform.parent.gameObject.GetComponent<Siren>();
        Debug.Assert(mEnemy != null, "Cannot find parent enemy for siren");

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
        if (other.CompareTag("Player"))
        {
            mEnemy.StartScreaming();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mEnemy.StopScreaming();
        }
    }
}
