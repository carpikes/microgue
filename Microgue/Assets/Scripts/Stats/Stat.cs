using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat {

    public readonly string mName;
    public float mMin, mMax;
    
    private float mCurrValue;

    public Stat( string name, float min, float max )
    {
        mName = name;
        mMin = min;
        mMax = max;

        ResetToMin();
    }

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
