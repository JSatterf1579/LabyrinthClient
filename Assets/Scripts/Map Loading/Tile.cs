using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public int YPos { get; private set; }
    public int XPos { get; private set; }
    public int Rotation { get; private set; }
    public string Type;

    private bool _isInitialized = false;

    public Tile(int xPos, int yPos, int rotation, string type)
    {
        this.XPos = xPos;
        this.YPos = yPos;
        this.Type = type;
        this.Rotation = rotation;
        this._isInitialized = true;
    }

    public void Awake()
    {

    }

    public void Init(int xPos, int yPos, int rotation)
    {
        if (!_isInitialized)
        {
            this.XPos = xPos;
            this.YPos = yPos;
            this.Rotation = rotation;
            this._isInitialized = true;
        }
    }

    public override string ToString()
    {
        return "Position: " + XPos + "," + YPos + ":" + Rotation + "\r\n" + "Type: " + Type;
    }


}
