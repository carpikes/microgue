using UnityEngine;

public class EnemyLife : MonoBehaviour {

    private GameObject mHealthBar;
    public float mCurrentHP;
    private float mTotalHP;

    private Animator mAnimator;

	// Use this for initialization
	void Start () {
        mHealthBar = transform.FindChild("HealthBar").gameObject;
        Debug.Assert(mHealthBar != null && mHealthBar.activeSelf, "No health bar for: " + gameObject.name);

        mTotalHP = mCurrentHP;

        mAnimator = GetComponent<Animator>();
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Shot"))
        {
            EventManager.TriggerEvent(Events.ON_ENEMY_HIT, null);
            mCurrentHP -= other.GetComponent<ShotProperties>().mDamage;
            UpdateHPBar();
        }

        if (mCurrentHP <= 0.0f)
            DeathAnimation();
    }

    private void DeathAnimation()
    {
        mHealthBar.SetActive(false);
        mAnimator.SetTrigger("enemy_death");
        // destroy is invoked by animation through an animation event
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void UpdateHPBar() {
        mHealthBar.SetActive(true);

        float percentage = mCurrentHP / mTotalHP;
        float xOffset = -0.29f * (1.0f - percentage);

        Transform tr = mHealthBar.transform.GetChild(1);
        tr.localScale = new Vector3(percentage,1,1);
        tr.localPosition = new Vector3(xOffset, 0, 0);
    }
}
