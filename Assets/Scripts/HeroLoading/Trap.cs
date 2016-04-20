using UnityEngine;
using System.Collections;

public class Trap : MapObject
{

    public int damage;
    public Object[] effect;
    public int radius;
    public bool triggered;

    public AttackPattern pattern;

    public void Init(string name, string ownerID, string contrllerID, string UUID, int x, int y, int damage, int radius,
        bool triggered, AttackPattern pattern)
    {
        Init(name, ownerID, contrllerID, UUID, x, y, false);
        this.damage = damage;
        this.radius = radius;
        this.triggered = triggered;
        this.pattern = pattern;
    }
}
