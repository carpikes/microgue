﻿using UnityEngine;
using System.Collections;
using System;

public class JoypadInput : MonoBehaviour, InputInterface {
    private Vector2 mCurRStick;
    private float mCutOffFrequency = 1.5f;
    private float mCircleArea = 0.8f;
    public Vector2 mCurPlayerPos = Vector2.zero;

    // shoot button is os depedent
    bool isWindows;
    private string shootString;


    void Start () {
        mCurRStick = new Vector2(0, 0);

        // if I'm on windows...
        if( Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
            isWindows = true;
            shootString = "Shoot";
        } else
        {
            isWindows = false;
            shootString = "ShootLinux";
        }
    }

    void FixedUpdate() {
        Vector2 pos = GetRawPointerCoordinates();

        float RC = 1.0f / (2 * Mathf.PI * mCutOffFrequency);
        float a = Time.fixedDeltaTime / (RC + Time.fixedDeltaTime);
        mCurRStick = a * pos + (1.0f - a) * mCurRStick;
    }

    public Vector2 GetScreenPointerCoordinates()
    {
        return mCurPlayerPos + mCurRStick;
    }

    private Vector2 GetRawPointerCoordinates()
    {
        float horR = Input.GetAxis("Shoot Camera Horizontal");
        float verR = Input.GetAxis("Shoot Camera Vertical");

        float w = Camera.main.pixelWidth;
        float h = Camera.main.pixelHeight;

        float ratioX = 1.0f, ratioY = 1.0f;
        if (w > h)
            ratioX = h / w;
        else
            ratioY = w / h;

        // consider only 80% on the x and y axis of the screen
        float wClamp = mCircleArea * ratioX;
        float hClamp = mCircleArea * ratioY;

        // wClamp * Screen.width * (horR + 1) / 2 + (1-wClamp)*Screen.width/2;
        float screenHor = Screen.width * (1 + wClamp * horR) / 2;

        //hClamp * Screen.height * ( (verR * (-1)) + 1) / 2 + (1 - hClamp) * Screen.height/2;
        float screenVer = Screen.height * (1 - hClamp * verR) / 2;

        return new Vector2(screenHor, screenVer);
    }

    public Vector2 GetVelocityDelta()
    {
        Vector2 delta = new Vector2(0, 0);

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        if (Mathf.Abs(hor) < 0.1f)
            hor = 0.0f;
        if (Mathf.Abs(ver) < 0.1f)
            ver = 0.0f;

        delta.x += hor;
        delta.y += ver;

        if (delta != Vector2.zero)
            delta.Normalize();

        return delta;
    }

    public bool IsItemButtonPressed()
    {
        return Input.GetButtonDown("Item");
    }

    public bool IsDashButtonPressed()
    {
        //float v = Input.GetAxisRaw("Shoot");
        //return (v > 0.2);
        return false;
    }

    public bool IsSecondaryAttackButtonPressed()
    {
        return Input.GetButtonDown("Shoot 2");
    }

    public bool IsShootingButtonPressed()
    {
        float v = Input.GetAxisRaw(shootString);
        return isWindows ? (v < -0.2f) : (v > 0.2f);
    }

    public void FeedPlayerPosition(Vector2 pos)
    {
        mCurPlayerPos = pos;
    }

    public bool IsSkipToBossPressed()
    {
        return Input.GetButtonDown("SkipBoss");
    }
}
