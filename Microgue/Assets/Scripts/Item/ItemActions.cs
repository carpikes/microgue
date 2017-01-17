using UnityEngine;
using System.Collections;

public class ItemActions {

    public static void Nothing(string param) {
        //Debug.Log("It works:" + param);
    }

    public static void BallColor(string color)
    {
        Color c = Color.white;
        color = color.ToLower();
        switch (color) {
            case "red": c = Color.red; break;
            case "blue": c = Color.blue; break;
            case "green": c = Color.green; break;
            default: return;
        }
        //Debug.Log("BallColor: " + color);
        GameObject.Find("MainCharacter").GetComponent<InputManager>().mBallColor = c;
    }
}
