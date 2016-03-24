using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackPattern
{

    private int[,] attackValues;

    private int xmin = int.MaxValue;
    private int xmax = int.MinValue;
    private int ymin = int.MaxValue;
    private int ymax = int.MinValue;

    private bool rotatable;


    public AttackPattern(List<TargetGridTile> tiles, bool isRoatatble)
    {
        rotatable = isRoatatble;
        //Get the max and min for x and y
        foreach (TargetGridTile tile in tiles)
        {
            if (tile.x < xmin)
            {
                xmin = tile.x;
            }

            if (tile.x > xmax)
            {
                xmax = tile.x;
            }

            if (tile.y < ymin)
            {
                ymin = tile.y;
            }

            if (tile.y > ymax)
            {
                ymax = tile.y;
            }
        }

        attackValues = new int[xmax - xmin + 1, ymax - ymin + 1];

        //place the tiles in the arrays, as shifted by the min values
        foreach (TargetGridTile tile in tiles)
        {
            attackValues[tile.x - xmin, tile.y - ymin] = tile.damagePercent;
        }

    }


    public int GetRelativeTileAttackValue(int x, int y)
    {
        if (BoundsCheck(x, y))
        {
            return attackValues[x - xmin, y - ymin];
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public List<Tile> GetAffectedTiles(Tile target, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        for (int x = 0; x < attackValues.GetLength(0); x++)
        {
            for (int y = 0; y < attackValues.GetLength(1); y++)
            {
                Tile test = Map.Current.GetTileAtPosition(target.XPos + x + xmin, target.YPos + y + ymin);
                if (attackValues[x, y] != 0 && test != null)
                {
                    tiles.Add(test);
                }
            }
        }
        return tiles;
    }


    private bool BoundsCheck(int x, int y)
    {
        return x <= xmax && x >= xmin && y <= ymax && y <= ymin;
    }

}
