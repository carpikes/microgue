using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{

    public enum StatStates
    {
        CURRENT_HEALTH,
        MAX_HEALTH,
        DEFENCE,
        DAMAGE,
        TEMP_DISTORSION,
        SPEED,

        NULL
    };

    const int numberOfStates = (int)StatStates.NULL;
    public Stat[] stats;

    public GameObject mPlayer;
    public float MAX_DEFENCE = 2f;

    bool isInvulnerable;
    GameplayManager gameplayMgr;
    TimerManager timerMgr;
    InputManager inputMgr;
    PlayerAnimationManager animMgr;

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_MAIN_CHAR_HIT, DecreaseEnergy);
        EventManager.StartListening(Events.ON_MAIN_CHAR_INVULNERABLE_BEGIN, SetInvulnerable);
        EventManager.StartListening(Events.ON_MAIN_CHAR_INVULNERABLE_END, SetVulnerable);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_MAIN_CHAR_HIT, DecreaseEnergy);
        EventManager.StopListening(Events.ON_MAIN_CHAR_INVULNERABLE_BEGIN, SetInvulnerable);
        EventManager.StopListening(Events.ON_MAIN_CHAR_INVULNERABLE_END, SetVulnerable);

    }

    private void SetVulnerable(Dictionary<string, string> arg0)
    {
        IsInvulnerable = false;
    }

    private void SetInvulnerable(Dictionary<string, string> arg0)
    {
        IsInvulnerable = true;
    }

    private void SetupStat(StatStates s, float min, float max, bool show)
    {
        stats[(int)s] = new Stat(s.ToString(), min, max, show);
    }

    public void Start()
    {
        stats = new Stat[numberOfStates];

        SetupStat(StatStates.MAX_HEALTH, 10, 10, false);
        SetupStat(StatStates.CURRENT_HEALTH, 0, stats[(int)StatStates.MAX_HEALTH].CurrentValue, false);
        SetupStat(StatStates.DEFENCE, 1, 10, true);
        SetupStat(StatStates.DAMAGE, 10, 10, true);
        SetupStat(StatStates.TEMP_DISTORSION, 1, 10, true);
        SetupStat(StatStates.SPEED, 1, 10, true);

        stats[(int)StatStates.CURRENT_HEALTH].CurrentValue = stats[(int)StatStates.CURRENT_HEALTH].mMax;

        timerMgr = GameObject.FindGameObjectWithTag("GameController").GetComponent<TimerManager>();
        inputMgr = mPlayer.GetComponent<InputManager>();
        animMgr = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerAnimationManager>();
        gameplayMgr = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameplayManager>();

        isInvulnerable = gameplayMgr.isInvincible;
    }

    public void UpdateStatValue(StatStates s, float delta)
    {
        SetStatValue(s, stats[(int)s].CurrentValue + delta);
    }

    public void SetStatValue(StatStates s, float v)
    {
        Stat currentStat = stats[(int)s];

        currentStat.CurrentValue = v;
        Mathf.Clamp(currentStat.CurrentValue, currentStat.mMin, currentStat.mMax);

        switch (s)
        {
            case StatStates.MAX_HEALTH:
                Stat currHealth = stats[(int)StatStates.CURRENT_HEALTH];
                currHealth.mMax = currentStat.CurrentValue;

                // if I decrease the max health, maybe the current health is invalid now, fix with clamp
                Mathf.Clamp(currHealth.CurrentValue, currHealth.mMin, currHealth.mMax);
                break;
            case StatStates.CURRENT_HEALTH:
                if (stats[(int)s].CurrentValue <= 0)
                {
                    EventManager.TriggerEvent(Events.ON_MAIN_CHAR_DEATH, null);
                }
                break;

            case StatStates.TEMP_DISTORSION:
                timerMgr.setInterval((int)GetStatValue(StatStates.TEMP_DISTORSION));
                break;

            case StatStates.SPEED:
                inputMgr.SetStatSpeed((int)GetStatValue(StatStates.SPEED));
                break;

            case StatStates.DAMAGE:
                // you cannot set a different value to the prefab at runtime, only to their instances
                // therefore the values are set to the clones in the InputManager script.
                break;

            case StatStates.DEFENCE:
                break;

            default:
                Debug.Log("Trying to set invalid stat");
                break;
        }

        EventManager.TriggerEvent(Events.ON_STAT_CHANGED, null);
    }

    public float GetStatValue(StatStates s) { return stats[(int)s].CurrentValue; }

    private void DecreaseEnergy(Dictionary<string, string> args)
    {
        if (!IsInvulnerable)
        {
            EventManager.TriggerEvent(Events.ON_MAIN_CHAR_ACTUALLY_HIT, null);
            float defence = GetStatValue(StatStates.DEFENCE);

            // TODO USE ACTUAL ATTACK POINT ENEMY
            float amountHitPoints = Mathf.Floor(2 * (MAX_DEFENCE + 1 - defence)) / 2;

            UpdateStatValue(StatStates.CURRENT_HEALTH, -amountHitPoints);
            animMgr.OnMainCharHit(null);

            if (GetStatValue(StatStates.CURRENT_HEALTH) <= 0)
            {
                EventManager.TriggerEvent(Events.ON_MAIN_CHAR_DEATH, null);
            }
        }
    }

    public bool IsInvulnerable
    {
        get
        {
            return isInvulnerable;
        }

        set
        {
            isInvulnerable = value;
        }
    }

}