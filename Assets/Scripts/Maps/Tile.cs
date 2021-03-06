﻿using UnityEngine;
using System.Collections.Generic;


public class Tile : MonoBehaviour {
    public int YPos { get; private set; }
    public int XPos { get; private set; }
    public int Rotation { get; private set; }
    public string Type { get; private set; }
    public bool IsObstacle { get; private set; }
    public bool BlocksVision { get; private set; }

    public bool HighlightOverrideLock { get; set; }

    private bool _Locked = false;
    public bool Locked {
        get {
            return _Locked;
        }

        set {
            _Locked = value;
            if (value) {
                IsMouseOver = false;
                if (HighlightOverrideLock && Highlighted) {
                    SetEmissionColor(HighlightColor);
                } else {
                    SetEmissionColor(Color.black);
                }
            } else {
                RefreshEmission();
            }
        }
    }

    public GameObject VisibleObject, SeenObject, HiddenObject;
    public ParticleSystem SeenParticleSystem, HiddenParticleSystem;

    //private bool seenBefore = false;
    //originally I was keeping track of whether or not this tile had been seen before, so that when
    //the last unit is deregistered from UnitsInSight, I know wether to set Vision state to SEEN or HIDDEN,
    //but then I realized that the only way to deregister a unit is if a unit was once registered, meaning that
    //seenBefore would always be true when I need to check it, so I removed it.  
    //if you're still confused, draw the state machine on paper, it'll be clear then

    public VisionState VisionState { get; private set; }

    /// <summary>
    /// This hashset stores the UUIDS of all units that "see" this tile.
    /// This is used to calculate the VisionState of the tile
    /// </summary>
    private HashSet<string> _UnitsInSight = new HashSet<string>();
    public HashSet<string> UnitsInSight {
        get { return _UnitsInSight; }
    }

