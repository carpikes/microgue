using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat {

    public string m_name;
    public float m_min;
    public float m_max;
    
    private float m_currValue;

    public float CurrentValue
    {
        get
        {
            return m_currValue;
        }

        set
        {
            if (value >= m_max)
                m_currValue = m_max;
            else if (value <= m_min)
                m_currValue = m_min;
            else
                m_currValue = value;
        }
    }

    public void ResetToMin()
    {
        m_currValue = m_min;
    }
}
