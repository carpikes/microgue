using UnityEngine;
using System.Collections;
using System.Timers;
using System;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class TimerManager : MonoBehaviour {

    // how much time the player has in total
    public const float MAX_TIME = 200.0f;

    // in the format seconds.milliseconds
    private float ticksLeft;
    private int lastSecond;
    private float scaleTimer; // divide time delta time according to stat value, to make time go to different speeds

    private bool isTimerOn;

    public static readonly string TICKS_LEFT_TAG = "TICKS_LEFT";

    public void Start()
    {
        ticksLeft = MAX_TIME;
        lastSecond = (int)ticksLeft;

        isTimerOn = true;
        scaleTimer = 1.0f;
    }

    public void Update()
    {
        if( isTimerOn )
        {
            ticksLeft -= Time.deltaTime / scaleTimer;

            if ((int)ticksLeft < lastSecond)
            {
                DecreaseCountdown();
            }
        }
    }

    private void DecreaseCountdown()
    {
        Bundle tickBundle = new Bundle();
        tickBundle.Add(TICKS_LEFT_TAG, Mathf.RoundToInt(ticksLeft).ToString());
        EventManager.TriggerEvent(Events.ON_TICK, tickBundle);

        lastSecond = (int)ticksLeft;

        if( ticksLeft <= 0 )
        {
            Debug.Log("END OF TIME!");
            EventManager.TriggerEvent(Events.ON_TIME_ENDED, null);

            isTimerOn = false;
        }
    }

    public void setInterval(int s)
    {
        // linear interpolation between 1 second interval (s=1) and 2 second interval (s=10)
        scaleTimer = (s - 1) / 9.0f + 1;
    }

}
