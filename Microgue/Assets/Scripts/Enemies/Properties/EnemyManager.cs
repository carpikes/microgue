using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    public static Dictionary<string, List<string>> enemyDictionary = new Dictionary<string, List<string>>();

    // Use this for initialization
    void Start () {
        enemyDictionary.Clear();

        // --- SILENT WOODS ---
        List<string> firstRoomEnemies = new List<string> { "DesperateSoul" };
        enemyDictionary.Add("FirstRoomEnemy", firstRoomEnemies);

        List<string> easyEnemies = new List<string> { "AngrySoul", "BlueChasingBird", "DesperateSoul", "LittleJimmy" };
        enemyDictionary.Add("EasyEnemy", easyEnemies);

        List<string> difficultEnemies = new List<string> { "StompStomp", "BlueChasingBird" };
        enemyDictionary.Add("DifficultEnemy", difficultEnemies);

        // --- HOLY WATERS ---
        List<string> basic = new List<string> { "AngrySoul", "BlueChasingBird", "DesperateSoul", "LittleJimmy", "StompStomp", "BlueChasingBird" };
        enemyDictionary.Add("Basic", difficultEnemies);

        List<string> water = new List<string> { "WaterBlob", "Siren" };
        enemyDictionary.Add("Water", water);

        // --- FIRE OF THE DAMNED ---
        List<string> fire = new List<string> { "Fire Magician", "ExplodingStatue", "BlackChasingBird" };
        enemyDictionary.Add("Fire", fire);

        List<string> wanderingFlame = new List<string> { "WanderingFlame" };
        enemyDictionary.Add("Flame", wanderingFlame);

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
