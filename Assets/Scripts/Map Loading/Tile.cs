public class Tile {

    public int xPos;
    public int yPos;
    public int rotation;
    public string type;

    public Tile(int xPos, int yPos, int rotation, string type)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.type = type;
    }

    public override string ToString()
    {
        return "Position: " + xPos + "," + yPos + ":" + rotation + "\r\n" + "Type: " + type;
    }

	
}
