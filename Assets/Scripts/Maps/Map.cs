using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Map : MonoBehaviour {

    public static Map Current = null;

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

    public Tile TileUnderMouse {
        get;
        private set;
    }

    /// <summary>
    /// width is on the x axis
    /// depth is on the z (or y) axis
    /// </summary>
    private int mapWidth, mapDepth;

    private float xOffset, yOffset;

    public int Width { get { return mapWidth; } }
    public int Depth { get { return mapDepth; } }
    public int Height { get { return mapDepth; } }

    public float MaxXBound { get { return -xOffset - 0.5f; } }
    public float MinXBound { get { return xOffset - 0.5f; } }
    public float MaxZBound { get { return -yOffset - 0.5f; } }
    public float MinZBound { get { return yOffset - 0.5f; } }

    public bool Loaded {
        get;
        private set;
    }


    void Awake() {
        Debug.Log("Map is awake");
        Current = this;
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
        mapWidth = width;
        mapDepth = height;
        Loaded = true;
    }

    void OnDestroy() {
        if (Current == this) Current = null;
    }


    public Tile InstantiateTile(int x, int y, string type, int rotation, bool isObstacle) {
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
                tile.Init(x, y, rotation, type, isObstacle);
                CurrentMap[x, y] = tile;
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
        if (!Loaded || !CheckCoordinates(x,y)) {
            Debug.LogError("Read out of bounds map tile or Attempted to read map before it was loaded");
            return null;
        }
        return CurrentMap[x, y];
    }

    public bool CheckCoordinates(int x, int y) {
        if (x < 0) {
            return false;
        }
        if (y < 0) {
            return false;
        }
        if (x >= mapWidth) {
            return false;
        }
        if (y >= mapDepth) {
            return false;
        }
        return true;
    }

    public Tile GetTileAtMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("MapTile", "Hero"))) {
            if (hit.transform.CompareTag("Hero")) {
                var hero = hit.transform.GetComponent<Unit>();
                return GetTileAtPosition(hero.posX, hero.posY);
            }
            return hit.collider.GetComponent<Tile>();
        }
        return null;
    }

    internal void MoveMapObject(MapObject target, List<Tile> tiles) {
        Vector3 curPosition = target.transform.position;
        Tile lastTile = tiles[tiles.Count - 1];
        MoveMapObject(target, lastTile.XPos, lastTile.YPos);
        target.transform.position = curPosition;
        StartCoroutine(AnimateMove(target.transform, tiles, 1, 1 / 60f));
    }

    public void MoveMapObject(MapObject obj, int newX, int newY) {
        if (!obj.PlacedInMap) {
            Debug.LogError("Error; attempted to move an object that has not yet been placed on the map!");
            return;
        }
        if(!CheckCoordinates(newX, newY)) {
            Debug.LogError("Error; attempted to move an object to coordinates that are outside the map bounds");
            return;
        }
        Tile dest = GetTileAtPosition(newX, newY);
        if(dest == null) {
            Debug.LogError("Error; attempted to move an object to a null tile");
            return;
        }
        Tile src = GetTileAtPosition(obj.posX, obj.posY);
        src.RemoveMapObject(obj);
        putObjectAtTile(obj, dest);
    }

    private void putObjectAtTile(MapObject obj, Tile tile) {
        var mount = tile.OverlayTransform;
        obj.transform.parent = mount;
        obj.transform.localPosition = Vector3.zero;
        tile.AddMapObject(obj);
        obj.posX = tile.XPos;
        obj.posY = tile.YPos;
    }

    /// <summary>
    /// Initally places the MapObject in the map.  This should only EVER be called exactly ONCE PER OBJECT at initalization.  
    /// </summary>
    /// <param name="obj"></param>
    public void PlaceNewObjectIntoMap(MapObject obj) {
        Debug.Log("Placing new object into map");
        if (obj.PlacedInMap) {
            Debug.LogError("Error; attempted to place an object as new, but it has already been placed!");
            return;
        }
        Tile t = GetTileAtPosition(obj.posX, obj.posY);
        if (t != null) {
            putObjectAtTile(obj, t);
            obj.PlacedInMap = true;
        } else {
            Debug.LogError("Error; attempted to place a new object onto an invalid tile");
        }
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

    void Update() {
        DebugHUD.setValue("TileUnderMouse", GetTileAtMouse() as Tile);
    }

    IEnumerator AnimateMove(Transform t, List<Tile> path, float time, float updateDelay) {
        float timePerSegment = time / path.Count;
        float curSegmentTime = 0;
        int pathIndex = 0;
        Vector3 oldPos = t.position;
        Vector3 targetPos;
        while(pathIndex < path.Count) {
            targetPos = path[pathIndex].OverlayTransform.position;
            float delta = curSegmentTime / timePerSegment;
            t.position = Vector3.Lerp(oldPos, targetPos, delta);
            curSegmentTime += updateDelay;
            if(curSegmentTime >= timePerSegment) {
                curSegmentTime -= timePerSegment;
                pathIndex++;
                oldPos = targetPos;
            }
            yield return new WaitForSeconds(updateDelay);
        }
        t.localPosition = Vector3.zero;
    }
}
