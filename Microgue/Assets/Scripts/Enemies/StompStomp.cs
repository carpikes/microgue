using UnityEngine;
using System.Collections;
using DG.Tweening;

public class StompStomp : MonoBehaviour {

    Rigidbody2D rb;
    Collider2D coll;
    Transform playerTransform;

    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine("JumpCoroutine");
	}

    IEnumerator JumpCoroutine()
    {
        Sequence seq;

        while (true)
        {
            seq = rb.DOJump(playerTransform.position, 20f, 1, 2, false);
            yield return seq.WaitForStart();
            Debug.Log("Jump started");
            coll.enabled = false;
            sr.color = Color.magenta;
            yield return seq.WaitForCompletion();
            coll.enabled = true;
            sr.color = Color.white;

            // This log will happen after the tween has completed
            Debug.Log("Jump completed");
            yield return new WaitForSeconds(1);
        }

    }

    // Update is called once per frame
    /*void Update () {
        Vector3 a = gameObject.transform.position;
        float theta = Time.time * 7.0f;
        a.y = -5.0f + Mathf.Pow(Mathf.Abs(Mathf.Cos(theta)),1.5f) / 0.5f;
        a.z = 10 * Mathf.Abs(Mathf.Cos(theta));
        gameObject.transform.position = a;
	}*/

    void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Stomp hit " + other.name + " " + Time.time + ": " + gameObject.transform.position);
    }
        /*Vector3 a = gameObject.transform.position;
        if (a.z < 3.0f)
        {
            Debug.Log("Stomp hit " + other.name + " " + Time.time + ": " + gameObject.transform.position);
        }
    }*/


    }