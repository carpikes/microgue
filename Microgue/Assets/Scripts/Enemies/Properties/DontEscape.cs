using UnityEngine;
using System.Collections;

public class DontEscape : MonoBehaviour {
    private Rigidbody2D mRB;
    private WorldManager mWorld = null; 
    private Vector3[] mBounds = null;
    private Vector2 mCenter = Vector2.zero;

	// Use this for initialization
	void Start () {
        mRB = GetComponent<Rigidbody2D>();
        Debug.Assert(mRB != null, "RigidBody can't be null");
	}

    // Update is called once per frame
    void Update()
    {
        if (mBounds == null)
        {
            GameObject gpMan = GameObject.Find("GameplayManager");
            if (gpMan == null) return;
            GameplayManager gpObj = gpMan.GetComponent<GameplayManager>();
            if (gpObj == null) return;
            mWorld = gpObj.GetWorldManager();
            if (mWorld == null) return;
            mBounds = mWorld.GetCameraBounds().Clone() as Vector3[];
            if (mBounds == null) return;
            // compenso gli offset della telecamera
            float ratio = Camera.main.aspect * 2.0f;
            mBounds[0].x -= ratio; mBounds[0].y -= 2;
            mBounds[1].x += ratio; mBounds[1].y += 2 - 0.2f;
            mCenter = new Vector2((mBounds[0].x + mBounds[1].x) / 2.0f, (mBounds[0].y + mBounds[1].y) / 2.0f);
        }
    }

    public bool IsOutOfBound(Vector2 pos)
    {
        if (pos == null) return true;
        if (mBounds == null) return false;
        if (mBounds.Length != 2 || mBounds[0] == null || mBounds[1] == null) return false;
        if (pos.x < mBounds[0].x || pos.x > mBounds[1].x || pos.y < mBounds[0].y || pos.y > mBounds[1].y)
            return true;

        return false;
    }

    void FixedUpdate()
    {
        if (mBounds != null)
        {
            Vector2 pos = mRB.position;
            if(IsOutOfBound(pos))
            {
                Vector2 dir = (mCenter - pos).normalized;
                mRB.velocity += dir * 3.0f * Time.fixedDeltaTime;
            }
        }
        mRB.velocity *= 0.99f;
	}
}
