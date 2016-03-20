using UnityEngine;
using System.Collections;

public class TargetGridTile
{

    int x;
    int y;
    int damagePercent;

    public TargetGridTile(int x, int y, int damage)
    {
        this.x = x;
        this.y = y;
        this.damagePercent = damage;
    }
}
