using UnityEngine;
using System.Collections;

public class Medusa : MonoBehaviour
{
    // stato della fsm
    private int mState;

    // corro o freezo?
    private int mSubAI = 0;

    [Header("Speed")]
    public float mSpeed = 8.0f;

    [Header("Waiting+Freezing Time")]
    // tempo fermo e che freeza il giocatore
    public float mWaitingTimeMin = 2.5f;
    public float mWaitingTimeMax = 3.0f;

    [Header("Running-like-hell Time")]
    // tempo in cui corre
    public float mRunTimeMin = 5.0f;
    public float mRunTimeMax = 6.0f;

    [Header("Chosing Time (wait or run?)")]
    // tempo in cui decide cosa fare
    public float mChosingTimeMin = 0.7f;
    public float mChosingTimeMax = 0.9f;
    
    [Header("Initial Waiting Time")]
    // secondi di attesa iniziale
    public float mInitialWaitingTime = 5.0f;

    // raggio della spell che freeza
    [Header("Radius of the freeze")]
    public float mSpellRadius = 1.5f;

    [Header("Freeze time")]
    // secondi in cui il giocatore e` freezato
    public float mFreezeSeconds = 3.0f;

    private float mTimeout;
    private GameObject mPlayer;
    private Rigidbody2D mRB;
    private GameObject mSpellAnim;

    private Animator mAnimator;

	// Use this for initialization
	void Start () {
        mTimeout = Time.time + mInitialWaitingTime;
        mState = 0;
        mPlayer = GameObject.Find("MainCharacter");
        mAnimator = mPlayer.GetComponent<Animator>();
        mRB = GetComponent<Rigidbody2D>();
        mSpellAnim = transform.GetChild(0).gameObject;
        // MICHELE: qua anim di idle
	}
	
	// Update is called once per frame
	void Update () {
        switch (mState)
        {
            case 0:
                if (Time.time > mTimeout)
                {
                    int choice = Random.Range(0, 3);
                    mSubAI = (choice == 0 ? 1 : 0); // choice == 0 ? Wait : Run
                    mRunInited = false;
                    mFreezed = false;
                    mState = 1;
                    if (mSubAI == 0)
                    {
                        // MICHELE: #NOMIRROR qua anim di dash (se non ti interessa il mirror orizzontale) altrimenti cerca #MIRROR
                        mTimeout = Time.time + Random.Range(mRunTimeMin, mRunTimeMax);
                    }
                    else
                    {
                        mTimeout = Time.time + Random.Range(mWaitingTimeMin, mWaitingTimeMax);
                        mSpellAnim.SetActive(true); // questo coso andra` cambiato con il gameobject di un'onda o simili
                        // MICHELE: qua anim di "casto la spell che freeza"
                    }
                }
                break;
            case 1:
                bool b;
                if (mSubAI == 0)
                    b = Run();
                else
                    b = Wait();

                if (Time.time > mTimeout || b == false) 
                {
                    mSpellAnim.SetActive(false);
                    mState = 0;
                    mRB.velocity = Vector2.zero;
                    mTimeout = Time.time + Random.Range(mChosingTimeMin, mChosingTimeMax);
                    // MICHELE: qua anim di idle
                    break; 
                }
                break;
        }	
	}

    private Vector2 mRunTo;
    private bool mRunInited;
    private float mLastDist = 0.0f;

    // corre da un lato all'altro dello schermo passando sul giocatore
    bool Run()
    {
        if (!mRunInited)
        {
            mRunTo = GetRunTo();
            // MICHELE: anim di dash qua #MIRROR se non usi #NOMIRROR, qua controlla che mRunTo.x > mRB.position.x
            // per mettere l'anim di dash flippata giusta
            mLastDist = (new Vector2(mRB.position.x, mRB.position.y) - mRunTo).magnitude;
            mRunInited = true;
        }

        Vector2 tpos = mRB.position;
        Vector2 delta = mRunTo - tpos;
        if (delta.magnitude > mLastDist)
        {
            // target raggiunto o superato
            mRB.velocity = Vector2.zero;
            mRunTo = GetRunTo();
            // MICHELE: anim di dash qua #MIRROR (se non usi #NOMIRROR), stesso codice di poco sopra
            // quindi magari fai una funzione
            delta = mRunTo - tpos;
        }

        mRB.velocity = delta.normalized * mSpeed;
        mLastDist = delta.magnitude;
        return true;
    }

