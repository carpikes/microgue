﻿using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour {

    public enum InputChoiches
    {
        KeyboardMouse,
        Joypad
    }

    Rigidbody2D rb;
    Animator animator;
    Camera mainCam;
    private GameplayManager mGameManager = null;

    float oldAnimationDirX = 0.0f;

    public Transform aimTransform;

    [Header("Parameters for speed")]
    public float speed = 100f;

    [Header("Shots parameters")]
    public GameObject lightBall;
    public float shotCooldownTime = 0.2f;
    public float shotSpeed = 5f;
    float lastShootTime = 0f;

    InputInterface mInput;
    public InputChoiches mInputChoice;

    // Use this for initialization
    void Start ()
    {
        if (mInputChoice == InputChoiches.KeyboardMouse)
        {
            mInput = gameObject.AddComponent<KeyboardInput>();
        } else {
            mInput = gameObject.AddComponent<JoypadInput>();
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        mainCam = Camera.main;
        SetPositionCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));
    }

    private void SetPositionCamera()
    {
        Vector2 normCoords = GetNormalizedCoordinates();

        SetPlayerAnimation(normCoords);

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
        mainCam.transform.position = cameraPos;
    }

    private Vector2 GetNormalizedCoordinates()
    {
        Vector2 sp = mInput.GetScreenPointerCoordinates();
        Vector2 normCoords =
            new Vector2((sp.x / Screen.width) * 2 - 1, (sp.y / Screen.height) * 2 - 1);
        return normCoords;
    }

    private void SetPlayerAnimation( Vector2 dir )
    {
        // change of direction
        if (oldAnimationDirX * dir.x <= 0)
        {
            animator.SetFloat("dir_x", dir.x);
            oldAnimationDirX = dir.x;
        }
    }

    void Update()
    {
        if (mInput.IsShootingButtonPressed())
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime > shotCooldownTime)
        {
            GameObject lb = Instantiate(lightBall);
            lb.transform.position = transform.position;

            Vector2 playerPos = transform.position;
            Vector2 pointer = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());
            Vector2 direction = (pointer - playerPos).normalized;
            ((Rigidbody2D)lb.GetComponent<Rigidbody2D>()).velocity = direction * shotSpeed;

            lastShootTime = Time.time;
        }
    }

    void LateUpdate()
    {
        SetPositionCamera();
        Aim();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        ChangeVelocity();
    }

    private void Aim()
    {
        Vector2 p = mainCam.ScreenToWorldPoint(mInput.GetScreenPointerCoordinates());

        aimTransform.transform.position = p;
    }

    private void ChangeVelocity()
    {
        Vector2 delta = mInput.GetVelocityDelta();
        rb.velocity = delta * speed * Time.fixedDeltaTime;
    }
}