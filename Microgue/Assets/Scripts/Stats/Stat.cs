using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Stat {

    public readonly string mName;

    [HideInInspector]
    public float mMin, mMax;
    
    private float mCurrValue;
    Image mImg;

    public Stat( string name, float min, float max, Image image = null )
    {
        mName = name;
        mMin = min;
        mMax = max;
        mImg = image;

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
