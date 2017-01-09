using UnityEngine;
using System.Collections;

public class StompStompEnemy : MonoBehaviour
{
    private Transform mTransform;
    private Transform mBarTr;
    private float mInitialScale, mBarInitialScale;
	// Use this for initialization
	void Start () {
        mTransform = GetComponent<Transform>();
        mBarTr = mTransform.GetChild(0).transform;
        Debug.Assert(mBarTr.gameObject.name.Equals("HealthBar"), "Child0 is not hp bar!! StompStompEnemy.cs is sad.");
        mInitialScale = mTransform.localScale.x * -1.0f;
        mBarInitialScale = mBarTr.localScale.x;
	}

    float mOldPos = 0.0f;
	// Update is called once per frame
	void Update () {
        Vector2 scale = mTransform.localScale;
        Vector2 hpScale = mBarTr.localScale;
        float vx = transform.position.x - mOldPos;
        if (Mathf.Abs(vx) > 0.01)
        {
            scale.x = mInitialScale * ((vx > 0) ? 1.0f : -1.0f);
            hpScale.x = mBarInitialScale * ((vx > 0) ? 1.0f : -1.0f);
            mTransform.localScale = scale;
            mBarTr.localScale = hpScale;
            mOldPos = transform.position.x;
        }
	}

    void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Stomp hit " + other.name + " " + Time.time + ": " + gameObject.transform.position);
    }
}