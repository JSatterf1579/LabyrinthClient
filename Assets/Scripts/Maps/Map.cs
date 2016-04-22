using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Map : MonoBehaviour {

    /// <summary>
    /// Map is a singleton
    /// </summary>
    public static Map Current = null;

    public bool DisableVision = false;

    /// <summary>
    /// This allows us to set the tile names and their corresponding prefabs in the editor.  
    /// NOTE: changes made after the scene loads are ignored
    /// </summary>
    public PrefabNamePair[] TileList;

    /// <summary>
    /// this actually stores the tile prefabs and their names for fast lookup at runtime
    /// </summary>
    private Dictionary<string, GameObject> TileSet;

    /// <summary>
    /// This stores the tiles in the map
    /// </summary>
    private Tile[,] CurrentMap;

    /// <summary>
    /// width is on the x axis
    /// depth is on the z (or y) axis
    /// </summary>
    private int mapWidth, mapDepth;

    private float xOffset, yOffset;

    /// <summary>
    /// The number of tiles wide this map is (x axis)
    /// </summary>
    public int Width { get { return mapWidth; } }
    /// <summary>
    /// The number of tiles deep/tall this map is (y axis)
    /// </summary>
    public int Depth { get { return mapDepth; } }
    /// <summary>
    /// The number of tiles deep/tall this map is (y axis)
    /// </summary>
    public int Height { get { return mapDepth; } }

    /// <summary>
    /// The +x coordinate of the edge of the map.
    /// </summary>
    public float MaxXBound { get { return -xOffset - 0.5f; } }
    /// <summary>
    /// The -X coordinate of the edge of the map.
    /// </summary>
    public float MinXBound { get { return xOffset - 0.5f; } }
    /// <summary>
    /// The +Z coordinate of the edge of the map.
    /// </summary>
    public float MaxZBound { get { return -yOffset - 0.5f; } }
    /// <summary>
    /// The -Z coordinate of the edge of the map.
    /// </summary>
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
    /// Initalizes a map of the given size.
    /// This must be called prior to using the map.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void InitalizeMap(int width, int height) {
        InitalizeMap(width, height, width / -2.0f, height / -2.0f);
    }


    /// <summary>
    /// Initalizes the map with the given x and y offset overrides.  
    /// This must be called prior to using the map.
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

    /// <summary>
    /// Instantiates a tile into the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    /// <param name="rotation"></param>
    /// <param name="isObstacle"></param>
    /// <returns></returns>
    public Tile InstantiateTile(int x, int y, string type, int rotation, bool isObstacle, bool blocksVision) {
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
                tile.Init(x, y, rotation, type, isObstacle, blocksVision);
                CurrentMap[x, y] = tile;
            }
            return tile;
        } else {
            return null;
        }
    }

    /// <summary>
    /// destroys the map.  allows the same instance to be reused
    /// </summary>
    public void DestroyMap() {
        if (!Loaded) return;
        foreach(Tile t in CurrentMap) {
            Destroy(t);
        }
        CurrentMap = null;
        Loaded = false;
    }


    /// <summary>
    /// returns the tile at the given coordinates
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


    /// <summary>
    /// returns true if the given coordinates are within the bounds of the current map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
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

    /// <summary>
    /// returns the current tile that is under the mouse, or null if there is none.  If there is a mapobject under the mouse, the tile that the object is on will be returned
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Moves the given MapObject along the specified path.  This is the function you should be calling to move an object
    /// </summary>
    /// <param name="target"></param>
    /// <param name="tiles"></param>
    public void MoveMapObject(MapObject target, List<Tile> tiles) {
        StartCoroutine(AnimateMove(target, tiles, 1, 1 / 60f));
    }

    public void RemoveMapObject(MapObject target) {
        target.Tile.RemoveMapObject(target);
    }

    /// <summary>
    /// returns an IEnumerable containing first the tile the target is on, then the remaining tiles in the path
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerable<Tile> GetTilesInPath(MapObject target, List<Tile> path) {
        yield return target.Tile;
        foreach(Tile t in path) {
            yield return t;
        }
    }

    public void SetTilesLockedState(IEnumerable<Tile> tiles, bool locked) {
        foreach(Tile t in tiles) {
            t.Locked = locked;
        }
    }

    public void LockTiles(IEnumerable<Tile> tiles) {
        SetTilesLockedState(tiles, true);
    }

    public void LockAllTiles() {
        LockTiles(GetAllTiles());
    }

    public IEnumerable<Tile> GetAllTiles() {
        foreach(Tile t in CurrentMap) {
            yield return t;
        }
    }

    public void UnlockTiles(IEnumerable<Tile> tiles) {
        SetTilesLockedState(tiles, false);
    }

    public void UnlockAllTiles() {
        UnlockTiles(GetAllTiles());
    }

    /// <summary>
    /// Forcibly teleports a mapObject to new coordinates without any animation (and without traveling over any tiles in between)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
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
        dest.AddMapObject(obj);
        putObjectAtTile(obj, dest);
    }


    /// <summary>
    /// Updates the transform of the object to place it physically on the given tile.  
    /// Odds are, you should not be calling this function.  It does NOT update anything other than the MapObject's transform!
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tile"></param>
    private void putObjectAtTile(MapObject obj, Tile tile) {
        var mount = tile.OverlayTransform;
        obj.transform.parent = mount;
        obj.transform.localPosition = Vector3.zero;
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
            t.AddMapObject(obj);
            putObjectAtTile(obj, t);
            obj.PlacedInMap = true;
        } else {
            Debug.LogError("Error; attempted to place a new object onto an invalid tile");
        }
    }
    /// <summary>
    /// Shorthand for GetTileAtPosition(x, y)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile this[int x, int y] {
        get {
            return GetTileAtPosition(x, y);
        }
    }


    /// <summary>
    /// This struct is used to build the prefab mappings so that they can be set in the editor
    /// </summary>
    [System.Serializable]
    public struct PrefabNamePair {
        public string Name;
        public GameObject Prefab;
    }

    /// <summary>
    /// update just contains some non-essential debug code
    /// </summary>
    void Update() {
        var t = GetTileAtMouse();
        DebugHUD.setValue("TileUnderMouse", t as Tile);
        if (t != null) {
            DebugHUD.setValue("UnitsInSightOfTile", "{" + string.Join(", ", t.UnitsInSight.ToArray()) + "}");
        } else {
            DebugHUD.setValue("UnitsInSightOfTile", "N/A");
        }
    }

    /// <summary>
    /// Co-Routine to animate the movement of a map object along a path
    /// </summary>
    /// <param name="t"> The MapObject to move </param>
    /// <param name="path"> The path it should take </param>
    /// <param name="time"> How long the animation should take (in seconds) </param>
    /// <param name="updateDelay"> The number of seconds between updates to the position of the object (I recommend just passing in "1/60f" for 60fps)</param>
    /// <returns></returns>
    private IEnumerator AnimateMove(MapObject t, List<Tile> path, float time, float updateDelay) {
        List<Tile> allTiles = GetTilesInPath(t, path).ToList(); //we make it a list to force evaluation immediately
        LockTiles(allTiles);
        t.Tile.RemoveMapObjectLogically(t);
        path.Last().AddMapObjectLogically(t);
        float timePerSegment = time / path.Count;
        float curSegmentTime = 0;
        int pathIndex = 0;
        Vector3 oldPos = t.transform.position;
        Vector3 targetPos;
        Tile tile = t.Tile;
        bool pastMidpoint = false;
        while(pathIndex < path.Count) {
            targetPos = path[pathIndex].OverlayTransform.position;
            float delta = curSegmentTime / timePerSegment;
            t.transform.position = Vector3.Lerp(oldPos, targetPos, delta);
            curSegmentTime += updateDelay;
            if(!pastMidpoint && delta >= 0.5f) {
                Tile newTile = path[pathIndex];
                tile.RemoveMapObjectVisibly(t);
                tile.Locked = false;
                newTile.AddMapObjectVisibly(t);
                tile = newTile;
                pastMidpoint = true;
            }
            if(curSegmentTime >= timePerSegment) {
                curSegmentTime -= timePerSegment;
                pathIndex++;
                oldPos = targetPos;
                pastMidpoint = false;
            }
            yield return new WaitForSeconds(updateDelay);
        }
        putObjectAtTile(t, tile);
        UnlockTiles(allTiles);
    }
}
