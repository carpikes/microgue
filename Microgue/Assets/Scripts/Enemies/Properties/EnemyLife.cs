using System;
using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour {

    private GameObject mHealthBar;
    public float mCurrentHP;
    private float mTotalHP;

    private Animator mAnimator;

    public bool mIsInvincible = false;
    bool canDieAnimation = true;
    public AudioClip mDeathAudio = null;
    public AudioClip mHitSound = null;
    private float mKillTimeout = 0.0f;

    Collider2D[] colliders;

	// Use this for initialization
	void Start () {
        if (!mIsInvincible)
        {
            mHealthBar = transform.FindChild("HealthBar").gameObject;
            Debug.Assert(mHealthBar != null && mHealthBar.activeSelf, "No health bar for: " + gameObject.name);
        }

        mTotalHP = mCurrentHP;

        mAnimator = GetComponent<Animator>();

        colliders = GetComponents<Collider2D>();
	}

    public float GetTotalHP() { return mTotalHP; }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot") && !mIsInvincible)
        {
            AudioSource src = GetComponent<AudioSource>();
            if(src != null && mHitSound != null)
                src.PlayOneShot(mHitSound);
            Damage(other.GetComponent<ShotProperties>().mDamage);
        }
    }

    public void Damage(float howMuch)
    {
        EventManager.TriggerEvent(Events.ON_ENEMY_HIT, null);
        mCurrentHP -= howMuch;
        UpdateHPBar();

        if (mCurrentHP <= 0.0f)
            DeathAnimation();
        else
            StartCoroutine(FlashAnimation());
    }

    private IEnumerator FlashAnimation()
    {
        SpriteRenderer renderer = GetComponentInParent<SpriteRenderer>();
        Color color = renderer.color;

        Debug.Assert(renderer != null, "Sprite renderer not present");
        renderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        renderer.color = color;
    }

    public void InstaKill()
    {
        Damage(mCurrentHP);
    }

    IEnumerator DieCoroutine() {
        Debug.Log("Started..");
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Finished");
        Die();
    }

    private void DeathAnimation()
    {
        if (canDieAnimation)
        {
            canDieAnimation = false;
            mHealthBar.SetActive(false);

            foreach (var c in colliders)
                c.enabled = false;

            mAnimator.SetTrigger("enemy_death");

            // audio of death
            if (mDeathAudio != null && GetComponent<AudioSource>() != null)
                GetComponent<AudioSource>().PlayOneShot(mDeathAudio);
        }
    }

    public void Die()
    {
        EventManager.TriggerEvent(Events.ON_ENEMY_DEATH, null);

        foreach (MonoBehaviour mb in GetComponents<MonoBehaviour>())
            if (mb != this)
                mb.enabled = false;

        // disable sprite renderer
        GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(DeathAudioWait());
    }

    private IEnumerator DeathAudioWait()
    {
        if( mDeathAudio != null )
            yield return new WaitForSeconds(mDeathAudio.length);

        Destroy(gameObject);
    }

    void UpdateHPBar() {
        if( !mIsInvincible )
        {
            mHealthBar.SetActive(true);

            float percentage = mCurrentHP / mTotalHP;
            float xOffset = -0.29f * (1.0f - percentage);

            Transform tr = mHealthBar.transform.GetChild(1);
            tr.localScale = new Vector3(percentage, 1, 1);
            tr.localPosition = new Vector3(xOffset, 0, 0);
        }
    }
}
