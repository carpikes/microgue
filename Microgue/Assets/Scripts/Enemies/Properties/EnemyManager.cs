using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public static Dictionary<string, List<string>> enemyDictionary = new Dictionary<string, List<string>>();

    // Use this for initialization
    void Start () {
        enemyDictionary.Clear();

        List<string> firstRoomEnemies = new List<string> { "WaterBlob" };
        enemyDictionary.Add("FirstRoomEnemy", firstRoomEnemies);

        List<string> easyEnemies = new List<string> { "AngrySoul", "BlueChasingBird", "DesperateSoul", "LittleJimmy" };
        enemyDictionary.Add("EasyEnemy", easyEnemies);

        List<string> difficultEnemies = new List<string> { "StompStomp", "BlueChasingBird" };
        enemyDictionary.Add("DifficultEnemy", difficultEnemies);

        // --- BOSS ---
        // silent woods
        List<string> silentWoodsBoss = new List<string> { "BigLittleJimmy" };
        enemyDictionary.Add("Boss", silentWoodsBoss);

        // holy waters
        List<string> waterBoss = new List<string> { "Medusa" };
        enemyDictionary.Add("WaterBoss", waterBoss);

        // fire of the damned
        List<string> fireBoss = new List<string> { "FireTaurus" };
        enemyDictionary.Add("FireBoss", fireBoss);
    }
	

}
