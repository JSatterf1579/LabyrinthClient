using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class CaptureUI : MonoBehaviour {

    public Dialog dialogBox;

    public Selector Selector;

    public Color HighlightColorTargetable;
    public Color HighlightColorTargetted;

    public Map Map;

    private List<Tile> ObjectiveTiles = new List<Tile>(); 

    private Objective TargetedObjective;

    private Unit SelectedUnit;

    private bool Active = false;
    private bool Frozen = false;

    private float Timer = 0;
    private static float Debounce = 0.05f;

    public void Freeze()
    {

        Frozen = true;
    }

    public void Resume()
    {
        if (!Frozen) return;

        Frozen = false;
    }

    public void BeginCapture(Unit unit)
    {
        SelectedUnit = unit;
        ObjectiveTiles = new List<Tile>();
        Active = true;
        dialogBox.ShowLeft(null, "C A N C E L", this.EndCapture);
        ClearObjectiveTiles();
        HighlightObjectiveTiles(unit);
    }

    public void EndCapture()
    {
        ClearObjectiveTiles();
        Active = false;
        Frozen = false;
        dialogBox.Hide();
        Selector.EndCapture();
    }

    public void ConfirmCapture()
    {
        JSONObject payload = JSONEncoder.EncodeCapture(SelectedUnit, TargetedObjective);
        MatchManager.instance.SendAction("capture_objective", payload);
        EndCapture();
    }

    public bool IsObjectiveCaptureable(Unit unit)
    {
        int objCount = 0;
        int unitX = unit.posX;
        int unitY = unit.posY;

        for (int i = -1; i <= 1; i += 2)
        {
            if (Map.CheckCoordinates(unitX + i, unitY))
            {
                if (Map.GetTileAtPosition(unitX + i, unitY).MapObjects.Any())
                {
                    if (Map.GetTileAtPosition(unitX + i, unitY).MapObjects.First() is Objective)
                    {
                        objCount++;
                    }
                }
            }

            if (Map.CheckCoordinates(unitX, unitY + i))
            {
                if (Map.GetTileAtPosition(unitX, unitY + i).MapObjects.Any())
                {
                    if (Map.GetTileAtPosition(unitX, unitY + i).MapObjects.First() is Objective)
                    {
                        objCount++;
                    }
                }
            }
        }

        
        Debug.Log(objCount);
        return objCount > 0;
    }

    private void HighlightObjectiveTiles(Unit unit)
    {
        
        int unitX = unit.posX;
        int unitY = unit.posY;

        for (int i = -1; i <= 1; i += 2)
        {
            if (Map.CheckCoordinates(unitX + i, unitY))
            {
                Tile t = Map.GetTileAtPosition(unitX + i, unitY);
                if (t.MapObjects.Any())
                {
                    if (t.MapObjects.First() is Objective)
                    {
                        ObjectiveTiles.Add(t);
                        t.HighlightColor = HighlightColorTargetable;
                        t.Highlighted = true;
                    }
                }
            }

            if (Map.CheckCoordinates(unitX, unitY + i))
            {
                Tile t = Map.GetTileAtPosition(unitX, unitY + i);
                if (t.MapObjects.Any())
                {
                    if (t.MapObjects.First() is Objective)
                    {
                        ObjectiveTiles.Add(t);
                        t.HighlightColor = HighlightColorTargetable;
                        t.Highlighted = true;
                    }
                }
            }
        }
    }

    private void ClearObjectiveTiles()
    {
        foreach (Tile tile in ObjectiveTiles)
        {
            tile.Highlighted = false;
        }
        ObjectiveTiles.Clear();
    }


	
	// Update is called once per frame
	void Update () {

        if (!Map.Current.Loaded) return;

        if (!Active) return;

	    if (Timer >= Debounce && Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
	    {
	        Timer = 0;
	        Tile t = Map.GetTileAtMouse();
	        if (t != null)
	        {
	            if (ObjectiveTiles.Contains(t))
	            {
                    if (TargetedObjective != null)
                    {
                        TargetedObjective.Tile.HighlightColor = HighlightColorTargetable;
                    }
	                TargetedObjective = (Objective) t.MapObjects.First();
	                TargetedObjective.Tile.HighlightColor = HighlightColorTargetted;
                    dialogBox.ShowRight(null, "C O N F I R M", this.ConfirmCapture);
                }
	        }
	    }
	    if (Timer < Debounce)
	    {
	        Timer += Time.deltaTime;
	    }

    }
}
