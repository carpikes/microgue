using UnityEngine;
using System.Collections;

public class FireMagician : MonoBehaviour {
    AIMap mAIMap;
    bool[,] mMap = null;
    public GameObject darkBall;

    private float mNextShot = 0.0f;
    private float mChangeTime = 0.0f;
    private bool mJumpAwayInstant = false;

    Transform mPlayer;
    int mState = 0;
	// Use this for initialization
	void Start () {
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
        mPlayer = GameObject.Find("MainCharacter").transform;
        mChangeTime = Time.time + Random.Range(3.0f, 6.0f);
        mJumpAwayInstant = false;
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
                    mNextShot = Time.time + Random.Range(0.3f, 0.5f);
                }

                if (mChangeTime < Time.time || mJumpAwayInstant) // ok nasconditi!
                {
                    mState = 1;
                    mChangeTime = Time.time + Random.Range(3.0f, 4.0f); // stai questo tempo nascosto
                }
                break;
            case 1: // stai nascosto
                // questi 3 comandi dovrebbero essere chiamati una sola volta,
                // ma a quanto pare qualche altro script cambia il colore o simili
                // e se non li setto in loop non funziona bene. lol
                GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
                if (mChangeTime < Time.time) // time out, riappari altrove!
                {
                    mState = 2;
                    mChangeTime = Time.time + Random.Range(0.7f, 1.0f);  // e stai questo tempo fermo appena riapparso
                    JumpAway();
                    // e riappari
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    GetComponent<BoxCollider2D>().enabled = true;
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                break;
            case 2: // appena riapparso... 
                if (mChangeTime < Time.time) // ok hai capito il tuo ruolo nel mondo, spara!
                {
                    mChangeTime = Time.time + Random.Range(3.0f, 6.0f); // stai questo tempo fermo (da attivo)
                    mJumpAwayInstant = false;
                    mState = 0;
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
        GameObject lb = Instantiate(darkBall);
        lb.transform.position = transform.position;

        Vector2 direction = (mPlayer.transform.position - transform.position).normalized;
        (lb.GetComponent<Rigidbody2D>()).velocity = direction * 5.0f;
    }
}
