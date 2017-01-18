using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Bundle = System.Collections.Generic.Dictionary<string, string>;

public class LoadingScreenManager : MonoBehaviour {
    public GameObject mCanvas = null;
    public GameObject mText = null;
	// Use this for initialization
	void Start () {
        if (mCanvas == null)
            Debug.LogError("Can't find my /Canvas/LoadingCanvas");
        mCanvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable() {
        EventManager.StartListening(Events.ON_LEVEL_BEFORE_LOADING, BeforeLoading);
        EventManager.StartListening(Events.ON_LEVEL_AFTER_LOADING, AfterLoading);
    }

    void OnDisable() {
        EventManager.StopListening(Events.ON_LEVEL_BEFORE_LOADING, BeforeLoading);
        EventManager.StopListening(Events.ON_LEVEL_AFTER_LOADING, AfterLoading);
    }

    IEnumerator WaitCoroutine() {
        yield return new WaitForSeconds(1.1f);
        EventManager.TriggerEvent(Events.ON_LOADING_SCREEN_COMPLETE, null);
        EventManager.TriggerEvent(Events.FADE_OUT, null);
        yield return new WaitForSeconds(0.5f);
        mCanvas.SetActive(false);
        EventManager.TriggerEvent(Events.FADE_IN, null);
    }

    void BeforeLoading(Bundle arg) {
        mText.GetComponent<Text>().text = arg["Name"];
        mCanvas.SetActive(true);
    }

    void AfterLoading(Bundle arg) {
        StartCoroutine(WaitCoroutine());
    }
}
