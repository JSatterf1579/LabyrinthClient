using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon
{
    public int AttackModifier;
    public int AttackRange;
    public int NumTargets;
    public AttackPattern Pattern;
    public string Image;
    public string Name;
    public string Description;



    public Weapon(string name, string image, int attackModifier, int range,  string description, List<TargetGridTile> attackTiles, bool rotatable, int numTargets)
    {
        this.Name = name;
        this.Image = image;
        this.Description = description;
        this.AttackModifier = attackModifier;
        NumTargets = numTargets;
        AttackRange = range;
        Pattern = MakeDamageMap(attackTiles, rotatable);
    }

    private AttackPattern MakeDamageMap(List<TargetGridTile> pattern, bool rotatable)
    {
        AttackPattern p = new AttackPattern(pattern, rotatable);
        return p;
    }

}
