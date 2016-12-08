using UnityEngine;
using System.Collections;
using System.Timers;
using System;

public class TimerManager : MonoBehaviour {

    public const long MAX_TIME = 1000;

    Timer everySecondTimer = null;
    private long secondsLeft = MAX_TIME;

    private int intervalTime = 1000;
    public int IntervalTime
    {
        get
        {
            return intervalTime;
        }

        set
        {
            intervalTime = value;
        }
    }

    public void Start()
    {
        StartEverySecondTimer();
    }

    private void StartEverySecondTimer()
    {
        everySecondTimer = new System.Timers.Timer();
        everySecondTimer.Elapsed += new ElapsedEventHandler(OnTick);
        everySecondTimer.Interval = IntervalTime;
        everySecondTimer.Enabled = true;
    }

    private void OnTick(object sender, ElapsedEventArgs e)
    {
        //EventManager.TriggerEvent(Events.ON_SECOND_PASSED, null);
        Debug.Log("TICK");
        DecreaseCountdown();
    }

    private void DecreaseCountdown()
    {
        secondsLeft--;

        if( secondsLeft <= 0 )
        {
            EventManager.TriggerEvent(Events.ON_TIME_ENDED, null);
            everySecondTimer.Stop();
        }
    }

    public void OnDisable()
    {
        if (everySecondTimer != null)
            everySecondTimer.Stop();
    }

    public void setInterval(int v)
    {
        IntervalTime = 1000 + 100 * v;
        Debug.Log(intervalTime);
        everySecondTimer.Interval = IntervalTime;
    }
}
