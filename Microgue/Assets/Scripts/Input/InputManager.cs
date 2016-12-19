using UnityEngine;
using System.Collections;
using System;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class InputManager : MonoBehaviour {

    public static string IS_FACING_RIGHT = "IS_FACING_RIGHT";

    public enum InputChoiches
    {
        KeyboardMouse,
        Joypad
    }

    Rigidbody2D rb;

    Camera mainCam;
    GameplayManager mGameManager = null;
    PlayerItemHandler playerManager = null;
    StatManager statManager = null;

    PlayerAnimationManager animManager;

    float lastAimX = 0.0f;
    private float mShakeTime = 0.0f, mShakeForce = 0.0f;

    [Header("Aim transform")]
    public Transform aimTransform;

    [Header("Parameters for speed")]
    public float mAcceleration = 100f;
    public float mInitialSpeed = 20.0f;
    [Header("Friction")]
    public float mFriction = 5.0f;
    private float mMaxSpeed;

    [Header("Shots parameters")]
    public GameObject lightBall;
    public float shotCooldownTime = 0.2f;
    public float shotSpeed = 5f;
    float lastShootTime = 0f;

    [Header("Keyboard or joypad?")]
    InputInterface mInput;
    public InputChoiches mInputChoice;
    public Color mBallColor;

    // True se e` in shooting.
    private bool mIsShooting = false;

    // Use this for initialization
    void Start ()
    {
        mMaxSpeed = mInitialSpeed;

        if (mInputChoice == InputChoiches.KeyboardMouse)
        {
            mInput = gameObject.AddComponent<KeyboardInput>();
        } else {
            mInput = gameObject.AddComponent<JoypadInput>();
        }

        rb = GetComponent<Rigidbody2D>();
        
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerItemHandler>();
        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();
        animManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerAnimationManager>();

        mBallColor = Color.white;
        mainCam = Camera.main;
        SetPositionCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));
        mIsShooting = false;
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
        Vector3[] cameraBound = mGameManager.GetCameraBounds();
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

    private Vector2 GetNormalizedPlayerCoordinates()
    {
        Vector2 sp = mainCam.WorldToScreenPoint(transform.position); 
        return new Vector2((sp.x / Screen.width) * 2 - 1, (sp.y / Screen.height) * 2 - 1);
    }

    private Vector2 GetNormalizedPointerCoordinates()
    {
        Vector2 sp = mInput.GetScreenPointerCoordinates();
        return new Vector2((sp.x / Screen.width) * 2 - 1, (sp.y / Screen.height) * 2 - 1);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
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
        Vector2 friction = mFriction * newVelocity * Time.fixedDeltaTime;

        newVelocity += delta * mAcceleration * Time.fixedDeltaTime;
        newVelocity = (friction.magnitude >= newVelocity.magnitude) ? Vector2.zero : newVelocity - friction;

        if (newVelocity.magnitude > mMaxSpeed)
            newVelocity = newVelocity.normalized * mMaxSpeed;

        rb.velocity = newVelocity;
    }

    internal void setSpeed(int v)
    {
        mMaxSpeed = mInitialSpeed + v * 10;
    }

    private void Aim()
    {
        mInput.FeedPlayerPosition(mainCam.WorldToScreenPoint(transform.position) - new Vector3(mainCam.pixelWidth / 2.0f, mainCam.pixelHeight / 2.0f, 0));
        Vector2 p = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());

        aimTransform.transform.position = p;
    }

    void LateUpdate()
    {
        CheckDirection();
        SetPositionCamera();
        Aim();
    }

    void CheckDirection()
    {
        Vector2 p = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());
        Vector2 aimCoords = p - new Vector2(transform.position.x, transform.position.y);
        if( aimCoords.x * lastAimX < 0.0f )
        {
            Bundle dir = new Bundle();
            dir.Add(IS_FACING_RIGHT, (aimCoords.x >= 0.0f ? true : false).ToString() );

            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_CHANGE_DIR, dir);
        }

        lastAimX = aimCoords.x;
    }

    void Update()
    {
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

        if (mInput.IsItemButtonPressed())
            playerManager.UseActiveItem();

        if (mInput.IsDashButtonPressed())
            Dash();

        if (mInput.IsSecondaryAttackButtonPressed())
            SecondaryAttack();
    }

    private void Shoot()
    {
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

    private void SecondaryAttack()
    {
        Debug.Log("SecondaryAttack()");
        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_SECOND_ATTACK, null);
    }

    private void Dash()
    {
        Debug.Log("Dash()");
        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_DASH, null);
    }
}
