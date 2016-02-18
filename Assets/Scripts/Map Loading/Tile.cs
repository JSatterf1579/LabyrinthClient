using UnityEngine;
using System.Collections;

public class Tile {

    int xPos;
    int yPos;
    int rotation;
    public string type;

    public Tile(int xPos, int yPos, int rotation, string type)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.type = type;
    }

	
}
