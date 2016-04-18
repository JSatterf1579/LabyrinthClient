using System;
using UnityEngine;
using System.Collections;

public class HeroCardData : MonoBehaviour
{

    public string Name;
    public string ownerID;
    public string UUID;

    public int health;
    public int attack;
    public int defense;
    public int vision;
    public int movement;
    public int actionPoints;
    public Weapon Weapon;
    public int level;

    public void init(string name, string ownerID, string UUID, int health, int attack, int defense, int vision,
        int movement, int actionPoints, Weapon weapon, int level)
    {
        Name = name;
        this.ownerID = ownerID;
        this.UUID = UUID;
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.vision = vision;
        this.movement = movement;
        this.actionPoints = actionPoints;
        Weapon = weapon;
        this.level = level;
    }
	
	
}
