using UnityEngine;
using System.Collections;
using System;

public class Joypad : MonoBehaviour {

    private enum RStickState
    {
        IN_USE,
        RELEASED
    }

    Rigidbody2D rb;
    Camera mainCam;
    RStickState rStickState;

    Vector2 previousRStickpos;

    [Header("Parameters for speed")]
    public float speed = 100f;

    public Transform aimTransform;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();

        mainCam = Camera.main;
        SetPositionCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));

        // Right stick management
        previousRStickpos = Vector2.zero;
        rStickState = RStickState.IN_USE;
    }

    private void SetPositionCamera()
    {
        Vector2 charPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 currentRStickpos = new Vector2(Input.GetAxis("JRHorizontal"), -Input.GetAxis("JRVertical"));

        // check only if we release the stick from non zero position to zero position
        // in all other situations just move the cursor
        //if( currentRStickpos == Vector2.zero && previousRStickpos != Vector2.zero )
        //{
        //StartCoroutine(RestoreCamera());
        //} else if( rStickState == RStickState.IN_USE) {
        //     mainCam.transform.position =
        //        new Vector3(charPosition.x + currentRStickpos.x, charPosition.y + currentRStickpos.y, -1);
        //}

        mainCam.transform.position =
                new Vector3(charPosition.x + currentRStickpos.x, charPosition.y + currentRStickpos.y, -1);
        previousRStickpos = currentRStickpos;
    }

    //private IEnumerator RestoreCamera()
    //{
    //    rStickState = RStickState.RELEASED;

    //    rStickState = RStickState.IN_USE;
   // }

    // Update is called once per frame
    void Update () {
        
    }

    void FixedUpdate()
    {
        ChangeVelocity();
        
    }

    void LateUpdate()
    {
        SetPositionCamera();
        Aim();
    }

    private void Aim()
    {
        Vector2 joypadWP = mainCam.ScreenToWorldPoint(GetScreenJoypadCoordinates());
        aimTransform.transform.position = joypadWP;
    }

    private Vector3 GetScreenJoypadCoordinates()
    {
        float horR = Input.GetAxis("JRHorizontal");
        float verR = Input.GetAxis("JRVertical");
        
        // consider only 80% on the x and y axis of the screen
        float wClamp = 0.8f;
        float hClamp = 0.8f;

        // wClamp * Screen.width * (horR + 1) / 2 + (1-wClamp)*Screen.width/2;
        float screenHor = Screen.width * (1 + wClamp * horR) / 2;

        //hClamp * Screen.height * ( (verR * (-1)) + 1) / 2 + (1 - hClamp) * Screen.height/2;
        float screenVer = Screen.height * (-hClamp * verR + 1) / 2; 

        return new Vector3(screenHor, screenVer, 0);
    }

    private void ChangeVelocity()
    {
        Vector2 delta = new Vector2(0, 0);

        float hor = Input.GetAxis("JHorizontal");
        float ver = Input.GetAxis("JVertical");

        if (hor < -float.Epsilon)
            delta += Vector2.left;
        else if (hor > float.Epsilon)
            delta += Vector2.right;

        if (ver > float.Epsilon)
            delta += Vector2.down;
        else if (ver < -float.Epsilon)
            delta += Vector2.up;

        if (delta != Vector2.zero)
            delta.Normalize();

        rb.velocity = delta * speed * Time.fixedDeltaTime;
    }
}
