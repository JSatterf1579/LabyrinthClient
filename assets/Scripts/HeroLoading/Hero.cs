using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class Hero : Unit
{

    //Really placeholder stuff until we finish this up.
    public Object[] equipment;

    public Object[] weapon;

    public Object passive;

    public void Init(string ownerID, string contrllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement, Object passive)
    {
        base.Init(ownerID, contrllerID, UUID, x, y, level, health, attack, defense, vision, movement);
    }


    private bool moving = false;
    private List<Tile> moves = null;
    private float timer = 0;
    void Update() {
        ////DEBUG STUFF
        //if (moves != null) {
        //    if(timer < 1.0f) {
        //        timer += Time.deltaTime;
        //    } else {
        //        timer -= 1.0f;
        //        Map.Current.MoveMapObject(this, moves[0].XPos, moves[0].YPos);
        //        moves.RemoveAt(0);
        //        Debug.Log(moves.Count + " moves remaining");
        //        if (moves.Count == 0) moves = null;
        //    }
        //} else if (Input.GetMouseButtonDown(0)) {
        //    if (!moving) {
        //        var tile = Map.Current.GetTileAtMouse();
        //        if (tile == null) return;
        //        if (tile.XPos == posX && tile.YPos == posY) {
        //            var movementUI = FindObjectOfType<MovementUI>();
        //            if (movementUI == null) {
        //                Debug.LogError("Could not find MovementUI");
        //                return;
        //            }
        //            moving = true;
        //            movementUI.BeginMove(posX, posY, movement);
        //        }
        //    }
        //    else {
        //        moving = false;
        //        var movementUI = FindObjectOfType<MovementUI>();
        //        if (movementUI == null) {
        //            Debug.LogError("Could not find MovementUI");
        //            return;
        //        }
        //        movementUI.EndMove();
        //        moves = movementUI.Path;
        //        Debug.Log("Movement of length "+moves.Count +" recieved");
        //    }
        //}
    }
}
