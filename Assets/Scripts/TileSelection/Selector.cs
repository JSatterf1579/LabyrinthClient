using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{

    private Unit SelectedUnit;

    public CursorState CurrentState { get; set; }

    public MovementUI Mover;

    public Map GameMap;

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
	                    if (SelectedUnit.controllerID == GameManager.instance.Username)
	                    {
                            CurrentState = CursorState.Movement;
	                        Mover.BeginMove(SelectedUnit);
	                    }
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

    public void EndMovement()
    {
        CurrentState = CursorState.Selecting;
        
    }

    private MapObject SelectUnitUnderCursor()
    {
        Tile underCursor = GameMap.GetTileAtMouse();
        if (underCursor != null)
        {
            return underCursor.MapObjects.Any() ? underCursor.MapObjects.First() : null;
        }

        return null;
    }


    public enum CursorState
    {
        Movement,
        Attacking,
        Selecting
    };
}
