using UnityEngine;
using System.Collections;
using System.Timers;
using System;

public class TimerManager : MonoBehaviour {

    public const long MAX_TIME = 10;

    Timer everySecondTimer = null;
    private long secondsLeft = MAX_TIME;

    public void Start()
    {
        StartEverySecondTimer();
    }

    private void StartEverySecondTimer()
    {
        everySecondTimer = new System.Timers.Timer();
        everySecondTimer.Elapsed += new ElapsedEventHandler(OnSecondPassed);
        everySecondTimer.Interval = 1000;
        everySecondTimer.Enabled = true;
    }

    private void OnSecondPassed(object sender, ElapsedEventArgs e)
    {
        //EventManager.TriggerEvent(Events.ON_SECOND_PASSED, null);

        //Debug.Log("Time is running out!");
        DecreaseCountdown();
    }

    private void DecreaseCountdown()
    {
        secondsLeft--;

        if( secondsLeft <= 0 )
        {
            Debug.Log("Goodbye cruel world, I'm leaving you today!");
            EventManager.TriggerEvent(Events.ON_TIME_ENDED, null);
            everySecondTimer.Stop();
        }
    }

    public void OnDisable()
    {
        if (everySecondTimer != null)
            everySecondTimer.Stop();
    }
}
