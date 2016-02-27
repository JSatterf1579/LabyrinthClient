using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroManager : MonoBehaviour
{

    public Map.PrefabNamePair[] heroPrefabs;
    private Dictionary<string, GameObject> heroPrefabDict; 

    public Dictionary<string, Unit> heroList;
    public Map GameMap;
     

	// Use this for initialization
	void Awake () {
	
        heroPrefabDict = new Dictionary<string, GameObject>();
        heroList = new Dictionary<string, Unit>();

	    foreach (var pair in heroPrefabs)
	    {
	        heroPrefabDict.Add(pair.Name.ToUpper(), pair.Prefab);
	    }
	}

    public void InstantiateHero(string heroID, string ownerID, string controllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement)
    {
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
                heroList.Add(UUID, hero);
                if (GameMap.Loaded)
                {
                    //Move to unit functionality
                    Tile currentTile = GameMap.GetTileAtPosition(x, y);
                    GameObject tileObject = currentTile.gameObject;
                    instance.transform.SetParent(tileObject.transform);
                    instance.transform.localPosition = new Vector3(0, 0, tileObject.transform.localScale.z);
                }
            }
        }
    }
	
	
}
