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
        Debug.Log(currentMap.ToString());
    }

    private Map storeRecievedMap(JSONObject response)
    {
        if(response.GetField("status").n == 200)
        {
            Debug.Log("Got proper response");
            MapDecoder decoder = new MapDecoder(response.GetField("map"));
            decoder.decodeMap();
            return decoder.getMap();
        }
        Debug.Log("Got non 200 return. Did something go wrong?");
        return currentMap;
        
    }

    public Map getCurrentMap()
    {
        return currentMap;
    }
}
