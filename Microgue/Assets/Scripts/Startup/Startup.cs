using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour {

    public Image polimiSplash;
    public Image developerSplash;

    public readonly static float TIME_STILL = 3f; // seconds
    public readonly static float TIME_BETWEEN_FRAMES = .03f;
    public readonly static float TIME_BETWEEN_SCREENS = 1f;

    public bool isDebug = false;

    // Use this for initialization
    void Start () {
        polimiSplash.enabled = false;
        developerSplash.enabled = false;

        StartCoroutine(LoadSplash());
    }

    private IEnumerator LoadSplash( )
    {
        if (!isDebug)
        {
            yield return Load(polimiSplash);
            yield return new WaitForSeconds(TIME_BETWEEN_SCREENS);

            yield return Load(developerSplash);
            yield return new WaitForSeconds(TIME_BETWEEN_SCREENS);
        }

        SceneManager.LoadScene("Menu");
    }

    private static void ChangeTransparency(Image img, float alpha)
    {
        UnityEngine.Color col = img.color;
        col.a = alpha;
        img.color = col;
    }

    private IEnumerator Load(Image img)
    {
        img.enabled = true;

        for (float alpha = 0.0f; alpha <= 1.0f; alpha += 0.1f)
        {
            ChangeTransparency(img, alpha);
            yield return new WaitForSeconds(TIME_BETWEEN_FRAMES);
        }

        yield return new WaitForSeconds(TIME_STILL);

        for (float alpha = 1.0f; alpha >= 0.0f; alpha -= 0.1f)
        {
            ChangeTransparency(img, alpha);
            yield return new WaitForSeconds(TIME_BETWEEN_FRAMES);
        }

        img.enabled = false;
    }

}
