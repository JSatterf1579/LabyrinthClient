using UnityEngine;

public class Tile : MonoBehaviour
{
    public int YPos { get; private set; }
    public int XPos { get; private set; }
    public int Rotation { get; private set; }
    public string Type { get; private set; }

    public void Init(int xPos, int yPos, int rotation, string type)
    {
        this.XPos = xPos;
        this.YPos = yPos;
        this.Rotation = rotation;
        this.Type = type;
    }

    public override string ToString()
    {
        return "Position: " + XPos + "," + YPos + ":" + Rotation + "\r\n" + "Type: " + Type;
    }


}
