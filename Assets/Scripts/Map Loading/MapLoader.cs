using System;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class MapLoader : MonoBehaviour {

    public Button mapButton;
    private SocketIOComponent socket;
    private Map currentMap;
    private JSONObject currenMapJSON;
    public bool MapLoaded {
        get;
        private set;
    }

    // Use this for initialization
    void Start () {
        MapLoaded = false;
        socket = GameManager.instance.getSocket();
        socket.On("room_ping", (SocketIOEvent data) => {
            Debug.Log(data.data.ToString());
        });
        mapButton.onClick.AddListener(requestMap);

    }



    private void requestMap()
    {
        JSONObject data = new JSONObject();
        data.AddField("x", 20);
        data.AddField("y", 20);
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
        if(Math.Abs(response.GetField("status").n - 200) < .000001)
        {
            Debug.Log("Got proper response");
            MapDecoder decoder = new MapDecoder(response.GetField("map"));
            currenMapJSON = response.GetField("map");
            decoder.decodeMap();
            MapLoaded = true;
            return decoder.getMap();
        }
        Debug.Log("Got non 200 return. Did something go wrong?");
        return currentMap;
        
    }

    public Map getCurrentMap()
    {
        return currentMap;
    }

    public JSONObject getCurrentMapJson()
    {
        return currenMapJSON;
    }
}
