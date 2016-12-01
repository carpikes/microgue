using UnityEngine;
using System.Collections;
using DG.Tweening;

public class StompStomp : MonoBehaviour {

    Rigidbody2D rb;
    Collider2D coll;
    Transform playerTransform;

    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //StartCoroutine("JumpCoroutine");
	}

    IEnumerator JumpCoroutine()
    {
        Sequence seq;

        while (true)
        {
            seq = rb.DOJump(playerTransform.position, 2f, 1, 1, false);
            yield return seq.WaitForStart();
            Debug.Log("Jump started");
            coll.enabled = false;
            sr.color = Color.magenta;
            yield return seq.WaitForCompletion();
            coll.enabled = true;
            sr.color = Color.white;

            // This log will happen after the tween has completed
            Debug.Log("Jump completed");
            yield return new WaitForSeconds(1);
        }

    }

    float curTime = 0;
    private Vector2 mVelocity = new Vector2(0,0);
    private Vector2 mCurTarget = new Vector2(0, 0);

    public float mGravity = 9.80665f;
    public float mJumpAccel = 5.0f;

    void BeginJumpToTarget(Vector2 newTarget)
    {
        Vector2 pos = rb.transform.position;
        Vector2 ds = newTarget - pos;

        float angle = Mathf.PI / 4.0f;
        float Vx = mJumpAccel * Mathf.Cos(angle);
        float Vy = mJumpAccel * Mathf.Sin(angle);
        float jumpTime = 2 * (Vy / mGravity + Mathf.Sqrt(Vy * Vy + 2 * mGravity * Mathf.Abs(ds.y))) / mGravity;
        float jumpX = Vx * jumpTime;
        
        // salto troppo, riduco l'angolo
        if (ds.x * ds.x < jumpX * jumpX)
        { 
            // dato che jumpTime dipende dall'angolo e l'angolo da jumpTime, itero 5 volte 
            // (10 volte converge alla 4^ cifra decimale, ma non serve cosi` tanta precisione)
            for (int i = 0; i < 5; i++)
            {
                angle = Mathf.Acos(Mathf.Abs(ds.x) / (mJumpAccel * jumpTime));
                Vy = mJumpAccel * Mathf.Sin(angle);
                jumpTime = 2 * (Vy / mGravity + Mathf.Sqrt(Vy * Vy + 2 * mGravity * Mathf.Abs(ds.y))) / mGravity;
            }
        }

        mVelocity.x = mJumpAccel * Mathf.Cos(angle) * Mathf.Sign(ds.x);
        mVelocity.y = mJumpAccel * Mathf.Sin(angle);
        mCurTarget = newTarget;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (curTime == 0.0f)
            BeginJumpToTarget(playerTransform.position);

        mVelocity.y -= mGravity * Time.fixedDeltaTime;

        Vector3 newPosition = rb.transform.position;
        newPosition.x += mVelocity.x * Time.fixedDeltaTime;
        newPosition.y += mVelocity.y * Time.fixedDeltaTime;

        curTime += Time.fixedDeltaTime;
        if (newPosition.y <= mCurTarget.y && mVelocity.y < 0)
        {
            mVelocity = Vector2.zero;
            curTime = 0.0f;
        }
        
        rb.transform.position = newPosition;

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Stomp hit " + other.name + " " + Time.time + ": " + gameObject.transform.position);
    }

}
