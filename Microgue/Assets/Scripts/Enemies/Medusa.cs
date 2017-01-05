using UnityEngine;
using System.Collections;

public class Medusa : MonoBehaviour
{
    private AIMap mAIMap;
    private bool[,] mMap = null;
    private int mState = 0;
    private int mSubAI = 0;

    private float mTimeout = 0;
    private GameObject mPlayer;
    private Rigidbody2D mRB;

	// Use this for initialization
	void Start () {
        mTimeout = Time.time + 2.0f;
        mPlayer = GameObject.Find("MainCharacter");
        mRB = GetComponent<Rigidbody2D>();
        mAIMap = GameObject.Find("GameplayManager").GetComponent<AIMap>();
	}
	
	// Update is called once per frame
	void Update () {
        if (mMap == null && mAIMap.IsMapReady)
                mMap = mAIMap.GetMap();

        if (mMap == null)
            return;

        switch (mState)
        {
            case 0:
                if (Time.time > mTimeout)
                {
                    int choice = Random.Range(0, 3);
                    mSubAI = (choice == 0 ? 1 : 0); // choice == 0 ? Wait : Run
                    mSubAI = 0;
                    Debug.Log("SubAI: " + mSubAI);
                    mRunInited = false;
                    mFreezed = false;
                    mState = 1;
                    mTimeout = Time.time + Random.Range(3.0f, 5.0f);
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
                    Debug.Log("Timeout!");
                    mState = 0;
                    mTimeout = Time.time + Random.Range(1.0f, 2.0f);
                    break; 
                }
                break;
        }	
	}

    private Vector2 mRunTo;
    private bool mRunInited;

    // corre da un lato all'altro dello schermo passando sul giocatore
    bool Run()
    {
        if (!mRunInited)
        {
            mRunTo = GetRunTo();
            mRunInited = true;
        }

        Vector2 tpos = transform.position;
        Vector2 delta = mRunTo - tpos;
        if (delta.magnitude < 1.0f || (mRB.velocity != Vector2.zero && Vector2.Dot(delta.normalized, mRB.velocity.normalized) < -0.9f))
        {
            // target raggiunto o superato
            mRunTo = GetRunTo();
            delta = mRunTo - tpos;
        }

        mRB.velocity = delta.normalized * 6.0f;
        return true;
    }

    // Ottiene in punto piu` lontano sulla retta
    // su cui giaciono i punti
    // 1) attuale
    // 2) giocatore
    private Vector2 GetRunTo()
    {
        AIMap.IntPoint player = AIMap.WorldToTileCoordinates(mPlayer.transform.position);
        AIMap.IntPoint me = AIMap.WorldToTileCoordinates(transform.position);

        AIMap.IntPoint jumpTo = player;

        int dx = player.x - me.x;
        int dy = player.y - me.y;
        if ((mPlayer.transform.position - transform.position).magnitude < 1.0f)
            dx = dy = 0;

        if (dx == 0 && dy == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                jumpTo.x = Random.Range(0, mMap.GetLength(0));
                jumpTo.y = Random.Range(0, mMap.GetLength(1));
                if (!mMap[jumpTo.x, jumpTo.y])
                    break; 
            }
        }

        if (dx == 0)
        {
            int lastValid = me.y;
            int inc = (dy > 0) ? 1 : -1;
            for (int y = me.y + inc; !mMap[me.x,y]; y += inc)
                lastValid = y;
            jumpTo.y = lastValid;
        }
        else
        {
            float d = (float)dy / (float)dx;
            int inc = (dx > 0) ? 1 : -1;
            float y = me.y;
            int x = me.x + inc;
            for (; x < mMap.GetLength(0) && y < mMap.GetLength(1) && x >= 0 && y >= 0 && !mMap[(int)x, (int)y]; x += inc)
                y += d * (float)inc;
            jumpTo.x = x;
            jumpTo.y = (int)y;
        }

        return AIMap.TileToWorldCoordinates(jumpTo);
    }

    private bool mFreezed = false;
    // aspetta fermo. se riesci a congelare il giocatore
    // e la vita e` minore di X, allora spawna nemici
    bool Wait()
    {
        if (!mFreezed)
        {
            Vector2 delta = mPlayer.transform.position - transform.position;

            if (delta.magnitude < 2.0f)
            {
                mPlayer.GetComponent<InputManager>().Freeze(Time.time + 4.0f);
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
//            mEnemies.Add(go);
        }
    }
}
