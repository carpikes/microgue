using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public static Dictionary<string, List<string>> enemyDictionary = new Dictionary<string, List<string>>();

    // Use this for initialization
    void Start () {
        enemyDictionary.Clear();

        List<string> firstRoomEnemies = new List<string> { "DesperateSoul" };
        List<string> easyEnemies = new List<string> { "AngrySoul", "ChasingBird", "DesperateSoul", "LittleJimmy" };
        List<string> difficultEnemies = new List<string> { "StompStomp", "ChasingBird" };

        enemyDictionary.Add("FirstRoomEnemy", firstRoomEnemies);
        enemyDictionary.Add("EasyEnemy", easyEnemies);
        enemyDictionary.Add("DifficultEnemy", difficultEnemies);
    }
	

}
