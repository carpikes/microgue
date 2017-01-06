using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public static Dictionary<string, List<string>> enemyDictionary = new Dictionary<string, List<string>>();

    // Use this for initialization
    void Start () {
        enemyDictionary.Clear();

        List<string> firstRoomEnemies = new List<string> { "DesperateSoul" };
        List<string> easyEnemies = new List<string> { "AngrySoul", "BlueChasingBird", "DesperateSoul", "LittleJimmy" };
        List<string> difficultEnemies = new List<string> { "StompStomp", "BlueChasingBird" };
        List<string> healingAngel = new List<string> { "Siren" };

        // boss

        // silent woods
        List<string> boss = new List<string> { "BigLittleJimmy" };

        // fire of the damned
        List<string> firefox = new List<string> { "AngrySoul" };

        enemyDictionary.Add("Boss", boss);
        enemyDictionary.Add("FireFox", firefox);
        enemyDictionary.Add("FirstRoomEnemy", firstRoomEnemies);
        enemyDictionary.Add("EasyEnemy", easyEnemies);
        enemyDictionary.Add("DifficultEnemy", difficultEnemies);
        enemyDictionary.Add("HealingAngel", healingAngel);
    }
	

}
