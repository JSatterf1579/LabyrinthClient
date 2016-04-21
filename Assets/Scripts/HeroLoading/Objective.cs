using UnityEngine;
using System.Collections;

public class Objective : MapObject {

    public void Init(string id, int x, int y, bool blocksMovement)
    {
        Init("Artifact", null, null, id, x, y, blocksMovement);
    }
}
