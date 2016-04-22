using UnityEngine;
using System.Collections;

// Move hero up here
public abstract class Unit : MapObject
{


    
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


    public void Init(string name, string ownerID, string contrllerID, string UUID, int x, int y, int health, int attack, int defense, int vision, int movement, int ap, Weapon weapon, bool blocksMovement)
    {
        base.Init(name, ownerID, contrllerID, UUID, x, y, blocksMovement);
        //this.level = level;
        this.maxHealth = health;
        this.currentHealth = health;
        this.attack = attack;
        this.defense = defense;
        this.vision = vision;
        this.movement = movement;
        this.weapon = weapon;
        MaxActionPoints = ap;
        CurrentActionPoints = MaxActionPoints;

        if (MatchManager.instance) {
            MatchManager.instance.RegisterJSONChangeAction("/board_objects/" + UUID + "/action_points", ActionPointsChanged);
            MatchManager.instance.RegisterJSONChangeAction("/board_objects/" + UUID + "/health", HealthChanged);
        }
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
