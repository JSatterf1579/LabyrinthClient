using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class JSONEncoder {


    public static JSONObject EncodeMove(Unit unit, List<Tile> path)
    {
        JSONObject data = new JSONObject();
        data.AddField("character_id", unit.UUID);
        JSONObject pathData = new JSONObject(JSONObject.Type.ARRAY);
        data.AddField("path", pathData);
        for(int i = 0; i < path.Count; i++)
        {
            JSONObject pathNode = new JSONObject();
            pathNode.AddField("x", path[i].XPos);
            pathNode.AddField("y", path[i].YPos);
            pathData.Add(pathNode);
        }
        return data;
    }

    public static JSONObject EncodeAttack(Unit unit, List<Tile> targets)
    {
        JSONObject data = new JSONObject();
        data.AddField("attacker_id", unit.UUID);
        JSONObject targetList = new JSONObject(JSONObject.Type.ARRAY);
        data.AddField("targets", targetList);
        foreach (Tile target in targets)
        {
            JSONObject targetPos = new JSONObject();
            targetPos.AddField("x", target.XPos);
            targetPos.AddField("y", target.YPos);
            targetList.Add(targetPos);
        }
        return data;
    }

}
