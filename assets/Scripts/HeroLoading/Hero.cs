using UnityEngine;
using System.Collections;
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
}
