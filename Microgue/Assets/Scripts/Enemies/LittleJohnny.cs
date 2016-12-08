using UnityEngine;
using System.Collections;

using Random = UnityEngine.Random;

public class LittleJohnny : MonoBehaviour {
    private GameObject mTarget;
    private Rigidbody2D mPlayerRb;
    private Rigidbody2D mRb;
    
    // Use this for initialization
    void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mTarget = GameObject.Find("MainCharacter");
        mPlayerRb = mTarget.GetComponent<Rigidbody2D>();
        
    }

    void FixedUpdate()
    {
        
    }

    void ChooseNewPoint() {
        
    }
}
