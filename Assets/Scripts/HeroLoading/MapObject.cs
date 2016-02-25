using UnityEngine;
using System.Collections;

public abstract class MapObject : MonoBehaviour {

    public string ownerID;
    public string controllerID;
    public string UUID;

    public int posX;
    public int posY;

    public void Init(string ownerID, string contrllerID, string UUID, int x, int y)
    {
        this.ownerID = ownerID;
        this.controllerID = contrllerID;
        this.UUID = UUID;
        posX = x;
        posY = y;
    }

}
