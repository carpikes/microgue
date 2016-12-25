using System;
using UnityEngine;

public class KeyboardInput : MonoBehaviour, InputInterface {

    [Header("Limit area for mouse")]
    public float xPercentage = 0.05f;
    public float yPercentage = 0.05f;

    public Vector2 GetScreenPointerCoordinates()
    {
        float w = Screen.width;
        float h = Screen.height;

        Rect area = new Rect(xPercentage * w, yPercentage * h, (1 - 2 * xPercentage) * w, (1 - 2 * yPercentage) * h);

        Vector2 mouseSP = ClampRectangle(area, Input.mousePosition);

        return mouseSP;
    }

    private Vector2 ClampRectangle(Rect r, Vector2 mousePP)
    {
        Vector2 res = mousePP;

        res.x = Mathf.Clamp(res.x, r.xMin, r.xMax);
        res.y = Mathf.Clamp(res.y, r.yMin, r.yMax);

        return res;
    }

    public Vector2 GetVelocityDelta()
    {
        Vector2 delta = new Vector2(0, 0);

        if (Input.GetAxisRaw("Horizontal") < -0.01)
            delta += Vector2.left;

        if (Input.GetAxisRaw("Horizontal") > 0.01)
            delta += Vector2.right;

        if (Input.GetAxisRaw("Vertical") > 0.01)
            delta += Vector2.up;

        if (Input.GetAxisRaw("Vertical") < -0.01)
            delta += Vector2.down;

        if (delta != Vector2.zero)
            delta.Normalize();

        return delta;
    }

    public bool IsShootingButtonPressed()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }

    public bool IsItemButtonPressed()
    {
        return Input.GetButtonDown("Item");
    }

    public bool IsDashButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    public bool IsSecondaryAttackButtonPressed()
    {
        return Input.GetButton("Shoot 2");
    }

    public void FeedPlayerPosition(Vector2 pos)
    {
        // not used here
    }
}
