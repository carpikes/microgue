using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat {

    public string mName;
    public float mMin, mMax;
    
    private float mCurrValue;

    public float CurrentValue
    {
        get
        {
            return mCurrValue;
        }

        set
        {
            mCurrValue = Mathf.Clamp(value, mMin, mMax);
        }
    }

    public void ResetToMin()
    {
        mCurrValue = mMin;
    }
}
