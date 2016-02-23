using UnityEngine;
using System.Collections;

public class MapDecoder{

    

    

    

    public static Map decodeMap(JSONObject serializedMap)
    {
        

        int x = (int)serializedMap.GetField("size").GetField("x").n;
        int y = (int)serializedMap.GetField("size").GetField("y").n;

        Map workingMap = new Map(x, y);
        JSONObject serializedTiles = serializedMap.GetField("tiles");
        for (int i = 0; i < serializedTiles.list.Count; i++)
        {
            int tileX = (int)serializedTiles.list[i].GetField("position").list[0].n;
            int tileY = (int)serializedTiles.list[i].GetField("position").list[1].n;
            int rotation = (int)serializedTiles.list[i].GetField("rotation").n;
            string type = serializedTiles.list[i].GetField("terrain").str;
            workingMap.SetTile(tileX, tileY, new Tile(tileX, tileY, rotation, type));
        }
        return workingMap;
    }

    


}
