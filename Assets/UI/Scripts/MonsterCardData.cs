using UnityEngine;
using System.Collections;

public class MonsterCardData
{

    public string Name;
    public int id;

    public int health;
    public int attack;
    public int defense;
    public int movement;
    public int vision;

    public Weapon weapon;
    public int count;
    public int cost = 1;

    public MonsterCardData(string Name, int id, int health, int attack, int defense, int movement,
        int vision, Weapon weapon, int count)
    {
        this.Name = Name;
        this.id = id;
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.movement = movement;
        this.vision = vision;
        this.weapon = weapon;
        this.count = count;
    }
}
