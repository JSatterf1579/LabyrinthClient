using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementUI : MonoBehaviour {
    public GameObject ArrowPrefab;
    public GameObject StraightPrefab;
    public GameObject CornerPrefab;
	public GameObject ConfirmPanel;

    public Selector Selector;

    public Color highlightColor;

    public Map map;
    
    private Tile firstTile;
    private List<Tile> movement;

    private List<Tile> validTiles = new List<Tile>();

    private List<GameObject> overlays = new List<GameObject>();

    private int maxPathLength;

    private bool active = false;

    private Unit selectedUnit;

    private float timer = 0;

    private static float debounce = 1.0f;

    public List<Tile> Path {
        get { return movement; }
    }

    private bool frozen = false;

    public void Freeze() {
        
        frozen = true;
    }

    public void Resume() {
        if (!frozen) return;
        
        frozen = false;
    }
    
    public void BeginMove(int x, int y, int maxLength)
    {
        timer = 0;
        frozen = false;
        Debug.Log("beginning a move from (" + x + ", " + y + ") of length " + maxLength);
        movement = new List<Tile>();
        firstTile = map.GetTileAtPosition(x, y);
        maxPathLength = maxLength;
        active = true;

        ClearValidTiles();
        HighlightValidTiles(firstTile, maxLength);
        Debug.Log("" + validTiles.Count + " Tile(s) highlighted");
    }

    public void BeginMove(Unit moveTarget)
    {
        selectedUnit = moveTarget;
        BeginMove(moveTarget.posX, moveTarget.posY, moveTarget.movement);
    }

    public void EndMove() {
        Debug.Log("Movement Canceled");
        ClearValidTiles();
        ClearLine();
        active = false;
        frozen = false;
        Selector.EndMovement();
		ConfirmPanel.gameObject.SetActive(false);
    }

    private void ClearValidTiles() {
        foreach(var tile in validTiles) {
            tile.Highlighted = false;
        }
        validTiles.Clear();
    }



    /// <summary>
    /// uses a basic floodfill to highlight valid tiles and place them into the validTiles list
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="distance"></param>
    private void HighlightValidTiles(Tile tile, int distance) {
        Queue<BFSNode> q = new Queue<BFSNode>();
        List<Tile> seen = new List<Tile>();
        BFSEnque(tile, q, distance - 1, map, seen);
        
        while(q.Count > 0) {
            BFSNode node = q.Dequeue();
            seen.Add(node.tile);
            if(node.count >= 0 && node.tile.IsValidForMovement) {
                validTiles.Add(node.tile);
                node.tile.HighlightColor = highlightColor;
                node.tile.Highlighted = true;
                BFSEnque(node.tile, q, node.count - 1, map, seen);
            }
        }
    }

    private static void BFSEnque(Tile tile, Queue<BFSNode> q, int distance, Map map, List<Tile> seen) {
        if (map.CheckCoordinates(tile.XPos - 1, tile.YPos)) {
            Tile next = map.GetTileAtPosition(tile.XPos - 1, tile.YPos);
            if(!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
        if (map.CheckCoordinates(tile.XPos + 1, tile.YPos)) {
            Tile next = map.GetTileAtPosition(tile.XPos + 1, tile.YPos);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
        if (map.CheckCoordinates(tile.XPos, tile.YPos - 1)) {
            Tile next = map.GetTileAtPosition(tile.XPos, tile.YPos - 1);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
        if (map.CheckCoordinates(tile.XPos, tile.YPos + 1)) {
            Tile next = map.GetTileAtPosition(tile.XPos, tile.YPos + 1);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
    }

    private struct BFSNode {
        internal Tile tile;
        internal int count;

        public BFSNode(Tile tile, int count) : this() {
            this.tile = tile;
            this.count = count;
        }
    }

    private static bool tileIsOneAway(Tile from, Tile to) {
        int fx = from.XPos;
        int fy = from.YPos;
        int tx = to.XPos;
        int ty = to.YPos;
        if(fx == tx) {
            if (fy == ty + 1 || fy == ty - 1) return true;
        }
        if(fy == ty) {
            if (fx == tx + 1 || fx == tx - 1) return true;
        }
        return false;
    }
	
	// Update is called once per frame
	void Update () {
        //DEBUG STUFF
        if (!map.Loaded) return;
        //if (Input.GetMouseButtonDown(0)) {
        //    var tile = map.GetTileAtMouse();
        //    if (tile == null) return;
        //    Cancel();
        //    BeginMove(tile.XPos, tile.YPos, 5);
        //}
	    
        if (!active) {
            return;
        }

        if (timer >= debounce && Input.GetMouseButtonDown(0)  && EventSystem.current.currentSelectedGameObject == null)
        {
            timer = 0;
            if (frozen)
            {
                Resume();
				ConfirmPanel.gameObject.SetActive(false);
            }
            else
            {
                Freeze();
				ConfirmPanel.gameObject.SetActive(true);
            }
        }

	    if (timer < debounce)
	    {
	        timer += Time.deltaTime;
	    }

	    if (frozen)
	    {
	        return;
	    }

        Tile selected = map.GetTileAtMouse();
        if (selected == null) return;
        if (!selected.IsValidForMovement) return;
        if (!validTiles.Contains(selected)) return;
        if (selected == LastTile) return;
        if(selected == firstTile) {
            movement.Clear();
            RedrawLine();
        }
        if (movement.Contains(selected)) {
            RollBackMovementToTile(selected);
            return;
        }
        if(tileIsOneAway(LastTile, selected)) {
            if(movement.Count >= maxPathLength) {
                doTheAStar(selected);
            } else {
                movement.Add(selected);
                RedrawLine();
            }
        } else {
            doTheAStar(selected);
        }

	}

    private void doTheAStar(Tile selected) {
        AStar astar = new AStar();
        astar.FindPath(firstTile, selected, map);
        movement = astar.CellsFromPath();
        movement.Remove(firstTile);
        RedrawLine();
    }

    private Tile LastTile {
        get {
            if (movement.Count == 0) return firstTile;
            return movement[movement.Count - 1];
        }
    }
    private Tile PreviousTile {
        get {
            if (movement.Count == 2) return firstTile;
            if (movement.Count < 2) return null;
            return movement[movement.Count - 2];
        }
    }
    
    private void RollBackMovementToTile(Tile tile) {
        while(LastTile != tile && LastTile != firstTile) {
            movement.Remove(LastTile);
        }
        RedrawLine();
    }

    private void RedrawLine() {
        ClearLine();
        DrawLine();
    }

    private void ClearLine() {
        foreach(var obj in overlays) {
            Destroy(obj);
        }
    }

    private void DrawLine() {
        Vector2 last = new Vector2(firstTile.XPos, firstTile.YPos);
        for (int i = 0; i < movement.Count; i++) {
            var cur = movement[i];
            if (cur == LastTile) {//this is an arrow
                GameObject arrow = Instantiate(ArrowPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                arrow.transform.SetParent(cur.OverlayTransform, false);
                var angle = arrow.transform.eulerAngles;
                //the arrow prefab points from negative y to positive y
                if (cur.XPos == last.x) {//this arrow points along the y axis
                    angle.y = last.y < cur.YPos ? 180 : 0;
                } else if (cur.YPos == last.y) { //this arrow points along the x axis
                    angle.y = last.x > cur.XPos ? 90 : 270;
                } else {
                    Debug.LogError("Attempted to draw an invalid path.  Error at Arrow: cur=("+cur.XPos+", "+cur.YPos+") last=("+last.x+", "+last.y+").  Go yell at Steven");
                }
                arrow.transform.eulerAngles = angle;
                overlays.Add(arrow);
            } else {
                var next = movement[i + 1];
                if (last.x == next.XPos) {//this is a straight segment on the y axis
                    GameObject segment = Instantiate(StraightPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    segment.transform.SetParent(cur.OverlayTransform, false);
                    overlays.Add(segment);
                } else if (last.y == next.YPos) {//this is a straight segment on the x axis
                    GameObject segment = Instantiate(StraightPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    segment.transform.SetParent(cur.OverlayTransform, false);
                    segment.transform.eulerAngles = new Vector3(0, 90, 0);
                    overlays.Add(segment);
                } else if ((next.YPos > cur.YPos && next.XPos == cur.XPos && last.x < cur.XPos && last.y == cur.YPos) || (last.y > cur.YPos && last.x == cur.XPos && next.XPos < cur.XPos && next.YPos == cur.YPos)) { //zero degree turn
                    GameObject corner = Instantiate(CornerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    corner.transform.SetParent(cur.OverlayTransform, false);
                    overlays.Add(corner);
                } else if ((next.YPos > cur.YPos && next.XPos == cur.XPos && last.x > cur.XPos && last.y == cur.YPos) || (last.y > cur.YPos && last.x == cur.XPos && next.XPos > cur.XPos && next.YPos == cur.YPos)) { //90 degree turn
                    GameObject corner = Instantiate(CornerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    corner.transform.SetParent(cur.OverlayTransform, false);
                    corner.transform.eulerAngles = new Vector3(0, 90, 0);
                    overlays.Add(corner);
                } else if ((next.YPos < cur.YPos && next.XPos == cur.XPos && last.x > cur.XPos && last.y == cur.YPos) || (last.y < cur.YPos && last.x == cur.XPos && next.XPos > cur.XPos && next.YPos == cur.YPos)) { //180 degree turn
                    GameObject corner = Instantiate(CornerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    corner.transform.SetParent(cur.OverlayTransform, false);
                    corner.transform.eulerAngles = new Vector3(0, 180, 0);
                    overlays.Add(corner);
                } else if ((next.YPos < cur.YPos && next.XPos == cur.XPos && last.x < cur.XPos && last.y == cur.YPos) || (last.y < cur.YPos && last.x == cur.XPos && next.XPos < cur.XPos && next.YPos == cur.YPos)) { //270 degree turn
                    GameObject corner = Instantiate(CornerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    corner.transform.SetParent(cur.OverlayTransform, false);
                    corner.transform.eulerAngles = new Vector3(0, 270, 0);
                    overlays.Add(corner);
                } else {
                    Debug.LogError("Could not create line, go yell at Steven");
                }
            }
            last.x = cur.XPos;
            last.y = cur.YPos;
        }
    }

    public void ConfirmMove()
    {
        JSONObject data = JSONEncoder.EncodeMove(selectedUnit, Path);
        MatchManager.instance.SendAction("move", data);
        Debug.Log(data);
        EndMove();
		ConfirmPanel.gameObject.SetActive(false);
    }
}
