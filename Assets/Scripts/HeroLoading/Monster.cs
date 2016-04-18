using UnityEngine;
using System.Collections;

public class Monster : Unit
{

    

    public void Init(string ownerID, string contrllerID, string UUID, int x, int y, int health, int attack,
        int defense, int vision, int movement, int ap, Weapon weapon, string name)
    {
        
        base.Init(name, ownerID, contrllerID, UUID, x, y, health, attack, defense, vision, movement, ap, weapon);
    }
}