    private Vector2 GetRunTo()
    {
        Vector3[] boundaries = {
            new Vector3(0.75f, -2f,   1),
            new Vector3(9.21f, -2f,   1),
            new Vector3(9.21f, -7.3f, 1),
            new Vector3(0.75f, -7.3f, 1)
        };
        Vector3[] lines = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            int j = (i + 1) % 4;
            lines[i] = Vector3.Cross(boundaries[i], boundaries[j]);
        }

        Vector2 pc = mPlayer.transform.position;
        Vector2 mc = mRB.position;
        Vector2 direction = (pc - mc);

        Vector3 pc1 = new Vector3(pc.x, pc.y, 1);
        Vector3 mc1 = new Vector3(mc.x, mc.y, 1);

        Vector3 l1 = Vector3.Cross(pc1, mc1); // linea per i due punti
        
        // controllo le intersezioni sui 4 muri
        for (int i = 0; i < 4; i++)
        {
            int j = (i + 1) % 4;
            Vector3 r = Vector3.Cross(l1, lines[i]); 
            if (Mathf.Abs(r.z) < Mathf.Epsilon) // parallelo all'asse, -j DROP
                continue;
            r /= r.z; // normalizzo
            // controllo se sono nei boundaries dei muri
            if (((i == 0 || i == 2) && InBoundaries(r.x, boundaries[i].x, boundaries[j].x)) ||
                ((i == 1 || i == 3) && InBoundaries(r.y, boundaries[i].y, boundaries[j].y)))
            {
                // hit!
                Vector2 ret = new Vector2(r.x, r.y);
                Vector2 direction2 = (ret - mc);
                // controllo che sia il muro giusto e non quello "dietro"
                if (Vector2.Dot(direction.normalized, direction2.normalized) >= 0.9f && direction2.magnitude >= direction.magnitude)
                    return ret;
            }
        }
        // Bug: non trovo posizioni? miro il giocatore e basta
        return mRB.position;
    }

    bool InBoundaries(float x, float a, float b)
    {
        if (x >= Mathf.Min(a, b) - 0.001f && x <= Mathf.Max(a, b) + 0.001f)
            return true;
        return false;
    }

    private bool mFreezed = false;
    // aspetta fermo. se riesci a congelare il giocatore
    // e la vita e` minore di X, allora spawna nemici
    bool Wait()
    {
        if (!mFreezed)
        {
            Vector2 delta = mPlayer.transform.position - transform.position;

            Debug.Log(delta.magnitude);
            if (delta.magnitude < mSpellRadius)
            {
                mPlayer.GetComponent<InputManager>().Freeze(Time.time + mFreezeSeconds);
                mFreezed = true;
                OnFreeze();
            }
        }
        return true;
    }

    // codice da eseguire appena il giocatore viene freezato
    void OnFreeze()
    {
        EnemyLife life = GetComponent<EnemyLife>();
        if (life.mCurrentHP < 0.5f * life.GetTotalHP())
            SpawnEnemies();
    }

    void SpawnEnemies()
    {
        Vector2 pos = transform.position;

        GameObject el = Resources.Load("StompStomp") as GameObject;

        for (int i = 0; i < 3; i++)
        {
            GameObject go = GameObject.Instantiate(el);
            go.transform.position = pos + Random.insideUnitCircle * 2.0f;
            go.GetComponent<StompStomp>().mHackAttackInstant = true;
        }
    }
}
