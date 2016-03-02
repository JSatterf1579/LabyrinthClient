using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class MapLoader : MonoBehaviour {

    public Button mapButton;
    public Map mapRenderer;
    private SocketIOComponent socket;
    private JSONObject currentMapJSON;
    public bool MapLoaded {
        get;
        private set;
    }

    // Use this for initialization
    void Start () {
        MapLoaded = false;
        socket = GameManager.instance.getSocket();
        
        mapButton.onClick.AddListener(requestMap);

    }



    private void requestMap() {
        JSONObject data = new JSONObject();
        data.AddField("x", 20);
        data.AddField("y", 20);
        Debug.Log("Getting map");
        socket.Emit("map", data, recieveMap);
    }

    private void recieveMap(JSONObject response) {
        Debug.Log("Recieved Response");
        Debug.Log(response.ToString());
        storeRecievedMap(response.list[0]);
    }

    private void decodeMap(JSONObject serializedMap) {
        if(mapRenderer == null) {
            Debug.LogError("No MapRenderer was given, cannot decode map!");
            return;
        }

        int x = (int)serializedMap.GetField("size").GetField("x").n;
        int y = (int)serializedMap.GetField("size").GetField("y").n;

        mapRenderer.DestroyMap();
        mapRenderer.InitalizeMap(x, y);
        
        JSONObject serializedTiles = serializedMap.GetField("tiles");
        for (int i = 0; i < serializedTiles.list.Count; i++)
        {
            int tileX = (int)serializedTiles.list[i].GetField("x").n;
            int tileY = (int)serializedTiles.list[i].GetField("y").n;
            int rotation = (int)serializedTiles.list[i].GetField("rotation").n;
            string type = serializedTiles.list[i].GetField("terrain").str;
            bool isObstacle = serializedTiles.list[i].GetField("is_obstacle").b;
            mapRenderer.InstantiateTile(tileX, tileY, type, rotation, isObstacle);
        }
    }

    private void storeRecievedMap(JSONObject response) {
        if (response.GetField("status").n.ToString() == "200") {
            Debug.Log("Got proper response");
            currentMapJSON = response.GetField("map");
            decodeMap(currentMapJSON);
            MapLoaded = true;
        } else {
            Debug.LogError("Got non 200 return for map request. Did something go wrong?");
        }
        
    }
}
