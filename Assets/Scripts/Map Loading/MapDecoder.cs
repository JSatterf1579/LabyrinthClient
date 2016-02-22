using UnityEngine;
using System.Collections;

public class MapDecoder{

    private JSONObject serializedMap;

    private Map decodedMap;

    private bool isMapDecoded;

    public MapDecoder(JSONObject map)
    {
        isMapDecoded = false;
        serializedMap = map;
    }

    public void decodeMap()
    {
        if (isMapDecoded)
        {
            return;
        }

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
        isMapDecoded = true;
        decodedMap = workingMap;
    }

    public Map getMap()
    {
        return decodedMap;
    }


}
