using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public static Dictionary<string, List<string>> enemyDictionary = new Dictionary<string, List<string>>();

    // Use this for initialization
    void Start () {
        enemyDictionary.Clear();

        List<string> firstRoomEnemies = new List<string> { "DesperateSoul" };
        List<string> boss = new List<string> { "BigLittleJimmy" };
        List<string> easyEnemies = new List<string> { "AngrySoul", "BlueChasingBird", "DesperateSoul", "LittleJimmy" };
        List<string> difficultEnemies = new List<string> { "StompStomp", "BlueChasingBird" };
        List<string> healingAngel = new List<string> { "HealingAngel" };

        enemyDictionary.Add("Boss", boss);
        enemyDictionary.Add("FirstRoomEnemy", firstRoomEnemies);
        enemyDictionary.Add("EasyEnemy", easyEnemies);
        enemyDictionary.Add("DifficultEnemy", difficultEnemies);
        enemyDictionary.Add("HealingAngel", healingAngel);
    }
	

}
