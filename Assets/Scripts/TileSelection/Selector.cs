using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{

	public Unit SelectedUnit { get; private set; }

    public CursorState CurrentState { get; set; }

    public MovementUI Mover;

    public AttackUI Attacker;

    public Map GameMap;

    public Button StartAttackButton;

    private float timer = 0;
    private static float debounce = 1.0f;


	// Use this for initialization
	void Start ()
    {
        CurrentState = CursorState.Selecting;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (timer > debounce)
	    {
	        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
	        {
	            MapObject tempUnit = SelectUnitUnderCursor();
	            if (CurrentState == CursorState.Selecting)
	            {
	                if (tempUnit != null)
	                {
	                    SelectedUnit = (Unit) tempUnit;
                        
	                }
	                else
	                {
	                    Debug.Log("Didn't select a unit");
	                }
	            }
	        }
	    }
	    else
	    {
	        timer += Time.deltaTime;
	    }
	
	}

    public void StartMovement()
    {
        if (SelectedUnit != null && SelectedUnit.controllerID == GameManager.instance.Username && SelectedUnit.CurrentActionPoints > 0 && CurrentState != CursorState.Movement)
        {
            if (CurrentState == CursorState.Attacking)
            {
                Unit movingUnit = SelectedUnit;
                Attacker.EndAttack();
                SelectedUnit = movingUnit;
            }
            CurrentState = CursorState.Movement;
            Mover.BeginMove(SelectedUnit);
        }
    }

    public void EndMovement()
    {

        CurrentState = CursorState.Selecting;
        SelectedUnit = null;

//        StartAttackButton.gameObject.SetActive(false);
    }

    public void StartAttack()
    {
        if (SelectedUnit != null && SelectedUnit.controllerID == GameManager.instance.Username && SelectedUnit.CurrentActionPoints > 0 && CurrentState != CursorState.Attacking)
        {
            if (CurrentState == CursorState.Movement)
            {
                Unit attacker = SelectedUnit;
                Mover.EndMove();
                SelectedUnit = attacker;
            }
            CurrentState = CursorState.Attacking;
//            StartAttackButton.gameObject.SetActive(false);
            Attacker.BeginAttack(SelectedUnit);
            
        }
    }

    public void EndAttack()
    {
        CurrentState = CursorState.Selecting;
        SelectedUnit = null;
    }

	public void SelectUnit(Unit unit) {
		if (SelectedUnit) DeselectUnit(SelectedUnit);
		SelectedUnit = unit;
		Tile tile = unit.Tile;
		tile.HighlightColor = Color.green;
		tile.Highlighted = true;
	}

	public void DeselectUnit(Unit unit) {
		if (unit) {
			Tile tile = unit.Tile;
			tile.Highlighted = false;
			Mover.EndMove();
		}
		SelectedUnit = null;
	}

    private MapObject SelectUnitUnderCursor()
    {
        Tile underCursor = GameMap.GetTileAtMouse();
        if (underCursor != null && !underCursor.Locked)
        {
            return underCursor.MapObjects.Any() ? underCursor.MapObjects.First() : null;
        }

        return null;
    }


    public enum CursorState
    {
        Movement,
        Attacking,
        Selecting,
        Selected
    };
}
