using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class CanvasManager : MonoBehaviour {

    public Canvas uiCanvas;
    public Text statText;
    public Text timerText;
    public Text additionalInfoText;

    [Header("Health UI")]
    public Transform healthContainer;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    GameObject mainCharacter;
    StatManager playerStats;

    Coroutine lastTextCoroutine = null;

    // Use this for initialization
    void Start()
    {
        mainCharacter = GameObject.FindGameObjectWithTag("Player");
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
        EventManager.StartListening(Events.ON_STILL_ENEMIES_LEFT, OnStillEnemiesLeft);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ON_STAT_CHANGED, OnStatChanged);
        EventManager.StopListening(Events.ON_TICK, OnTick);
        EventManager.StopListening(Events.ON_ITEM_PICKUP, OnItemPickup);
        EventManager.StopListening(Events.ON_STILL_ENEMIES_LEFT, OnStillEnemiesLeft);
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
        statText.text = "";
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
                if (s.showOnStatCanvas)
                    statText.text += s.mName + ": " + s.CurrentValue + "\n";
            }
        } else
        {
            Debug.LogError("Cannot retrieve player stats component");
        }
    }
}
