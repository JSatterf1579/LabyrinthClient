using UnityEngine;
using System.Collections;

public abstract class MapObject : MonoBehaviour {

    public string ownerID;
    public string controllerID;
    public string UUID;

    public int posX;
    public int posY;

    public Renderer[] HighlightRenderers;

    public bool IsMouseOver {
        get;
        private set;
    }

    public bool BlocksMovement {
        get;
        private set;
    }

    public Tile Tile {
        get {
            return Map.Current.GetTileAtPosition(posX, posY);
        }
    }

    /// <summary>
    /// This will be set to true by the Map.  It is used to prevent calling the wrong functions
    /// </summary>
    [System.NonSerialized]
    public bool PlacedInMap = false;

    public void Init(string ownerID, string contrllerID, string UUID, int x, int y, bool blocksMovement)
    {
        this.ownerID = ownerID;
        this.controllerID = contrllerID;
        this.UUID = UUID;
        posX = x;
        posY = y;
        BlocksMovement = blocksMovement;

        MatchManager.instance.MapObjects.Add(UUID, this);
    }

    /// <summary>
    /// The delegate to respond to mouse enter/exit events.  
    /// </summary>
    /// <param name="enter">true on mouse enter, false on exit</param>
    public delegate void MouseEventDelegate(bool enter);

    /// <summary>
    /// This event is called when the mouse enters or exits this object
    /// </summary>
    public event MouseEventDelegate OnMouseEvent;

    void OnMouseEnter() {
        IsMouseOver = true;
        if (OnMouseEvent != null) OnMouseEvent(true);
    }

    void OnMouseExit() {
        IsMouseOver = false;
        if (OnMouseEvent != null) OnMouseEvent(false);
    }


	public bool Equals(MapObject other) {
		return UUID.Equals(this.UUID);
	}

    void OnDestroy() {
        if(MatchManager.instance != null) {
            MatchManager.instance.MapObjects.Remove(UUID);
        }
    }


}
