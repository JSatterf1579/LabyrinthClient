using UnityEngine;
using System.Collections;

public struct TargetGridTile
{

    public int x;
    public int y;
    public int damagePercent;

    public TargetGridTile(int x, int y, int damage)
    {
        this.x = x;
        this.y = y;
        this.damagePercent = damage;
    }
}
