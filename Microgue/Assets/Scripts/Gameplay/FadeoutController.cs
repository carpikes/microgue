using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Risponde agli eventi FADE_OUT e FADE_IN
 * facendo la corrispettiva azione.
 * Ogni effetto ci mette 0.1 secondi a completarsi
 */
public class FadeoutController : MonoBehaviour {
    private GameObject mScreen;
    private RawImage mRawImage;

    // Use this for initialization
    void Start() {
        mScreen = GameObject.Find("Canvas/FadeTransitionCanvas/BlackScreen");
        mRawImage = mScreen.GetComponent<RawImage>();
        mRawImage.rectTransform.sizeDelta = new Vector2(Camera.main.pixelWidth + 20, Camera.main.pixelHeight + 20);
        mRawImage.color = new Color(0, 0, 0, 0);
        mScreen.SetActive(false);
    }

    void OnEnable() {
        EventManager.StartListening(Events.FADE_OUT, FadeOut);
        EventManager.StartListening(Events.FADE_IN, FadeIn);
	}

    void OnDisable() {
        EventManager.StopListening(Events.FADE_OUT, FadeOut);
        EventManager.StopListening(Events.FADE_IN, FadeIn);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FadeOut(Dictionary<string,string> b)
    {
        StartCoroutine(FadeOutCoroutine());
    }

    void FadeIn(Dictionary<string,string> b)
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeOutCoroutine() {
        mRawImage.color = new Color(0, 0, 0, 0);
        mScreen.SetActive(true);
        for (float i = 0.0f; i <= 1.0f; i += 0.1f)
        {
            mRawImage.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        mRawImage.color = new Color(0, 0, 0, 1);
    }

    IEnumerator FadeInCoroutine() {
        for (float i = 0.0f; i <= 1.0f; i += 0.1f)
        {
            mRawImage.color = new Color(0, 0, 0, 1.0f - i);
            yield return new WaitForSeconds(0.01f);
        }
        mRawImage.color = new Color(0, 0, 0, 0);
        mScreen.SetActive(false);
    }

}
