using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroManager : MonoBehaviour
{

    public Map.PrefabNamePair[] heroPrefabs;
    public Map.PrefabNamePair[] monsterPrefab;
    private Dictionary<string, GameObject> monsterPrefabDict; 
    private Dictionary<string, GameObject> heroPrefabDict; 

    public Map GameMap;
     

	// Use this for initialization
	void Awake () {
	
        heroPrefabDict = new Dictionary<string, GameObject>();

	    foreach (var pair in heroPrefabs)
	    {
	        heroPrefabDict.Add(pair.Name.ToUpper(), pair.Prefab);
	    }

        monsterPrefabDict = new Dictionary<string, GameObject>();

	    foreach (var pair in monsterPrefab)
	    {
	        monsterPrefabDict.Add(pair.Name.ToUpper(), pair.Prefab);
	    }
	}

    public void InstantiateHero(string heroID, string ownerID, string controllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement, int ap, Weapon weapon, bool blocksMovement)
    {
        Debug.Log("Instantiating a hero");
        string heroType = heroID.ToUpper().Trim();
        if (!heroPrefabDict.ContainsKey(heroType))
        {
            Debug.Log("Hero type not found " + heroID);
            return;
        }

        GameObject prefab = heroPrefabDict[heroType];
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            instance.name = ownerID + " " + heroType;
            var hero = instance.GetComponent<Hero>();
            if (hero == null)
            {
                Debug.Log("Joe WTF? Where's the prefab?");
                Destroy(instance);
            }
            else
            {
                hero.Init(heroType, ownerID, controllerID, UUID, x, y, level, health, attack, defense, vision, movement, ap, weapon, blocksMovement);
                if (GameMap.Loaded)
                {
                    GameMap.PlaceNewObjectIntoMap(hero);
                }
            }
        }
    }
	
    public void InstantiateMonster(string monsterID, string ownerID, string controllerID, string UUID, int x, int y, int health, int attack,
        int defense, int vision, int movement, int ap, Weapon weapon)
    {
        Debug.Log("Instantiating a monster");
        string monsterType = monsterID.ToUpper().Trim();
        if (!monsterPrefabDict.ContainsKey(monsterType))
        {
            Debug.LogError("Monster type not found " + monsterID);
            return;
        }

        GameObject prefab = monsterPrefabDict[monsterType];
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            instance.name = ownerID + " " + monsterType;
            var monster = instance.GetComponent<Monster>();
            if (monster == null)
            {
                Debug.LogError("Prefab did not come with monster");
                Destroy(instance);
            }
            else
            {
                monster.Init(ownerID, controllerID, UUID, x, y, health, attack, defense, vision, movement, ap, weapon, monsterID);
                if (GameMap.Loaded)
                {
                    GameMap.PlaceNewObjectIntoMap(monster);
                }
            }
        }
    }


	
}
