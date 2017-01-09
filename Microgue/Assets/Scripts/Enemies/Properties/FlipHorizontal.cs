using UnityEngine;
using System.Collections;

public class FlipHorizontal : MonoBehaviour {
    private Rigidbody2D mRB;
    private Transform mTransform;
    private Transform mBarTr;

    public bool mMirror = false;
    private float mInitialScale, mBarInitialScale;
	// Use this for initialization
	void Start () {
        mRB = GetComponent<Rigidbody2D>();
        mTransform = GetComponent<Transform>();
        mBarTr = mTransform.GetChild(0).transform;
        mInitialScale = mTransform.localScale.x * ((mMirror) ? -1.0f : 1.0f);
        Debug.Assert(mBarTr.gameObject.name.Equals("HealthBar"), "Child0 is not hp bar!! FlipHorizontal.cs is sad.");
        mBarInitialScale = mBarTr.localScale.x;
	}

	// Update is called once per frame
	void Update () {
        Vector2 scale = mTransform.localScale;
        Vector2 hpScale = mBarTr.localScale;
        float vx = mRB.velocity.x;
        scale.x = mInitialScale * ((vx > 0) ? 1.0f : -1.0f);
        hpScale.x = mBarInitialScale * ((vx > 0) ? 1.0f : -1.0f);
        mTransform.localScale = scale;
        mBarTr.localScale = hpScale;
	}
}
