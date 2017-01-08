using UnityEngine;
using System.Collections;

public class FinalBoss : MonoBehaviour {
    private int mState = 0;
    private float mTimeout;
    private Rigidbody2D mRB;
    private GameObject mPlayer;

    private enum Animation {
        SLICE_LEFT,
        SLICE_RIGHT,
        SLICE_DOWN,
        IDLE,
    };

    private Vector2[] mArea = {
        new Vector2(0.7f,  -2.0f), // UL
        new Vector2(8.88f, -2.0f), // UR
        new Vector2(8.88f, -7.1f), // DR
        new Vector2(0.7f,  -7.1f), // DL
    };

	// Use this for initialization
	void Start () {
        mTimeout = Time.time + 3;
        mRB = GetComponent<Rigidbody2D>();
        mPlayer = GameObject.Find("MainCharacter");
	}
	
	// Update is called once per frame
	void Update () {
        switch (mState)
        {
            case 0: // idle
                if (Time.time > mTimeout)
                    mState = Random.Range(1, 4);
                break;
            case 1: // run sx/dx
                {
                    float x = mRB.position.x;
                    if (mRB.position.x < mArea[0].x + 0.1f) x = mArea[1].x;
                    else if (mRB.position.x > mArea[1].x - 0.1f) x = mArea[0].x;
                    else if (mPlayer.transform.position.x - mRB.position.x > 0) x = mArea[1].x;
                    else x = mArea[0].x;
                    Vector2 newPos = new Vector2(x, mRB.position.y);
                    GoTo(newPos, 0.8f, 10.0f, (x == mArea[0].x) ? Animation.SLICE_LEFT : Animation.SLICE_RIGHT);

                    mState = 0; // wait mTimeout
                    mTimeout = Time.time + 3.0f;
                }
                break;
            case 2: // run down
                float y = mRB.position.y;
                if (y > mArea[2].y + 0.5) // slide down (la y e` invertita!)
                {
                    Vector2 newPos = new Vector2(mRB.position.x, mArea[2].y);
                    GoTo(newPos, 0.8f, 10.0f, Animation.SLICE_DOWN);
                    mState = 0;
                    mTimeout = Time.time + 3.0f;
                }
                else
                    mState = 3;
                
                break;
            case 3: // goto player
            case 4:
                {
                    Vector2 newPos = mPlayer.transform.position;
                    newPos += Random.insideUnitCircle * 2.0f;
                    GoTo(newPos, 0.8f, 3f, Animation.IDLE);
                    mState = 0;
                    mTimeout = Time.time + 3.0f;
                }
                break;
            case 5: // spara con una mano
                DoSomething();
                mState = 0;
                mTimeout = Time.time + 2.0f;
                break;
        }	
	}

    // fra quanto iniziare lo spostamento. se messo a 0.0f == "non muoverti"
    private float mStartTimeout = 0.0f;
    private float mSpeed = 0.0f;
    private Vector2 mTarget, mLastTargetDelta;

    void FixedUpdate() {
        if (mStartTimeout < Time.time && mStartTimeout != 0.0f)
        {
            Vector2 velocity = (mTarget - mRB.position).normalized * mSpeed;
            Vector2 newPos = mRB.position + velocity * Time.fixedDeltaTime;
            if (Vector2.Dot((newPos - mTarget).normalized, (mRB.position - mTarget).normalized) < -0.9f)
            {
                // target raggiunto
                // MICHELE: qua va il codice di cambio animazione in idle
                mRB.position = mTarget;
                mSpeed = 0.0f;
                mStartTimeout = 0.0f;
            }
            else
                mRB.position = newPos;
        }
    }

    void GoTo(Vector2 coords, float delay, float speed, Animation animId)
    {
        //MICHELE: switch (animId) { .... } per fare animazioni
        Debug.Log("Goto: " + coords + ", " + delay + "," + animId.ToString());
        mStartTimeout = Time.time + delay;
        mSpeed = speed;
        mTarget = coords;
    }

    void DoSomething()
    {
        // TODO
    }
}
