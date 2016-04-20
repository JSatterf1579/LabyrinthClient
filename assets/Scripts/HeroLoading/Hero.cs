using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class Hero : Unit
{

    //Really placeholder stuff until we finish this up.
    public Object[] equipment;

    public Object passive;


    public int level;
    public new void Init(string name, string ownerID, string contrllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement, int ap, Weapon weapon, bool blocksMovement)
    {
        this.level = level;
        base.Init(name, ownerID, contrllerID, UUID, x, y, health, attack, defense, vision, movement, ap, weapon, blocksMovement);
    }
}
