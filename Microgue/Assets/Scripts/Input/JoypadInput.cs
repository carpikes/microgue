using UnityEngine;
using System.Collections;
using System;

public class JoypadInput : MonoBehaviour, InputInterface {

    Rigidbody2D rb;
    Camera mainCam;

    // r stick management
    Vector2 previousRStick, droppedRStick;
    bool resettingRStick = false;
    float counter = 1f;

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

        previousRStick = new Vector2(0, 0);
        droppedRStick = new Vector2(0, 0);
    }

    public void LateUpdate()
    {
        SetPositionCamera();
    }

    private void SetPositionCamera()
    {
        Vector2 charPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 currentRStick = new Vector2(Input.GetAxis("JRHorizontal"), -Input.GetAxis("JRVertical"));

        float counterStep = 0.05f;

        // if the player released the stick...
        // "zero" because of the dead zone set in the input manager!
        if (previousRStick != Vector2.zero && currentRStick == Vector2.zero)
        {
            // reset counter and save info about previous stick position
            counter = 1f;
            resettingRStick = true;
            droppedRStick = previousRStick;
        } else if (currentRStick != Vector2.zero)
        {
            // the player gets control back again
            resettingRStick = false;
        }

        // if we are in releasing phase...
        if( resettingRStick )
        {
            // and we still have to move...
            if (counter >= 0.0f)
            {
                // interpolate position by percentage of saved stick position
                mainCam.transform.position =
                    new Vector3(charPosition.x + (droppedRStick.x)*counter,
                                charPosition.y + (droppedRStick.y)*counter, -1);

                counter -= counterStep;
            } else
            {
                // we arrived at the char, stop interpolating
                resettingRStick = false;
            }
        } else
        {
            // the player has control: do whatever we wants
            mainCam.transform.position =
                new Vector3(charPosition.x + currentRStick.x, charPosition.y + currentRStick.y, -1);
        }

        // retain previous stick position
        previousRStick = currentRStick;
    }

    public Vector2 GetScreenPointerCoordinates()
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

        return new Vector2(screenHor, screenVer);
    }

    public Vector2 GetVelocityDelta()
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

        return delta;
    }

    public bool IsShootingButtonPressed()
    {
        throw new NotImplementedException();
    }
}
