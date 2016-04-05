using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class Hero : Unit
{

    //Really placeholder stuff until we finish this up.
    public Object[] equipment;

    public Object passive;

    public new void Init(string ownerID, string contrllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement, int ap, Weapon weapon)
    {
        base.Init(ownerID, contrllerID, UUID, x, y, level, health, attack, defense, vision, movement, ap, weapon);
    }
}
