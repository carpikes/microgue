using UnityEngine;
using System.Collections;
using System;

using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    public static string IS_FACING_RIGHT = "IS_FACING_RIGHT";

    public enum InputChoiches
    {
        KeyboardMouse,
        Joypad
    }

    enum PlayerDirection
    {
        LEFT,
        RIGHT,
        NONE,
    };


    Rigidbody2D rb;

    Camera mainCam;
    GameplayManager mGameManager = null;
    StatManager statManager = null;

    private float mShakeTime = 0.0f, mShakeForce = 0.0f;

    [Header("Aim transform")]
    public Transform aimTransform;

    [Header("Parameters for speed")]
    public float mAcceleration = 100f;
    public float mInitialSpeed = 20.0f;
    [Header("Friction")]
    public float mFriction = 5.0f;
    public float mMaxSpeed;

    [Header("Shots parameters")]
    public GameObject lightBall;
    public float shotCooldownTime = 0.2f;
    public float shotSpeed = 5f;
    float lastShootTime = 0f;

    [Header("Secondary Attack")]
    public int refillEnemiesToReload = 3;
    public int enemiesKilledCounter = 0;
    public float mSecondaryAttackDamage = 10f;
    bool sndAttackEnabled = true;
    
    [Header("Keyboard or joypad?")]
    InputInterface mInput;
    private InputChoiches mInputChoice;
    private Vector3 mLastMouseCoords; // usato per switchare a mouse se mosso

    public Color mBallColor;
    private float mFreezedUntil = 0;

    // True se e` in shooting.
    private bool mIsShooting = false;
    PlayerDirection mLastDirection = PlayerDirection.NONE;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_ENEMY_DEATH, UpdateCounter);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_ENEMY_DEATH, UpdateCounter);
    }

    // Use this for initialization
    void Start ()
    {
        mMaxSpeed = mInitialSpeed;

        mInputChoice = InputChoiches.KeyboardMouse;
        mInput = gameObject.AddComponent<KeyboardInput>();

        rb = GetComponent<Rigidbody2D>();
        
        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();

        mBallColor = Color.white;
        mainCam = Camera.main;
        SetPositionCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));
        mIsShooting = false;
        mLastMouseCoords = Input.mousePosition;
    }

    private void DetectDevice() {
        if (mInputChoice == InputChoiches.KeyboardMouse) {
            if (Mathf.Abs(Input.GetAxisRaw("Shoot Camera Horizontal")) > 0.1f)
            {
                Destroy(gameObject.GetComponent<KeyboardInput>());
                mInput = gameObject.AddComponent<JoypadInput>();
                mInputChoice = InputChoiches.Joypad;
                mLastMouseCoords = Input.mousePosition;
                Debug.Log("Switching to Joypad");
            }
        } else {
            if ((mLastMouseCoords - Input.mousePosition).magnitude > 10.0f)
            {
                Destroy(gameObject.GetComponent<JoypadInput>());
                mInput = gameObject.AddComponent<KeyboardInput>();
                mInputChoice = InputChoiches.KeyboardMouse;
                mLastMouseCoords = Input.mousePosition;
                Debug.Log("Switching to Keyboard");
            }
        }
    }

    private void SetPositionCamera()
    {
        Vector2 normCoords = GetNormalizedPointerCoordinates();

        Vector3 cameraPos = new Vector3(
            transform.position.x + normCoords.x, transform.position.y + normCoords.y, -1);

        // TODO HACK
        if (mGameManager == null)
        {
            GameObject gm = GameObject.Find("GameplayManager");
            mGameManager = gm.GetComponent<GameplayManager>();
        }

        Vector3[] cameraBound = mGameManager.GetWorldManager().GetCameraBounds();
        if (cameraBound != null)
        {
            cameraPos.x = Mathf.Clamp(cameraPos.x, cameraBound[0].x, cameraBound[1].x);
            cameraPos.y = Mathf.Clamp(cameraPos.y, cameraBound[0].y, cameraBound[1].y);
        }

        Vector3 r = Vector3.zero;
        if (Time.time < mShakeTime)
            r = UnityEngine.Random.insideUnitCircle / 20.0f * mShakeForce;
        else
            mShakeForce = 0.0f;
        mainCam.transform.position = cameraPos + r;
    }

    public void ShakeCamera(float duration, float force) {
        float finishTime = Time.time + duration;
        if (mShakeTime < finishTime)
            mShakeTime = finishTime;
        if(mShakeForce < force)
            mShakeForce = force;
    }

    private Vector2 GetNormalizedPointerCoordinates()
    {
        Vector2 sp = mInput.GetScreenPointerCoordinates();
        return new Vector2((sp.x / Screen.width) * 2 - 1, (sp.y / Screen.height) * 2 - 1);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (Time.time < mFreezedUntil)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 delta = mInput.GetVelocityDelta();

        // TODO HACK
        /*
        if( delta == Vector2.zero )
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_IDLE, null);
        } else
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_MOVE, null);
        }
        */

        Vector2 newVelocity = rb.velocity;

        newVelocity += delta * mAcceleration * Time.fixedDeltaTime;
        Vector2 friction = mFriction * newVelocity * Time.fixedDeltaTime;

        newVelocity = (friction.magnitude >= newVelocity.magnitude) ? Vector2.zero : newVelocity - friction;

        if (newVelocity.magnitude > mMaxSpeed)
            newVelocity = newVelocity.normalized * mMaxSpeed;

        rb.velocity = newVelocity;
    }

    public void SetMaxSpeed(float v)
    {
        mMaxSpeed = v;
    }

    internal void SetStatSpeed(float v)
    {
        mMaxSpeed = mInitialSpeed + v;
    }

    private void Aim()
    {
        mInput.FeedPlayerPosition(mainCam.WorldToScreenPoint(transform.position) - new Vector3(mainCam.pixelWidth / 2.0f, mainCam.pixelHeight / 2.0f, 0));
        Vector2 p = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());

        aimTransform.transform.position = p;
    }

    void LateUpdate()
    {
        DetectDevice();
        CheckDirection();
        SetPositionCamera();
        Aim();
    }

    PlayerDirection GetAimDirection()
    {
        Vector2 p = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());
        Vector2 aimCoords = (p - new Vector2(transform.position.x, transform.position.y)).normalized;
        if(aimCoords.x < -0.05f)
            return PlayerDirection.LEFT;
        if(aimCoords.x > 0.05f)
            return PlayerDirection.RIGHT;
        return PlayerDirection.NONE;
    }

    PlayerDirection GetMovingDirection()
    {
        if (rb.velocity.x < -0.05f)
            return PlayerDirection.LEFT;
        if (rb.velocity.x > 0.05f)
            return PlayerDirection.RIGHT;
        return PlayerDirection.NONE;
    }

    void UpdateDirection(PlayerDirection dir)
    {
        if (dir == PlayerDirection.NONE)
            return;

        if (mLastDirection == PlayerDirection.NONE && dir == PlayerDirection.NONE)
            dir = PlayerDirection.LEFT;

        if (dir != mLastDirection)
        {
            Bundle d = new Bundle();
            d.Add(IS_FACING_RIGHT, (dir == PlayerDirection.RIGHT ? true : false).ToString());

            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_CHANGE_DIR, d);
        }
    }

    void CheckDirection()
    {
        if (mIsShooting)
            UpdateDirection(GetAimDirection());
        else
            UpdateDirection(GetMovingDirection());
    }

    void Update()
    {

        if (mInput.IsSkipToBossPressed())
        {
            EventManager.TriggerEvent(Events.ON_BOSS_GOTO, null);
            return;
        }

        if (Time.time < mFreezedUntil)
            return; 

        // vvvvvv THIS CODE IS NOT EXECUTED IF THE PLAYER IS FREEZED vvvvvv
        if (!mIsShooting && mInput.IsShootingButtonPressed() && CanShoot())
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_START_ATTACK, null);
            mIsShooting = true;
        }

        if (mIsShooting && !mInput.IsShootingButtonPressed())
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_STOP_ATTACK, null);
            mIsShooting = false;
        }

        if(mIsShooting && CanShoot())
            Shoot();

        /*
        if (mInput.IsItemButtonPressed())
        {
            Debug.Log("Item");
            playerManager.UseActiveItem();
        }

        if (mInput.IsDashButtonPressed())
            Dash();
        */

        if (mInput.IsSecondaryAttackButtonPressed())
            SecondaryAttack();
    }

    private void Shoot()
    {
        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_KEEP_ATTACK, null);
        GameObject lb = Instantiate(lightBall);
        lb.GetComponent<ShotProperties>().mDamage = statManager.GetStatValue(StatManager.StatStates.DAMAGE);
        lb.GetComponent<SpriteRenderer>().color = mBallColor;

        Vector2 playerPos = transform.position;
        Vector2 pointer = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());
        Vector2 direction = (pointer - playerPos).normalized;

        Rigidbody2D ball2d = lb.GetComponent<Rigidbody2D>();
        Vector3 offset = new Vector3(0.15f, -0.05f, 0.0f);
        offset.x *= direction.x > 0.0f ? 1.0f : -1.0f;

        lb.transform.position = transform.position + offset;
        ball2d.velocity = direction * shotSpeed;

        lastShootTime = Time.time;
    }

    private bool CanShoot()
    {
        return Time.time - lastShootTime > shotCooldownTime;
    }

    private void UpdateCounter(Bundle arg0)
    {
        ++enemiesKilledCounter;

        Bundle b = new Bundle();
        b.Add(CanvasManager.SECONDARY_ATTACK_BAR, Mathf.Clamp01( (float)enemiesKilledCounter / refillEnemiesToReload).ToString());

        EventManager.TriggerEvent(Events.UPDATE_SECONDARY_ATTACK, b);
    }

    IEnumerator InvertColors()
    {
        InvertColors col = Camera.main.GetComponent<InvertColors>();
        for (float i = 0; i < 1.0f; i += 0.2f)
        {
            col.mIntensity = i;
            yield return new WaitForSeconds(0.01f);
        }
        col.mIntensity = 1.0f;
        yield return new WaitForSeconds(0.2f);
        for (float i = 1.0f; i > 0.0f; i -= 0.2f)
        {
            col.mIntensity = i;
            yield return new WaitForSeconds(0.01f);
        }
        col.mIntensity = 0.0f;
    }

    private void SecondaryAttack()
    {
        if (sndAttackEnabled && refillEnemiesToReload <= enemiesKilledCounter)
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_SECOND_ATTACK, null);
            //Debug.Log("BUM");
            sndAttackEnabled = false;

            StartCoroutine(InvertColors());
            
            DamageAllEnemies();

            StartCoroutine(ResetSecondAttackStats());
        }
    }

    private IEnumerator ResetSecondAttackStats()
    {
        Bundle b = new Bundle();
        b.Add(CanvasManager.SECONDARY_ATTACK_BAR, "0");
        EventManager.TriggerEvent(Events.UPDATE_SECONDARY_ATTACK, b);

        yield return new WaitForSeconds(1f);
        enemiesKilledCounter = 0;

        sndAttackEnabled = true;
    }

    private void DamageAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach( var enemy in enemies )
        {
            EnemyLife lifeScript = enemy.GetComponent<EnemyLife>();
            if (lifeScript == null)
                continue;

            if (!lifeScript.mIsInvincible)
                lifeScript.Damage(mSecondaryAttackDamage);
        }
    }

    /*private void Dash()
    {
        Debug.Log("Dash()");
        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_DASH, null);
    }*/

    public void Freeze(float timeout)
    {
        mFreezedUntil = timeout;
    }
}
