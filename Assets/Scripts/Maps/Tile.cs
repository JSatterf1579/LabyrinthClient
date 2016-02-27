using UnityEngine;

public class Tile : MonoBehaviour
{
    public int YPos { get; private set; }
    public int XPos { get; private set; }
    public int Rotation { get; private set; }
    public string Type { get; private set; }
    public bool IsObstacle { get; private set; }

    private Renderer[] renderers;

    public Color MouseOverColor;
    public Color HighlightColor;
    private bool forceHighlighted = false;

    public Transform OverlayTransform;
    
    public bool IsMouseOver {
        get;
        private set;
    }

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
        foreach (var r in renderers) {
            r.material.SetColor("_EmissionColor", c);
        }
    }

    void Awake() {
        // this is required to enable the OnMouseEnter() and OnMouseExit() events
        if (!Physics.queriesHitTriggers) Physics.queriesHitTriggers = true;
        renderers = GetComponentsInChildren<Renderer>();
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

    public override string ToString() {
        return "Tile: " + Type + ": (" + XPos + ", " + YPos + ") r=" + Rotation;
    }

}
