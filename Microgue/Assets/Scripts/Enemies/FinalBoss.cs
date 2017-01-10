using UnityEngine;
using System.Collections;

public class FinalBoss : MonoBehaviour {
    private int mState = 0;
    private float mTimeout, mAttackDelay;
    private Rigidbody2D mRB;
    public GameObject mPlayer;
    public GameObject mWorldContainer;

    // fra quanto iniziare lo spostamento. se messo a 0.0f == "non muoverti"
    private float mStartTimeout = 0.0f;
    private float mSpeed = 0.0f;
    private Vector2 mTarget, mLastTargetDelta;

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
        mWorldContainer = GameObject.Find("WorldData");
    }
	
	// Update is called once per frame
	void Update () {
        switch (mState)
        {
            case 0: // idle
                if (Time.time > mTimeout)
                    mState = Random.Range(1, 6);
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
            case 5: // entry point per l'attacco
                DoSomething(); // il nuovo stato (6 o 7) viene settato qua
                break;
            case 6: // attacchi
            case 7:
            case 8:
                if (Time.time > mTimeout)
                {
                    // MICHELE codice animazione idle
                    mState = 0;
                    mTimeout = Time.time + 2.0f;
                }
                if (mAttackDelay != 0.0f && Time.time >= mAttackDelay)
                {
                    if(mState == 6) OneHandAttack();
                    else if(mState == 7) TwoHandAttack();
                    else if(mState == 8) PowaAttack();
                }
                break;
        }	
	}

    void FixedUpdate() {
        if (mStartTimeout < Time.time && mStartTimeout != 0.0f)
        {
            Vector2 velocity = (mTarget - mRB.position).normalized * mSpeed;
            Vector2 newPos = mRB.position + velocity * Time.fixedDeltaTime;
            if (Vector2.Dot((newPos - mTarget).normalized, (mRB.position - mTarget).normalized) < -0.9f)
            {
                // target raggiunto (slice)
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
        Debug.Log("DoSomething()");
        EnemyLife life = GetComponent<EnemyLife>();
        float lifePerc = life.mCurrentHP / life.GetTotalHP();

        float animDelay = 0.5f; // MICHELE: qua il delay fra l'inizio dell'animazione e l'attacco vero
        mAttackDelay = Time.time + animDelay;
        mTimeout = mAttackDelay + Random.Range(1.5f, 2.0f);

        if (lifePerc < 0.3f)
        {
            // MICHELE: qua animazione attacco powa
            mState = 8;
        }
        else if (lifePerc < 0.6f)
        {
            // MICHELE: qua animazione attacco a due mani
            mState = 7;
        }
        else
        {
            // MICHELE: qua animazione attacco a una mano
            mState = 6;
        }
    }

    // Le tre funzioni *Attack() sono chiamate in ad ogni
    // update mentre devono essere eseguite.
    // I child di finalboss devono essere: HealthBar, SingleHand, OtherHand, LeftHandPowa, RightHandPowa

    private float mAttackTimeout = 0.0f; // usato come delay fra le varie instantiate
    public GameObject mDarkBall = null;

    [Header("Quella che esplode forte")]
    public GameObject mFinalBall = null;

    void OneHandAttack()
    {
        if (mAttackTimeout != 0.0f && Time.time < mAttackTimeout)
            return;

        Transform spawnPoint = transform.GetChild(1);
        Debug.Assert(spawnPoint.name.Equals("SingleHand"), "Ordine dei child errato!");

        int n = Random.Range(5, 8);
        Shot(spawnPoint, mDarkBall, n, false);

        mAttackTimeout = Time.time + 0.3f;
    }

    void TwoHandAttack()
    {
        if (mAttackTimeout != 0.0f && Time.time < mAttackTimeout)
            return;

        Transform spawnPoint = transform.GetChild(1);
        Transform spawnPoint2 = transform.GetChild(2);
        Debug.Assert(spawnPoint.name.Equals("SingleHand"), "Ordine dei child errato!");
        Debug.Assert(spawnPoint2.name.Equals("OtherHand"), "Ordine dei child errato!");

        int n = Random.Range(5, 8);
        Shot(spawnPoint, mDarkBall, n, false);
        Shot(spawnPoint2, mDarkBall, n, false);

        mAttackTimeout = Time.time + 0.3f;
    }

    void PowaAttack()
    {
        if (mAttackTimeout != 0.0f && Time.time < mAttackTimeout)
            return;

        Transform spawnPoint = transform.GetChild(3);
        Transform spawnPoint2 = transform.GetChild(4);
        Debug.Assert(spawnPoint.name.Equals("LeftHandPowa"), "Ordine dei child errato!");
        Debug.Assert(spawnPoint2.name.Equals("RightHandPowa"), "Ordine dei child errato!");

        int n = Random.Range(5, 8);
        Shot(spawnPoint, mFinalBall, n, false);
        Shot(spawnPoint2, mFinalBall, n, false);

        mAttackTimeout = Time.time + 0.3f;
    }

    void Shot(Transform startPoint, GameObject ball, int n, bool toPlayer)
    {
        float phase = Random.Range(0, Mathf.PI);
        for (int i = 0; i < n; i++)
        {
            float a = 2.0f * Mathf.PI / n * i + phase;
            GameObject lb = Instantiate(ball, mWorldContainer.transform) as GameObject;
            lb.transform.position = startPoint.position;

            Vector2 direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            lb.GetComponent<Rigidbody2D>().velocity = direction * 2.0f;
            lb.GetComponent<ShotProperties>().SetDuration(UnityEngine.Random.Range(2.0f, 3.0f));
        }
    }
}
