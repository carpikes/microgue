using UnityEngine;
using System.Collections;
using System.Timers;
using System;

using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System.Collections.Generic;

public class TimerManager : MonoBehaviour {

    // how much time the player has in total
    public float MAX_TIME = 300.0f;

    // in the format seconds.milliseconds
    private float mTicksLeft;
    private int mLastSecond;
    private float mScaleTimer; // divide time delta time according to stat value, to make time go to different speeds

    private bool mIsTimerOn;

    public static readonly string TICKS_LEFT_TAG = "TICKS_LEFT";

    public void Start()
    {
        mTicksLeft = MAX_TIME;
        mLastSecond = (int)mTicksLeft;

        mIsTimerOn = true;
        mScaleTimer = 1.0f;
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.INCREMENT_TIME, IncrementTicks);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.INCREMENT_TIME, IncrementTicks);
    }

    public void Update()
    {
        Debug.Log(mTicksLeft);

        if( mIsTimerOn )
        {
            mTicksLeft -= Time.deltaTime / mScaleTimer;

            if ((int)mTicksLeft < mLastSecond)
            {
                UpdateCountdown();
            }
        }
    }

    private void UpdateCountdown()
    {
        Bundle tickBundle = new Bundle();
        tickBundle.Add(TICKS_LEFT_TAG, Mathf.RoundToInt(mTicksLeft).ToString());
        EventManager.TriggerEvent(Events.ON_TICK, tickBundle);

        mLastSecond = (int)mTicksLeft;

        if( mTicksLeft <= 0 )
        {
            Debug.Log("END OF TIME!");
            EventManager.TriggerEvent(Events.ON_TIME_ENDED, null);

            mIsTimerOn = false;
        }
    }

    /*public void setInterval(int s)
    {
        // linear interpolation between 1 second interval (s=1) and 2 second interval (s=10)
        mScaleTimer = (s - 1) / 9.0f + 1;
    }*/

    private void IncrementTicks(Bundle args)
    {
        mTicksLeft += 30.0f;
        UpdateCountdown();
    }

}
