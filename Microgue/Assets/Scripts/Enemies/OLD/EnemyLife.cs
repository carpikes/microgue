using UnityEngine;
using System.Collections;
using POLIMIGameCollective;
using System;

public class EnemyLife : MonoBehaviour {

    private GameObject mHealthBar;
    public float hp = 2;
    private float totalHp;

    private Animator mAnimator;

	// Use this for initialization
	void Start () {
        mHealthBar = transform.GetChild(0).gameObject;
        totalHp = hp;

        mAnimator = GetComponent<Animator>();

        if (mHealthBar != null)
            mHealthBar.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
        {
            EventManager.TriggerEvent(Events.ON_ENEMY_HIT, null);
            hp -= other.GetComponent<ShotDamage>().Damage;
            UpdateHPBar();
        }
        if (hp <= 0.0f)
        {
            //EventManager.TriggerEvent(Events.ON_ENEMY_DEATH, null);
            DeathAnimation();
        }
    }

    private void DeathAnimation()
    {
        mHealthBar.SetActive(false);
        mAnimator.SetTrigger("enemy_death");
        // destroy is invoked by animation thru an animation event
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void UpdateHPBar() {
        if (mHealthBar == null)
            return;

        mHealthBar.SetActive(true);

        float perc = hp / totalHp;
        float x_off = -0.29f * (1.0f - perc);

        Transform tr = mHealthBar.transform.GetChild(1);
        tr.localScale = new Vector3(perc,1,1);
        tr.localPosition = new Vector3(x_off, 0, 0);
    }
}
