using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class AttackUI : MonoBehaviour
{

    public Button ConfirmButton;
    public Button CancelButton;

    public Selector Selector;

    public Color HighlightColorTargetable;
    public Color HighlightColorTargetted;

    public Map Map;

    private Tile AttackSource;
    private List<Tile> TargetableTiles = new List<Tile>();
    private List<Tile> AOETiles = new List<Tile>(); 
    private List<Tile> TargetTiles; 

    private int AttackRange;

    private Unit SelectedUnit;

    private bool Active = false;
    private bool Frozen = false;

    private float Timer = 0;
    private static float Debounce = 0.5f;

    public void Freeze()
    {

        Frozen = true;
    }

    public void Resume()
    {
        if (!Frozen) return;

        Frozen = false;
    }

    public void BeginAttack(int x, int y, int range)
    {
        Timer = 0;
        Frozen = false;
        Debug.Log("Attack Starting");
        TargetTiles = new List<Tile>();
        AttackSource = Map.Current.GetTileAtPosition(x, y);
        AttackRange = range;
        Active = true;
        CancelButton.gameObject.SetActive(true);
        ClearValidTiles();
        HighlightTargettableTiles(AttackSource, range);
    }

    public void BeginAttack(Unit attacker)
    {
        SelectedUnit = attacker;
        BeginAttack(attacker.posX, attacker.posY, attacker.weapon.AttackRange);
    }

    private void ClearValidTiles()
    {
        foreach (var tile in TargetableTiles)
        {
            tile.Highlighted = false;
        }
        TargetableTiles.Clear();
    }

    //BFS search on map for tiles in range of character
    private void HighlightTargettableTiles(Tile tile, int range)
    {
        Queue<BFSNode> q = new Queue<BFSNode>();
        List<Tile> traversed = new List<Tile>();
        BFSEnque(tile, q, range-1, Map.Current, traversed);
        traversed.Add(tile);

        while (q.Count > 0)
        {
            BFSNode node = q.Dequeue();
            traversed.Add(node.tile);
            if (node.count >= 0)
            {
                if (node.tile.IsValidForMovement)
                {
                    TargetableTiles.Add(node.tile);
                    node.tile.HighlightColor = HighlightColorTargetable;
                    node.tile.Highlighted = true;
                }
                BFSEnque(node.tile, q, node.count-1, Map.Current, traversed);
            }
        }
    }


    private static void BFSEnque(Tile tile, Queue<BFSNode> q, int distance, Map map, List<Tile> seen)
    {
        if (map.CheckCoordinates(tile.XPos - 1, tile.YPos))
        {
            Tile next = map.GetTileAtPosition(tile.XPos - 1, tile.YPos);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
        if (map.CheckCoordinates(tile.XPos + 1, tile.YPos))
        {
            Tile next = map.GetTileAtPosition(tile.XPos + 1, tile.YPos);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
        if (map.CheckCoordinates(tile.XPos, tile.YPos - 1))
        {
            Tile next = map.GetTileAtPosition(tile.XPos, tile.YPos - 1);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
        if (map.CheckCoordinates(tile.XPos, tile.YPos + 1))
        {
            Tile next = map.GetTileAtPosition(tile.XPos, tile.YPos + 1);
            if (!seen.Contains(next)) q.Enqueue(new BFSNode(next, distance));
        }
    }


    private struct BFSNode
    {
        internal Tile tile;
        internal int count;

        public BFSNode(Tile tile, int count) : this()
        {
            this.tile = tile;
            this.count = count;
        }
    }


    public void EndAttack()
    {
        Debug.Log("Attack ended");
        ClearValidTiles();
        Active = false;
        Frozen = false;
        ConfirmButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        Selector.EndAttack();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!Map.Current.Loaded) return;

	    if (!Active) return;

	    if (Timer >= Debounce && Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
	    {
	        Timer = 0;
	        Tile underMouse = Map.Current.GetTileAtMouse();
            if (TargetableTiles.Contains(underMouse))
	        {
	            if (TargetTiles.Contains(underMouse))
	            {
	                TargetTiles.Remove(underMouse);
	                underMouse.HighlightColor = HighlightColorTargetable;
	            }
                else if (TargetTiles.Count <= SelectedUnit.weapon.NumTargets)
                {
                    TargetTiles.Add(underMouse);
                    underMouse.HighlightColor = HighlightColorTargetted;
                }

	            if (TargetTiles.Count == SelectedUnit.weapon.NumTargets)
	            {
	                ConfirmButton.gameObject.SetActive(true);
	            }
	            else
	            {
	                ConfirmButton.gameObject.SetActive(false);
	            }
	        }
	    }

        if (Timer < Debounce)
        {
            Timer += Time.deltaTime;
        }

    }


    public void ConfirmAttack()
    {
        JSONObject data = JSONEncoder.EncodeAttack(SelectedUnit, TargetTiles);
        MatchManager.instance.SendAction("basic_attack", data);
        Debug.Log(data);
        EndAttack();
        ConfirmButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
    }
}
