using UnityEngine;
using System.Collections;

public class Map {

    //Keyed to y,x
    public Tile[,] Tiles;
    public int Width;
    public int Height;


    public Map(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.Tiles = new Tile[height ,width];
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if(x < Width && y < Height)
        {
            Tiles[y, x] = tile;
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x < Width && y < Height)
        {
            return Tiles[y, x];
        }
        else
        {
            return null;
        }
    }

    public override string ToString()
    {
        string rep = "";
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                rep = rep + GetTile(x, y).ToString() + "\r\n";
            }
        }
        return rep;
    }

}
