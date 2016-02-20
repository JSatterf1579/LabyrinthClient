using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using System;

public class MapLoader : MonoBehaviour {

    public Button mapButton;
    private SocketIOComponent socket;
    private Map currentMap;

    // Use this for initialization
    void Start () {
        socket = GameManager.instance.getSocket();
        socket.On("room_ping", (SocketIOEvent data) => {
            Debug.Log(data.data.ToString());
        });
        mapButton.onClick.AddListener(requestMap);

    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void requestMap()
    {
        JSONObject data = new JSONObject();
        data.AddField("x", 2);
        data.AddField("y", 2);
        Debug.Log("Getting map");
        socket.Emit("map", data, recieveMap);
        

    }

    private void recieveMap(JSONObject response)
    {
        Debug.Log("Recieved Response");
        Debug.Log(response.ToString());
        currentMap = storeRecievedMap(response.list[0]);
    }

    private Map storeRecievedMap(JSONObject response)
    {
        if(response.GetField("status").n == 200)
        {
            Debug.Log("Got proper response");
            return buildMapFromJSON(response.GetField("map"));
        }
        return currentMap;
        
    }

    private Map buildMapFromJSON(JSONObject serializedMap)
    {
        int x = (int)serializedMap.GetField("size").GetField("x").n;
        int y = (int)serializedMap.GetField("size").GetField("y").n;

        Map workingMap =  new Map(x, y);
        JSONObject serializedTiles = serializedMap.GetField("tiles");
        for(int i = 0; i < serializedTiles.list.Count; i++)
        {
            int tileX = (int)serializedTiles.list[i].GetField("position").list[0].n;
            int tileY = (int)serializedTiles.list[i].GetField("position").list[1].n;
            int rotation = (int)serializedTiles.list[i].GetField("rotation").n;
            string type = serializedTiles.list[i].GetField("terrain").str;
            workingMap.setTile(tileX, tileY, new Tile(tileX, tileY, rotation, type));
        }

        return workingMap;
    }

    public Map getCurrentMap()
    {
        return currentMap;
    }
}
