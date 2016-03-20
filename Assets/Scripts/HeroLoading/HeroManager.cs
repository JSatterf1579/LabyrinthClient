using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroManager : MonoBehaviour
{

    public Map.PrefabNamePair[] heroPrefabs;
    private Dictionary<string, GameObject> heroPrefabDict; 

    public Map GameMap;
     

	// Use this for initialization
	void Awake () {
	
        heroPrefabDict = new Dictionary<string, GameObject>();

	    foreach (var pair in heroPrefabs)
	    {
	        heroPrefabDict.Add(pair.Name.ToUpper(), pair.Prefab);
	    }
	}

    public void InstantiateHero(string heroID, string ownerID, string controllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement)
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
                hero.Init(ownerID, controllerID, UUID, x, y, level, health, attack, defense, vision, movement, null);
                if (GameMap.Loaded)
                {
                    GameMap.PlaceNewObjectIntoMap(hero);
                }
            }
        }
    }
	
	
}
