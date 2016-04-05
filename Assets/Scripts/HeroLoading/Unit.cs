﻿using UnityEngine;
using System.Collections;
using UnityEditor;

// Move hero up here
public abstract class Unit : MapObject
{

    public int level;
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
    public int vision;
    public int movement;
    public int MaxActionPoints;
    public int CurrentActionPoints;
    public Weapon weapon;

    public Object[] status;
    public Object[] abilities;


    public void Init(string ownerID, string contrllerID, string UUID, int x, int y, int level, int health, int attack, int defense, int vision, int movement, int ap, Weapon weapon)
    {
        Init(ownerID, contrllerID, UUID, x, y);
        this.level = level;
        this.maxHealth = health;
        this.currentHealth = health;
        this.attack = attack;
        this.defense = defense;
        this.vision = vision;
        this.movement = movement;
        this.weapon = weapon;
        MaxActionPoints = ap;
        CurrentActionPoints = MaxActionPoints;

        MatchManager.instance.RegisterJSONChangeAction("/board_objects/" + UUID + "/action_points", ActionPointsChanged);
        MatchManager.instance.RegisterJSONChangeAction("/board_objects/" + UUID + "/health", HealthChanged);
    }


    private void ActionPointsChanged(JSONChangeInfo info)
    {
        if (info.Type != JSONChangeInfo.ChangeType.CHANGED)
        {
            Debug.LogWarning("Non change event recieved?");
            return;
        }
        Debug.Log(UUID + "'s Action Points were changed from " + info.OldValue.n + " to " + info.NewValue.n);
        CurrentActionPoints = (int)info.NewValue.n;
    }


    private void HealthChanged(JSONChangeInfo info)
    {
        if (info.Type != JSONChangeInfo.ChangeType.CHANGED)
        {
            Debug.LogWarning("Non change event recieved?");
            return;
        }
        currentHealth = (int) info.NewValue.n;
    }
}
