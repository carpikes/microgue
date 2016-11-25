using UnityEngine;
using System.Collections;

public class Stat {

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
            m_currValue = value;
        }
    }

    public Stat(float v) {
        m_currValue = v;
    }

    public Stat()
    {
        m_currValue = m_min;
    }
}
