﻿using UnityEngine;
using System.Collections;
using System;

public class JoypadInput : MonoBehaviour, InputInterface {
    private Vector2 mCurRStick;
    public float mCutOffFrequency = 2.0f;
    public float mCircleArea = 0.5f;

    void Start () {
        mCurRStick = new Vector2(0, 0);
    }

    void FixedUpdate() {
        Vector2 pos = GetRawPointerCoordinates();

        float RC = 1.0f / (2 * Mathf.PI * mCutOffFrequency);
        float a = Time.fixedDeltaTime / (RC + Time.fixedDeltaTime);
        mCurRStick = a * pos + (1.0f - a) * mCurRStick;
    }

    public Vector2 GetScreenPointerCoordinates()
    {
        return mCurRStick;
    }

    private Vector2 GetRawPointerCoordinates()
    {
        float horR = Input.GetAxis("JRHorizontal");
        float verR = Input.GetAxis("JRVertical");

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

        float hor = Input.GetAxis("JHorizontal");
        float ver = Input.GetAxis("JVertical");

        if (hor < -float.Epsilon)
            delta += Vector2.left;
        else if (hor > float.Epsilon)
            delta += Vector2.right;

        if (ver > float.Epsilon)
            delta += Vector2.down;
        else if (ver < -float.Epsilon)
            delta += Vector2.up;

        if (delta != Vector2.zero)
            delta.Normalize();

        return delta;
    }

    public bool IsShootingButtonKeepPressed()
    {
        return false;
    }

    public bool IsItemButtonPressed()
    {
        return false;
    }

    public bool isDashButtonPressed()
    {
        return false;
    }

    public bool isSecondaryAttackButtonPressed()
    {
        return false;
    }

    public bool IsShootingButtonReleased()
    {
        return false;
    }

    public bool IsShootingButtonPressed()
    {
        return false;
    }
}
