using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class VideoExecution : MonoBehaviour {

    public MovieTexture video;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

	// Use this for initialization
	void OnGUI () {

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), video, ScaleMode.StretchToFill);
        video.Play();

        StartCoroutine(OnEndMovie());
    }

    private IEnumerator OnEndMovie()
    {
        yield return new WaitForSeconds(video.duration + 1f);

        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if(Input.GetButtonDown("Escape"))
        {
            SceneManager.LoadScene("Menu");
        }
    }

}
