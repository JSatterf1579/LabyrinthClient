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
	
}
