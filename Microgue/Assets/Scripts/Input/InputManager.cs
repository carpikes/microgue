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
    PlayerManager playerManager = null;
    StatManager statManager = null;

    float lastAimX = 0.0f;
    private float mShakeTime = 0.0f, mShakeForce = 0.0f;

    [Header("Aim transform")]
    public Transform aimTransform;

    [Header("Parameters for speed")]
    public float initialSpeed = 100f;
    private float speed;

    [Header("Shots parameters")]
    public GameObject lightBall;
    public float shotCooldownTime = 0.2f;
    public float shotSpeed = 5f;
    float lastShootTime = 0f;

    [Header("Keyboard or joypad?")]
    InputInterface mInput;
    public InputChoiches mInputChoice;

    // Use this for initialization
    void Start ()
    {
        speed = initialSpeed;

        if (mInputChoice == InputChoiches.KeyboardMouse)
        {
            mInput = gameObject.AddComponent<KeyboardInput>();
        } else {
            mInput = gameObject.AddComponent<JoypadInput>();
        }

        rb = GetComponent<Rigidbody2D>();
        
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerManager>();
        statManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();

        mainCam = Camera.main;
        SetPositionCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));
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

    private Vector2 GetNormalizedPointerCoordinates()
    {
        Vector2 sp = mInput.GetScreenPointerCoordinates();
        Vector2 normCoords =
            new Vector2((sp.x / Screen.width) * 2 - 1, (sp.y / Screen.height) * 2 - 1);
        return normCoords;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        ChangeVelocity();
    }

    private void ChangeVelocity()
    {
        Vector2 delta = mInput.GetVelocityDelta();
        rb.velocity = delta * speed * Time.fixedDeltaTime;
    }

    internal void setSpeed(int v)
    {
        speed = initialSpeed + v * 10;
    }

    private void Aim()
    {
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
        if (mInput.IsShootingButtonPressed())
        {
            Shoot();
        }

        if (mInput.IsItemButtonPressed())
        {
            playerManager.UseActiveItem();
        }

        if (mInput.isDashButtonPressed())
        {
            Dash();
        }

        if (mInput.isSecondaryAttackButtonPressed())
        {
            SecondaryAttack();
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime > shotCooldownTime)
        {
            GameObject lb = Instantiate(lightBall);
            lightBall.GetComponent<ShotDamage>().Damage = statManager.GetStatValue(StatManager.StatStates.DAMAGE);

            Vector2 playerPos = transform.position;
            Vector2 pointer = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());
            Vector2 direction = (pointer - playerPos).normalized;

            Rigidbody2D ball2d = lb.GetComponent<Rigidbody2D>();
            Vector3 offset = new Vector3(0.15f, -0.05f, 0.0f);
            offset.x *= direction.x > 0.0f ? 1.0f : -1.0f;
            
            lb.transform.position = transform.position + offset;
            ball2d.velocity = direction * shotSpeed;

            lastShootTime = Time.time;

            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_ATTACK, null);
        }
    }

    private void SecondaryAttack()
    {
        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_SECOND_ATTACK, null);
    }

    private void Dash()
    {
        EventManager.TriggerEvent(Events.ON_MAIN_CHAR_DASH, null);
    }
}
