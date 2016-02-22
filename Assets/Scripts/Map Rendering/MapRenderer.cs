using UnityEngine;
using System.Collections.Generic;

public class MapRenderer : MonoBehaviour {

    /// <summary>
    /// This allows us to set the tile names and their corresponding prefabs in the editor.  
    /// NOTE: changes made after the scene loads are ignored
    /// </summary>
    public PrefabNamePair[] TileList;

    /// <summary>
    /// this actually stores the tile prefabs and their names for fast lookup at runtime
    /// </summary>
    private Dictionary<string, GameObject> TileSet;

    private GameObject[,] CurrentMap;

    /// <summary>
    /// width is on the x axis
    /// depth is on the z (or y) axis
    /// </summary>
    private int mapWidth, mapDepth;

    /// <summary>
    /// the MapLoader to pull data from
    /// </summary>
    public MapLoader loader;

    public bool Loaded {
        get;
        private set;
    }


    void Awake() {
        Loaded = false;
        TileSet = new Dictionary<string, GameObject>();
        foreach(var pair in TileList) {
            TileSet.Add(pair.Name.ToUpper(), pair.Prefab);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Loaded) return;
        if (loader.MapLoaded) {
            Debug.Log("Building map...");
            CreateMap();
        }
	}

    /// <summary>
    /// Does the initial rendering of the map
    /// </summary>
    void CreateMap() {
        var map = loader.getCurrentMap();
        
        CurrentMap = new GameObject[map.Width, map.Height];
        float xOffset = map.Width / -2.0f;
        float yOffset = map.Height / -2.0f;


        for(int x = 0; x < map.Width; x++) {
            for(int y = 0; y < map.Height; y++) {
                var tile = map.Tiles[x, y];
                CurrentMap[x, y] = InstantiateTile(x, y, tile.Type, tile.Rotation, xOffset, yOffset);
            }
        }
        Loaded = true;

    }


    private GameObject InstantiateTile(int x, int y, string type, int rotation, float xOffset = 0f, float yOffset = 0f) {
        if (!TileSet.ContainsKey(type.ToUpper())) {
            Debug.LogError("Unknown tyle type: " + type);
            return null;
        }
        GameObject prefab = TileSet[type.ToUpper()];
        if (prefab != null) {
            GameObject instance = Instantiate(prefab, new Vector3(x + xOffset, 0, y + yOffset), Quaternion.identity) as GameObject;
            instance.name = "tile (" + x + "," + y + ") " + type;
            instance.GetComponent<Tile>().Init(x, y, rotation);
            return instance;
        } else {
            return null;
        }
    }


    /// <summary>
    /// returns the game object representing the given map space
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GameObject GetTileAtPosition(int x, int y) {
        if (!Loaded) {
            Debug.LogError("Attempted to read map before it was loaded");
            return null;
        }
        return CurrentMap[x, y];
    }

    [System.Serializable]
    public struct PrefabNamePair {
        public string Name;
        public GameObject Prefab;
    }
}
