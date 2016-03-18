using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public int YPos { get; private set; }
    public int XPos { get; private set; }
    public int Rotation { get; private set; }
    public string Type { get; private set; }
    public bool IsObstacle { get; private set; }

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
    }

    public void RemoveMapObject(MapObject obj) {
        RemoveMapObject(obj.UUID);
    }

    public void RemoveMapObject(string UUID) {
        try {
            var obj = mapObjects[UUID];
            obj.OnMouseEvent -= ExternalMouseEvent;
            SetEmissionColorForObject(obj, Color.black);
            mapObjects.Remove(UUID);
            calculateObstacleOnTile();
        } catch (System.NullReferenceException e) {
            Debug.LogException(e);
        }
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

    public override string ToString() {
        return "Tile: " + Type + ": (" + XPos + ", " + YPos + ") r=" + Rotation + " IsObstacle="+IsObstacle + " IsValidForMovement="+IsValidForMovement;
    }

}
