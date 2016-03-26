using UnityEngine;
using System.Collections;
using UnityEditor;

// Move hero up here
public abstract class Unit : MapObject
{

    public int level;
    public int maxHealth;
    public int attack;
    public int defense;
    public int vision;
    public int movement;
    public int MaxActionPoints;
    public int CurrentActionPoints;
    public Weapon weapon;

    public Object[] status;
    public Object[] abilities;


    public void Init(string ownerID, string contrllerID, string UUID, int x, int y, int level, int health, int attack, int defense, int vision, int movement, Weapon weapon, bool blocksMovement)
    {
        base.Init(ownerID, contrllerID, UUID, x, y, blocksMovement);
        this.level = level;
        this.maxHealth = health;
        this.attack = attack;
        this.defense = defense;
        this.vision = vision;
        this.movement = movement;
        this.weapon = weapon;
    }

}