    [UnityEngine.Serialization.FormerlySerializedAs("HighlihgtRenderers")]//whoops, misspellings!
    public Renderer[] HighlightRenderers;

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
            if (Locked && !HighlightOverrideLock) return;
            RefreshEmission();
        }
    }

    private void RefreshEmission() {
        if (IsMouseOver) {
            SetEmissionColor(MouseOverColor);
        } else if (forceHighlighted) {
            SetEmissionColor(HighlightColor);
        } else {
            SetEmissionColor(Color.black);
        }
    }

    private void SetEmissionColor(Color c) {
        SetTileEmissionColor(c);
        SetMapObjectEmissionColor(c);
    }

    private void SetTileEmissionColor(Color c) {
        foreach (var r in HighlightRenderers) {
            r.material.SetColor("_EmissionColor", c);
        }
    }

    private void SetMapObjectEmissionColor(Color c) {
        foreach (var obj in MapObjects) {
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
        SetEmissionColor(Color.black);
    }

    public void Init(int xPos, int yPos, int rotation, string type, bool isObstacle, bool blocksVision) {
        this.XPos = xPos;
        this.YPos = yPos;
        this.Rotation = rotation;
        this.Type = type;
        this.IsObstacle = isObstacle;
        this.BlocksVision = blocksVision;
        UpdateVisibility(VisionState.HIDDEN);
        RefreshEmission();

        if (Map.Current.DisableVision) {
            UpdateVisibility(VisionState.VISIBLE);
        }
    }

    void OnMouseEnter() {
        IsMouseOver = true;
        if(!Locked)
            SetEmissionColor(MouseOverColor);
    }

    void OnMouseExit() {
        IsMouseOver = false;
        if (!Locked) {
            if (Highlighted) {
                SetEmissionColor(HighlightColor);
            } else {
                SetEmissionColor(Color.black);
            }
        }
    }

    /// <summary>
    /// Determines if there is an obstacle on this tile
    /// </summary>
    private void calculateObstacleOnTile() {
        obstacleOnTile = false;
        foreach (var obj in mapObjects) {
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
        AddMapObjectLogically(obj);
        AddMapObjectVisibly(obj);
    }

    public void AddMapObjectLogically(MapObject obj) {
        obj.OnMouseEvent += ExternalMouseEvent;
        mapObjects.Add(obj.UUID, obj);
        calculateObstacleOnTile();
    }

    public void AddMapObjectVisibly(MapObject obj) {
        if (obj.IsMouseOver && !this.IsMouseOver) OnMouseEnter();
        if (Highlighted) {
            SetEmissionColorForObject(obj, HighlightColor);
        }
        if (IsMouseOver) {
            SetEmissionColorForObject(obj, MouseOverColor);
        }
        if (!Map.Current.DisableVision) {
            ApplyVisibilityToObject(obj);
            if (obj.controllerID == GameManager.instance.Username && obj is Unit) {
                Debug.Log("Performing vision check on " + obj.UUID);
                var unit = (Unit)obj;
                Vision.ForEachVisibleTileInRange(XPos, YPos, unit.vision, t => {
                    t.UnitCanSeeTile(unit.UUID);
                });
            }
        }
    }

    public void RemoveMapObject(MapObject obj) {
        RemoveMapObjectLogically(obj);
        RemoveMapObjectVisibly(obj);
    }

    public void RemoveMapObjectLogically(MapObject obj) {
        mapObjects.Remove(obj.UUID);
        calculateObstacleOnTile();
        obj.OnMouseEvent -= ExternalMouseEvent;
    }

    public void RemoveMapObjectVisibly(MapObject obj) {
        try {
            SetEmissionColorForObject(obj, Color.black);

            if (!Map.Current.DisableVision) {
                if (obj.controllerID == GameManager.instance.Username && obj is Unit) {
                    var unit = (Unit)obj;
                    Vision.ForEachTileInRange(XPos, YPos, unit.vision, t => {
                        t.UnitCannotSeeTile(unit.UUID);
                    });
                }
            }
        } catch (System.NullReferenceException e) {
            Debug.LogException(e);
        }
    }

    public bool ContainsMapObject(string UUID) {
        return mapObjects.ContainsKey(UUID);
    }

    public bool IsEmpty {
        get { return mapObjects.Count == 0; }
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
        UpdateVisibility(VisionState.VISIBLE);
        UnitsInSight.Add(UUID);
    }

    /// <summary>
    /// deregisters the given unit UUID from UnitsInSight, updates VisionState accordingly
    /// </summary>
    /// <param name="UUID"></param>
    public void UnitCannotSeeTile(string UUID) {
        if (UnitsInSight.Remove(UUID) && UnitsInSight.Count == 0) {
            UpdateVisibility(VisionState.SEEN);
        }
    }

    private void UpdateVisibility(VisionState nvs) {
        if (VisionState != nvs) {
            VisionState = nvs;
            if (VisibleObject) VisibleObject.SetActive(nvs == VisionState.VISIBLE);
            if (HiddenObject) HiddenObject.SetActive(nvs == VisionState.HIDDEN);
            if (SeenObject) SeenObject.SetActive(nvs == VisionState.SEEN);
            UpdateParticleSystems(nvs);
            //foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = (nvs != VisionState.HIDDEN);
            foreach (var obj in mapObjects.Values) {
                ApplyVisibilityToObject(obj);
            }
        }
    }

    private void UpdateParticleSystems(VisionState nvs) {
        switch (nvs) {
            case VisionState.HIDDEN:
                if (HiddenParticleSystem && !HiddenParticleSystem.isPlaying)
                    HiddenParticleSystem.Play();
                if (SeenParticleSystem && SeenParticleSystem.isPlaying)
                    SeenParticleSystem.Stop();
                break;
            case VisionState.SEEN:
                if (HiddenParticleSystem && HiddenParticleSystem.isPlaying)
                    HiddenParticleSystem.Stop();
                if (SeenParticleSystem && !SeenParticleSystem.isPlaying)
                    SeenParticleSystem.Play();
                break;
            case VisionState.VISIBLE:
                if (HiddenParticleSystem && HiddenParticleSystem.isPlaying)
                    HiddenParticleSystem.Stop();
                if (SeenParticleSystem && SeenParticleSystem.isPlaying)
                    SeenParticleSystem.Stop();
                break;
        }
    }

    public void ApplyVisibilityToObject(MapObject obj) {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        switch (VisionState) {
            case VisionState.VISIBLE:
                foreach (var r in renderers) {
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
        return "Tile: " + Type + ": (" + XPos + ", " + YPos + ") r=" + Rotation + " IsObstacle=" + IsObstacle + " IsValidForMovement=" + IsValidForMovement + " VisionState=" + VisionState;
    }

}
