using UnityEngine;
using System.Collections;

public class ShotDamage : MonoBehaviour {

    public float damage;

    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }
}
