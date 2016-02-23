using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour {

    /// <summary>
    /// This allows us to set the tile names and their corresponding prefabs in the editor.  
    /// NOTE: changes made after the scene loads are ignored
    /// </summary>
    public PrefabNamePair[] TileList;

    /// <summary>
    /// this actually stores the tile prefabs and their names for fast lookup at runtime
    /// </summary>
    private Dictionary<string, GameObject> TileSet;

    private Tile[,] CurrentMap;

    /// <summary>
    /// width is on the x axis
    /// depth is on the z (or y) axis
    /// </summary>
    private int mapWidth, mapDepth;

    private float xOffset, yOffset;

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

    /// <summary>
    /// Initalizes a map of the given size
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void InitalizeMap(int width, int height) {
        InitalizeMap(width, height, width / -2.0f, height / -2.0f);
    }


    /// <summary>
    /// Initalizes the map with the given x and y offset overrides
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="xOffsetOverride"></param>
    /// <param name="yOffsetOverride"></param>
    public void InitalizeMap(int width, int height, float xOffsetOverride, float yOffsetOverride) {
        CurrentMap = new Tile[width, height];
        this.xOffset = xOffsetOverride;
        this.yOffset = yOffsetOverride;
        Loaded = true;
    }


    public Tile InstantiateTile(int x, int y, string type, int rotation) {
        if (!Loaded) {
            Debug.LogError("Tried to instantiate a tile on a Map that has not been initalized yet");
            return null;
        }
        string utype = type.ToUpper().Trim();
        if (!TileSet.ContainsKey(utype)) {
            Debug.LogError("Unknown tyle type: " + type);
            return null;
        }
        GameObject prefab = TileSet[utype];
        if (prefab != null) {
            GameObject instance = Instantiate(prefab, new Vector3(x + xOffset, 0, y + yOffset), Quaternion.identity) as GameObject;
            instance.transform.eulerAngles = new Vector3(0f, rotation * 90f, 0f);
            instance.name = "tile (" + x + "," + y + ") " + type;
            instance.transform.SetParent(this.transform, true);
            var tile = instance.GetComponent<Tile>();
            if (tile == null) {
                Debug.LogError("Error: a Tile prefab was instantiated, but it does not have an attached Tile behaviour; it will now be destroyed for its insubordination");
                Destroy(instance);
            } else {
                tile.Init(x, y, rotation, type);
            }
            return tile;
        } else {
            return null;
        }
    }

    public void DestroyMap() {
        if (!Loaded) return;
        foreach(Tile t in CurrentMap) {
            Destroy(t);
        }
        CurrentMap = null;
        Loaded = false;
    }


    /// <summary>
    /// returns the game object representing the given map space
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile GetTileAtPosition(int x, int y) {
        if (!Loaded) {
            Debug.LogError("Attempted to read map before it was loaded");
            return null;
        }
        return CurrentMap[x, y];
    }

    public Tile this[int x, int y] {
        get {
            return GetTileAtPosition(x, y);
        }
    }

    [System.Serializable]
    public struct PrefabNamePair {
        public string Name;
        public GameObject Prefab;
    }
}
