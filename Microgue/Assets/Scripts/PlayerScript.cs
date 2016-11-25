using UnityEngine;
using System.Collections;
using System;

public class PlayerScript : MonoBehaviour {

    Rigidbody2D rb;
    Animator animator;
    Camera mainCam;
    private GameplayManager mGameManager = null;

    float oldAnimationDirX = 0.0f;

    public Transform aimTransform;

    [Header("Parameters for speed")]
    public float speed = 100f;

    [Header("Limit area for mouse")]
    public float xPercentage = 0.05f;
    public float yPercentage = 0.05f;

    [Header("Shots parameters")]
    public GameObject lightBall;
    public float shotCooldownTime = 0.2f;
    public float shotSpeed = 5f;
    float lastShootTime = 0f;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        mainCam = Camera.main;
        SetPositionCamera();

        // set cursor to player position
        aimTransform.position = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 1));
    }

    private void SetPositionCamera()
    {
        Vector2 mouseNormalized = GetNormalizedMouseCoordinates();

        SetPlayerAnimation(mouseNormalized);

        // se vuoi mousenormalized *= cost fallo qui e non sopra setplayeranimation

        Vector3 cameraPos = new Vector3(
            transform.position.x + mouseNormalized.x, transform.position.y + mouseNormalized.y, -1);

        if (mGameManager == null)
        {
            GameObject gm = GameObject.Find("GameplayManager");
            mGameManager = gm.GetComponent<GameplayManager>();
        }
        Vector3[] cameraBound = mGameManager.GetCameraBounds();
        if (cameraBound != null)
        {
            cameraPos.x = Mathf.Clamp(cameraPos.x, cameraBound[0].x, cameraBound[1].x);
            cameraPos.y = Mathf.Clamp(cameraPos.y, cameraBound[0].y, cameraBound[1].y);
        }
        mainCam.transform.position = cameraPos;
    }

    private Vector2 GetNormalizedMouseCoordinates()
    {
        Vector2 mouseSP = GetScreenMouseCoordinates();
        Vector2 mouseNormalized =
            new Vector2((mouseSP.x / Screen.width) * 2 - 1, (mouseSP.y / Screen.height) * 2 - 1);
        return mouseNormalized;
    }

    private void SetPlayerAnimation( Vector2 dir )
    {
        // change of direction
        if (oldAnimationDirX * dir.x <= 0)
        {
            animator.SetFloat("dir_x", dir.x);
            oldAnimationDirX = dir.x;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime > shotCooldownTime)
        {
            GameObject lb = Instantiate(lightBall);
            lb.transform.position = transform.position;

            Vector2 playerPos = transform.position;
            Vector2 mouse = mainCam.ScreenToWorldPoint(GetScreenMouseCoordinates());
            Vector2 direction = (mouse - playerPos).normalized;
            ((Rigidbody2D)lb.GetComponent<Rigidbody2D>()).velocity = direction * shotSpeed;

            lastShootTime = Time.time;
        }
    }

    void LateUpdate()
    {
        SetPositionCamera();
        Aim();
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        ChangeVelocity();
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

        Vector2 mouseSP = ClampRectangle(area, Input.mousePosition);

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

    private Vector2 ClampRectangle( Rect r, Vector2 mousePP )
    {
        Vector2 res = mousePP;

        res.x = Mathf.Clamp(res.x, r.xMin, r.xMax);
        res.y = Mathf.Clamp(res.y, r.yMin, r.yMax);

        return res;
    }

}
