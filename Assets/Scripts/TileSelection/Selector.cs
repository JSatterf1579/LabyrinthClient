
﻿using System;
using System.Collections;
﻿using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{

	public Unit SelectedUnit { get; private set; }

    public InGame UI;

    public CursorState CurrentState { get; set; }

    public MovementUI Mover;

    public AttackUI Attacker;

    public CaptureUI Capture;

    public Map GameMap;

    public GameObject MoveButton;
    public GameObject AttackButton;
    public GameObject CaptureButton;

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
	                    if (SelectedUnit != null)
	                    {
	                        DeselectUnit(SelectedUnit);
	                    }
	                    if (tempUnit is Unit)
	                    {
	                        SelectUnit((Unit) tempUnit);
	                    }
	                    else
	                    {
	                        UI.selectedUnit = tempUnit;
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

    public void StartMovement()
    {
        if (SelectedUnit != null && SelectedUnit.controllerID == GameManager.instance.Username && SelectedUnit.CurrentActionPoints > 0 && CurrentState != CursorState.Movement)
        {
            if (CurrentState == CursorState.Attacking)
            {
                Unit movingUnit = SelectedUnit;
                Attacker.EndAttack();
                SelectUnit(movingUnit);
            }

            if (CurrentState == CursorState.Capturing)
            {
                Unit attacker = SelectedUnit;
                Capture.EndCapture();
                SelectUnit(attacker);
            }

            CurrentState = CursorState.Movement;
            Mover.BeginMove(SelectedUnit);
        }
    }

    public void EndMovement()
    {

        CurrentState = CursorState.Selecting;
        DeselectUnit(SelectedUnit);

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
                SelectUnit(attacker);
            }

            if (CurrentState == CursorState.Capturing)
            {
                Unit attacker = SelectedUnit;
                Capture.EndCapture();
                SelectUnit(attacker);
            }
            CurrentState = CursorState.Attacking;
//            StartAttackButton.gameObject.SetActive(false);
            Attacker.BeginAttack(SelectedUnit);
            
        }
    }

    public void EndAttack()
    {
        CurrentState = CursorState.Selecting;
        DeselectUnit(SelectedUnit);
    }

    public void StartCapture()
    {
        if (SelectedUnit != null && SelectedUnit.controllerID == GameManager.instance.Username && SelectedUnit.controllerID == MatchManager.instance.HeroPlayer &&
            SelectedUnit.CurrentActionPoints > 0 && CurrentState != CursorState.Attacking)
        {
            if (CurrentState == CursorState.Movement)
            {
                Unit capturer = SelectedUnit;
                Mover.EndMove();
                SelectUnit(capturer);
            }

            if (CurrentState == CursorState.Attacking)
            {
                Unit capturer = SelectedUnit;
                Attacker.EndAttack();
                SelectUnit(capturer);
            }

            CurrentState = CursorState.Capturing;
            Capture.BeginCapture(SelectedUnit);
        }
    }

    public void EndCapture()
    {
        CurrentState = CursorState.Selecting;
        DeselectUnit(SelectedUnit);
    }

	public void SelectUnit(Unit unit) {
	    if (SelectedUnit)
	    {
	        if (CurrentState == CursorState.Attacking)
	        {
	            Attacker.EndAttack();
	        }
            else if (CurrentState == CursorState.Movement)
            {
                Mover.EndMove();
            }
            else if (CurrentState == CursorState.Capturing)
            {
                Capture.EndCapture();
            }
            else
            {
                DeselectUnit(SelectedUnit);
            }
	    }
	    SelectedUnit = unit;
		Tile tile = unit.Tile;
		tile.HighlightColor = Color.green;
		tile.Highlighted = true;
	    UI.selectedUnit = SelectedUnit;
	    if (unit.CurrentActionPoints > 0 && unit.controllerID == GameManager.instance.Username)
	    {
	        MoveButton.SetActive(true);
            AttackButton.SetActive(true);
	        if (Capture.IsObjectiveCaptureable(SelectedUnit) && SelectedUnit.controllerID == MatchManager.instance.HeroPlayer)
	        {
	            CaptureButton.SetActive(true);
	        }
	    }
	}

	public void DeselectUnit(Unit unit) {
		if (unit) {
			Tile tile = unit.Tile;
			tile.Highlighted = false;
			//Mover.EndMove();
		}
		SelectedUnit = null;
	    UI.selectedUnit = null;
        MoveButton.SetActive(false);
        AttackButton.SetActive(false);
        CaptureButton.SetActive(false);
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
        Capturing,
        Selected
    };
}
