using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public int YPos { get; private set; }
    public int XPos { get; private set; }
    public int Rotation { get; private set; }
    public string Type { get; private set; }
    public bool IsObstacle { get; private set; }


    //private bool seenBefore = false;
    //originally I was keeping track of whether or not this tile had been seen before, so that when
    //the last unit is deregistered from UnitsInSight, I know wether to set Vision state to SEEN or HIDDEN,
    //but then I realized that the only way to deregister a unit is if a unit was once registered, meaning that
    //seenBefore would always be true when I need to check it, so I removed it.  

    public VisionState VisionState { get; private set; }

    /// <summary>
    /// This hashset stores the UUIDS of all units that "see" this tile.
    /// This is used to calculate the VisionState of the tile
    /// </summary>
    private HashSet<string> _UnitsInSight = new HashSet<string>();
    public HashSet<string> UnitsInSight {
        get { return _UnitsInSight; }
    }

    public Renderer[] HighlihgtRenderers;

    public Color MouseOverColor;
    public Color HighlightColor;
    private bool forceHighlighted = false;

    public Transform OverlayTransform;

    private Dictionary<string, MapObject> mapObjects = new Dictionary<string, MapObject>();
    
    public bool IsMouseOver {
        get;
        private set;
    }

    public bool IsValidForMovement {
        get {
            return !IsObstacle && !obstacleOnTile;
        }
    }

    private bool obstacleOnTile = false;

    /// <summary>
    /// true if this tile is highlighted (regardless of whether or not the cursor is over it)
    /// </summary>
    public bool Highlighted {
        get {
            return forceHighlighted;
        }

        set {
            forceHighlighted = value;
            if(!IsMouseOver) SetEmissionColor(value ? HighlightColor : Color.black);
        }
    }

    private void SetEmissionColor(Color c) {
        SetTileEmissionColor(c);
        SetMapObjectEmissionColor(c);
    }

    private void SetTileEmissionColor(Color c) {
        foreach (var r in HighlihgtRenderers) {
            r.material.SetColor("_EmissionColor", c);
        }
    }

    private void SetMapObjectEmissionColor(Color c) {
        foreach(var obj in MapObjects) {
            SetEmissionColorForObject(obj, c);
        }
    }

    private void SetEmissionColorForObject(MapObject obj, Color c) {
        foreach (var rend in obj.HighlightRenderers) {
            rend.material.SetColor("_EmissionColor", c);
        }
    }

    void Awake() {
        // this is required to enable the OnMouseEnter() and OnMouseExit() events
        if (!Physics.queriesHitTriggers) Physics.queriesHitTriggers = true;
        //HighlihgtRenderers = GetComponentsInChildren<Renderer>();
        SetEmissionColor(Color.black);
    }

    public void Init(int xPos, int yPos, int rotation, string type, bool isObstacle) {
        this.XPos = xPos;
        this.YPos = yPos;
        this.Rotation = rotation;
        this.Type = type;
        this.IsObstacle = isObstacle;
        VisionState = VisionState.HIDDEN;
    }

    void OnMouseEnter() {
        SetEmissionColor(MouseOverColor);
        IsMouseOver = true;
    }

    void OnMouseExit() {
        IsMouseOver = false;
        if (Highlighted) {
            SetEmissionColor(HighlightColor);
        } else {
            SetEmissionColor(Color.black);
        }
    }

    /// <summary>
    /// Determines if there is an obstacle on this tile
    /// </summary>
    private void calculateObstacleOnTile() {
        obstacleOnTile = false;
        foreach(var obj in mapObjects) {
            if (obj.Value.BlocksMovement) {
                obstacleOnTile = true;
                return;
            }
        }
    }

    /// <summary>
    /// This responds to mouse events sent by MapObjects
    /// </summary>
    /// <param name="enter"></param>
    private void ExternalMouseEvent(bool enter) {
        if (enter) {
            OnMouseEnter();
        } else {
            OnMouseExit();
        }
    }

    public void AddMapObject(MapObject obj) {
        mapObjects.Add(obj.UUID, obj);
        obj.OnMouseEvent += ExternalMouseEvent;
        if (obj.IsMouseOver && !this.IsMouseOver) OnMouseEnter();
        if (Highlighted) {
            SetEmissionColorForObject(obj, HighlightColor);
        }
        if (IsMouseOver) {
            SetEmissionColorForObject(obj, MouseOverColor);
        }
        calculateObstacleOnTile();
        if (obj is Unit) {
            var unit = (Unit)obj;
            Vision.PerformActionOnVisibleTilesInRange(unit.posX, unit.posY, unit.vision, t => {
                t.UnitCanSeeTile(unit.UUID);
            });
        }
    }

    public void RemoveMapObject(MapObject obj) {
        try {
            obj.OnMouseEvent -= ExternalMouseEvent;
            SetEmissionColorForObject(obj, Color.black);
            mapObjects.Remove(obj.UUID);
            calculateObstacleOnTile();
            if (obj is Unit) {
                var unit = (Unit)obj;
                Vision.PerformActionOnTilesInRange(unit.posX, unit.posY, unit.vision, t => {
                    t.UnitCannotSeeTile(unit.UUID);
                });
            }
        } catch (System.NullReferenceException e) {
            Debug.LogException(e);
        }
    }

    public void RemoveMapObject(string UUID) {
        RemoveMapObject(mapObjects[UUID]);
    }

    public bool ContainsMapObject(string UUID) {
        return mapObjects.ContainsKey(UUID);
    }

    public bool ContainsMapObject(MapObject obj) {
        return ContainsMapObject(obj.UUID);
    }

    public IEnumerable<MapObject> MapObjects {
        get {
            return mapObjects.Values;
        }
    }

    /// <summary>
    /// registers the given unit UUID as able to see this tile, updates VisionState accordingly
    /// </summary>
    /// <param name="UUID"></param>
    public void UnitCanSeeTile(string UUID) { 
        //seenBefore = true; //see comment at top
        UpdateVisibility(VisionState.VISIBLE);
        UnitsInSight.Add(UUID);
    }

    /// <summary>
    /// deregisters the given unit UUID from UnitsInSight, updates VisionState accordingly
    /// </summary>
    /// <param name="UUID"></param>
    public void UnitCannotSeeTile(string UUID) {
        UnitsInSight.Remove(UUID);
        if(UnitsInSight.Count == 0) {
            UpdateVisibility(VisionState.SEEN);
        }
    }

    private void UpdateVisibility(VisionState nvs) {
        if(VisionState != nvs) {
            VisionState = nvs;
            switch (VisionState) {
                case VisionState.VISIBLE:
                case VisionState.SEEN:
                    foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = true;
                    break;
                case VisionState.HIDDEN:
                    foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
                    break;
            }
            foreach(var obj in mapObjects.Values) {
                ApplyVisibilityToObject(obj);
            }
        }
    }

    public void ApplyVisibilityToObject(MapObject obj) {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        switch (VisionState) {
            case VisionState.VISIBLE:
                foreach(var r in renderers) {
                    r.enabled = true;
                }
                break;
            case VisionState.SEEN:
            case VisionState.HIDDEN:
                foreach (var r in renderers) {
                    r.enabled = false;
                }
                break;
        }
    }

    public override string ToString() {
        return "Tile: " + Type + ": (" + XPos + ", " + YPos + ") r=" + Rotation + " IsObstacle="+IsObstacle + " IsValidForMovement="+IsValidForMovement + " VisionState="+VisionState;
    }

}
