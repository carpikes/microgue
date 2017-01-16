using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class CanvasManager : MonoBehaviour {

    public static string SECONDARY_ATTACK_BAR = "SECONDARY_ATTACK_BAR";

    public Canvas uiCanvas;
    public Text timerText;
    public Text additionalInfoText;

    [Header("Health UI")]
    public Transform healthContainer;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    [Header("Stat UI")]
    public Text defText;
    public Text dmgText;
    public Text timeText;
    public Text spdText;

    [Header("Secondary Attack Bar")]
    public Image barGameObject;
    public Sprite notFullBarImage;
    public Sprite fullBarImage;

    public GameObject mainCharacter;
    StatManager playerStats;

    Coroutine lastTextCoroutine = null;

    // Use this for initialization
    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("GameController").GetComponent<StatManager>();

        UpdateStatText();
        UpdateHealth();
        additionalInfoText.text = "";
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.ON_STAT_CHANGED, OnStatChanged);
        EventManager.StartListening(Events.ON_TICK, OnTick);
        EventManager.StartListening(Events.ON_ITEM_PICKUP, OnItemPickup);
        EventManager.StartListening(Events.ON_SHOW_MESSAGE, OnShowMessage);
        EventManager.StartListening(Events.ON_STILL_ENEMIES_LEFT, OnStillEnemiesLeft);
        EventManager.StartListening(Events.UPDATE_SECONDARY_ATTACK, SecondaryAttackUpdate);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_STAT_CHANGED, OnStatChanged);
        EventManager.StopListening(Events.ON_TICK, OnTick);
        EventManager.StopListening(Events.ON_ITEM_PICKUP, OnItemPickup);
        EventManager.StopListening(Events.ON_SHOW_MESSAGE, OnShowMessage);
        EventManager.StopListening(Events.ON_STILL_ENEMIES_LEFT, OnStillEnemiesLeft);
        EventManager.StopListening(Events.UPDATE_SECONDARY_ATTACK, SecondaryAttackUpdate);
    }

    private void SecondaryAttackUpdate(Bundle args)
    {
        string v;
        args.TryGetValue(SECONDARY_ATTACK_BAR, out v);

        float value = float.Parse(v);

        barGameObject.fillAmount = value;
        if( value >= 0.999 )
        {
            barGameObject.sprite = fullBarImage;
        } else
        {
            barGameObject.sprite = notFullBarImage;
        }
    }

    private void OnStillEnemiesLeft(Bundle args)
    {
        if (lastTextCoroutine != null)
            StopCoroutine(lastTextCoroutine);

        lastTextCoroutine = StartCoroutine(StillEnemiesCoroutine(args));
    }

    private IEnumerator StillEnemiesCoroutine(object args)
    {
        additionalInfoText.text = "You have to kill all enemies first!";
        yield return new WaitForSeconds(2f);

        additionalInfoText.text = "";
    }

    private void OnShowMessage(Bundle args)
    {
        if (lastTextCoroutine != null)
            StopCoroutine(lastTextCoroutine);

        lastTextCoroutine = StartCoroutine( ShowMessageCoroutine(args) );
    }

    private IEnumerator ShowMessageCoroutine(Bundle args)
    {
        string text;
        if (args.TryGetValue("text", out text))
            additionalInfoText.text = text;

        yield return new WaitForSeconds(5f);

        additionalInfoText.text = "";
    }

    private void OnItemPickup(Bundle args)
    {
        if (lastTextCoroutine != null)
            StopCoroutine(lastTextCoroutine);

        lastTextCoroutine = StartCoroutine( ItemPickupCoroutine(args) );
    }

    private IEnumerator ItemPickupCoroutine(Bundle args)
    {
        string itemName;
        if (args.TryGetValue(ItemPrefabProperties.ITEM_PICKUP_TAG, out itemName))
        {
            additionalInfoText.text = "You picked up: " + itemName.ToUpper();
        }

        yield return new WaitForSeconds(2f);

        additionalInfoText.text = "";
    }

    private void OnTick(Bundle args)
    {
        string ticksLeft;
        if( args.TryGetValue(TimerManager.TICKS_LEFT_TAG, out ticksLeft) )
        {
            int intTicks = int.Parse(ticksLeft);
            int minutes = intTicks / 60;
            int seconds = intTicks % 60;

            timerText.text = "TIME LEFT: " + String.Format("{0:00}", minutes) + ":" + String.Format("{0:00}", seconds);
        }
    }

    private void OnStatChanged(Bundle args)
    {
        UpdateStatText();
        UpdateHealth();
    }

    private void UpdateStatText()
    {
        ShowPlayerStats();
    }

    private void UpdateHealth()
    {
        float currentHealth = playerStats.GetStatValue(StatManager.StatStates.CURRENT_HEALTH);
        float maxHealth = playerStats.GetStatValue(StatManager.StatStates.MAX_HEALTH);

        int i;
        for ( i = 0; i < Mathf.Floor(currentHealth); ++i )
            SetHeartImage(i, fullHeart);

        if ( Mathf.Abs( currentHealth - (int)currentHealth - 0.5f) < 0.0001f )
            SetHeartImage(i++, halfHeart);

        for (; i < Mathf.Floor(maxHealth); ++i)
            SetHeartImage(i, emptyHeart);

        for(; i < 10; ++i)
            healthContainer.GetChild(i).gameObject.SetActive(false);
    }

    private void SetHeartImage(int i, Sprite s)
    {
        GameObject go = healthContainer.GetChild(i).gameObject;
        go.GetComponent<Image>().sprite = s;
        go.SetActive(true);
    }

    private void ShowPlayerStats()
    {
        if ( playerStats )
        {
            foreach( Stat s in playerStats.stats )
            {
                if( s.mName == "DEFENCE" )
                {
                    defText.text = s.CurrentValue.ToString();
                } else if( s.mName == "DAMAGE" )
                {
                    dmgText.text = s.CurrentValue.ToString();
                } else if( s.mName == "TEMP_DISTORSION" )
                {
                    timeText.text = s.CurrentValue.ToString();
                } else if( s.mName == "SPEED" )
                {
                    spdText.text = s.CurrentValue.ToString();
                } else if( s.mName == "CURRENT_HEALTH" || s.mName == "MAX_HEALTH" )
                {
                    continue;
                } else
                {
                    Debug.LogError("UPDATING NON-EXISTENT STAT");
                }
            }
        } else
        {
            Debug.LogError("Cannot retrieve player stats component");
        }
    }
}
