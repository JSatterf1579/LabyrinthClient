using UnityEngine;
using System.Collections;

public class Map {

    //Keyed to y,x
    public Tile[,] tiles;
    public int width;
    public int height;


    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.tiles = new Tile[height ,width];
    }

    public void setTile(int x, int y, Tile tile)
    {
        if(x < width && y < height)
        {
            tiles[y, x] = tile;
        }
    }

    public Tile getTile(int x, int y)
    {
        if (x < width && y < height)
        {
            return tiles[y, x];
        }
        else
        {
            return null;
        }
    }

    public override string ToString()
    {
        string rep = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                rep = rep + getTile(x, y).ToString() + "\r\n";
            }
        }
        return rep;
    }

}
