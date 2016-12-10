using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Stat {

    public readonly string mName;

    [HideInInspector]
    public float mMin, mMax;
    
    private float mCurrValue;
    public bool showOnStatCanvas;
    // Image mImg;

    public Stat( string name, float min, float max, bool show)
    {
        mName = name;
        mMin = min;
        mMax = max;
        showOnStatCanvas = show;

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
