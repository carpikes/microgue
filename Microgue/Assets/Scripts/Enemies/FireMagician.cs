using UnityEngine;
using System.Collections;

public class FireMagician : MonoBehaviour {
    AIMap mAIMap;
    bool[,] mMap = null;
    public GameObject darkBall;

    public int mShots = 8;

    public float mShotCooldownMin = 3.0f;
    public float mShotCooldownMax = 4.0f;

    public float mHiddenTimeMin = 2.0f;
    public float mHiddenTimeMax = 4.0f;

    public float mShotTimeMin = 3.0f;
    public float mShotTimeMax = 6.0f;

    private float mNextShot = 0.0f;
    private float mChangeTime = 0.0f;
    private bool mJumpAwayInstant = false;
    private float mShotPhase = 0.0f;

    Transform mPlayer;
    int mState = 1;

    Animator mAnimator;

	// Use this for initialization
	void Start () {
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
        mPlayer = GameObject.Find("MainCharacter").transform;
        mJumpAwayInstant = false;

        mAnimator = GetComponent<Animator>();

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        mAnimator.SetTrigger("teleport_down");
        mChangeTime = Time.time + Random.Range(mHiddenTimeMin, mHiddenTimeMax);
	}

	// Update is called once per frame
	void Update () {
        if (mMap == null)
        {
            if (mAIMap.IsMapReady)
                mMap = mAIMap.GetMap();
        }

        if (mMap != null)
            UpdateAI();
	}

    void UpdateAI() {
        switch (mState) {
            case 0: // spara
                // se non e` il momento di saltare, spara
                if (mNextShot < Time.time)
                {
                    Shot();
                    mNextShot = Time.time + Random.Range(mShotCooldownMin, mShotCooldownMax);
                }

                if (mChangeTime < Time.time || mJumpAwayInstant) // ok nasconditi!
                {
                    mState = 1;
                    mAnimator.SetTrigger("teleport_down");
                    mChangeTime = Time.time + Random.Range(mHiddenTimeMin, mHiddenTimeMax); // stai questo tempo nascosto
                    GetComponent<BoxCollider2D>().enabled = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                }
                break;
            case 1: // stai nascosto
                    // questi 3 comandi dovrebbero essere chiamati una sola volta,
                    // ma a quanto pare qualche altro script cambia il colore o simili
                    // e se non li setto in loop non funziona bene. lol
                if (mChangeTime < Time.time) // time out, riappari altrove!
                {
                    mState = 2;
                    mChangeTime = Time.time + Random.Range(0.6f, 0.8f);  // e stai questo tempo fermo appena riapparso
                    JumpAway();
                    // e riappari
                    GetComponent<SpriteRenderer>().enabled = true;
                    GetComponent<BoxCollider2D>().enabled = true;
                    mAnimator.SetTrigger("teleport_up");
                }
                break;
            case 2: // appena riapparso... 
                if (mChangeTime < Time.time) // ok hai capito il tuo ruolo nel mondo, spara!
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    mChangeTime = Time.time + Random.Range(mShotTimeMin, mShotTimeMax); // stai questo tempo fermo (da attivo)
                    mJumpAwayInstant = false;
                    mState = 0;
                    mAnimator.SetTrigger("idle");
                }
                break;
        }
    }

    void JumpAway() {
        Vector2 point = Vector2.zero;
        // il for cerca un punto decente dove saltare
        for (int i = 0; i < 10; i++)
        {
            Vector2 coords = mPlayer.position;
            Vector2 p2 = Random.onUnitSphere * 3.0f;
            point = coords + p2;
            AIMap.IntPoint p = AIMap.WorldToTileCoordinates(point);
            if (mMap[p.x, p.y] == false)
                break;
        }
        // salta li`
        transform.position = point;
    }

    // se toccato, 1 volta su 3 salta via subito
    void OnTriggerEnter2D(Collider2D other) {
        if (mState != 0) return; // utile solo se sta sparando
        if (other.CompareTag("Shot"))
            if (Random.Range(0, 3) == 0)
                mJumpAwayInstant = true;
    }

    void Shot() {
        int n = mShots;
        float phaseInc = 2 * Mathf.PI / n / 2.0f;
        for (int i = 0; i < n; i++)
        {
            float a = 2 * Mathf.PI / n * i + mShotPhase;
            GameObject lb = Instantiate(darkBall);
            lb.transform.position = transform.position;

            Vector2 direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            lb.GetComponent<Rigidbody2D>().velocity = direction * 2.0f;
            lb.GetComponent<ShotProperties>().SetDuration(Random.Range(2.0f, 3.0f));
        }
        mShotPhase += phaseInc;
    }
}
