using UnityEngine;
using System.Collections;
using System;

public class PlayerScript : MonoBehaviour {

    Rigidbody2D rb;
    Animator animator;
    Camera mainCam;

    public Transform aimTransform;

    [Header("Parameters for speed")]
    public float speed = 100f;

    [Header("Limit area for mouse")]
    public float xPercentage = 0.05f;
    public float yPercentage = 0.05f;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        mainCam = Camera.main;
        CenterCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));

    }

    private void CenterCamera()
    {
        Vector2 charPosition = new Vector2(transform.position.x, transform.position.y);

        Vector2 mouseSP = GetScreenMouseCoordinates();
        Vector2 mouseNormalized = 
            new Vector2((mouseSP.x / Screen.width) * 2 - 1, (mouseSP.y / Screen.height) * 2 - 1);

        Debug.Log(mouseNormalized);

        mouseNormalized *= 1;

        mainCam.transform.position = new Vector3(
            transform.position.x + mouseNormalized.x, transform.position.y + mouseNormalized.y, -1);
    }

    void LateUpdate()
    {
        CenterCamera();
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        ChangeVelocity();
        Aim();
    }

    private void Aim()
    {
        Vector2 mouseWP = mainCam.ScreenToWorldPoint(GetScreenMouseCoordinates());
        aimTransform.transform.position = mouseWP;
    }

    private Vector2 GetScreenMouseCoordinates()
    {
        float w = Screen.width;
        float h = Screen.height;

        Rect area = new Rect(xPercentage * w, yPercentage * h, (1 - 2 * xPercentage) * w, (1 - 2 * yPercentage) * h);

        Vector2 mouseSP = clampRectangle(area, Input.mousePosition);

        return mouseSP;
    }

    private void ChangeVelocity()
    {
        Vector2 delta = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
            delta += Vector2.left;

        if (Input.GetKey(KeyCode.D))
            delta += Vector2.right;

        if (Input.GetKey(KeyCode.W))
            delta += Vector2.up;

        if (Input.GetKey(KeyCode.S))
            delta += Vector2.down;

        if (delta != Vector2.zero)
            delta.Normalize();

        rb.velocity = delta * speed * Time.fixedDeltaTime;
    }

    private Vector2 clampRectangle( Rect r, Vector2 mousePP )
    {
        Vector2 res = mousePP;

        res.x = Mathf.Clamp(res.x, r.xMin, r.xMax);
        res.y = Mathf.Clamp(res.y, r.yMin, r.yMax);

        return res;
    }

}
