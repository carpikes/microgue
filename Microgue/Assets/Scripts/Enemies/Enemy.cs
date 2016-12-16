using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	void Start () {
        SetupEnemy();
	}

    void Update () {
	
	}

    protected abstract void SetupEnemy();
}
