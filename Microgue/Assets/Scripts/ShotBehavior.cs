using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour
{
    private float mEndTime = 0.0f;
    private float mShotDuration = 0.0f;
    private SpriteRenderer mSprite;
    // Use this for initialization
    void Start()
    {
        mSprite = GetComponent<SpriteRenderer>();
        mShotDuration = Random.Range(0.8f, 0.9f);
        mEndTime = Time.time + mShotDuration;
    }

    // Update is called once per frame
    void Update()
    {
        float rem = (mEndTime - Time.time) / mShotDuration;

        if (mSprite != null) {
            Color c = mSprite.color;
            c.a = Mathf.Clamp(Mathf.Pow(rem * 2,4), 0.0f, 1.0f);
            mSprite.color = c;
        } 

        if (rem <= 0.0f)
            Destroy(gameObject);
    }
}
