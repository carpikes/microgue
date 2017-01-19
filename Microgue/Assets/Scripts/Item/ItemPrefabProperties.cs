using UnityEngine;
using System.Collections;

using StatPair = System.Collections.Generic.KeyValuePair<StatManager.StatStates, float>;
using Bundle = System.Collections.Generic.Dictionary<string, string>;
using System;

public class ItemPrefabProperties : MonoBehaviour {

    PlayerItemHandler playerManager;

    AudioSource audioSource;
    public AudioClip itemClip;

    [HideInInspector]
    public ItemData item;

    Rigidbody2D rb;
    Vector2 initialPos;
    Vector2 currPos;

    public float sineAmplitude = 0.5f;
    public float sineFrequency = 5f;

    Transform shadow;
    Vector2 shadowInitialPos;

    public static readonly string ITEM_PICKUP_TAG = "ITEM_PICKUP";

    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerItemHandler>();
        rb = GetComponent<Rigidbody2D>();
        initialPos = rb.position;
        currPos = initialPos;

        shadow = transform.FindChild("Shadow");
        shadowInitialPos = shadow.transform.position;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = itemClip;
    }

    void FixedUpdate ()
    {
        currPos.y = sineAmplitude * Mathf.Sin(Time.time) / sineFrequency + initialPos.y;

        rb.MovePosition(currPos);
        shadow.transform.position = shadowInitialPos;
    }

	void OnTriggerEnter2D( Collider2D other )
    {
        if (other.CompareTag("Player"))
        {
            Bundle itemBundle = new Bundle();
            itemBundle.Add(ITEM_PICKUP_TAG, item.Name);

            StartCoroutine(EndItem(itemBundle));
        }
    }

    private IEnumerator EndItem(Bundle itemBundle)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();

        foreach (MonoBehaviour mb in GetComponents<MonoBehaviour>())
            if (mb != this)
                mb.enabled = false;

        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);

        EventManager.TriggerEvent(Events.ON_ITEM_PICKUP, itemBundle);

        if (item.IsPassive)
            playerManager.UseItem(item);
        else
            playerManager.StoreItem(item);

        yield return new WaitForSeconds(itemClip.length);

        gameObject.SetActive(false);
    }
}
