using UnityEngine;
using System.Collections;

public class StompStompShadow : MonoBehaviour {
    StompStomp parent;

    void Start()
    {
        parent = GetComponentInParent<StompStomp>();
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag("Player"))
            parent.OnShadowTouch();
    }
}
