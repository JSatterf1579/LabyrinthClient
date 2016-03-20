using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SocketIO;
using Debug = UnityEngine.Debug;

public class MatchManager : MonoBehaviour
{

    public Map map;

    public HeroManager manager;

    public static MatchManager instance;

    public string MatchIdentifier { get; private set; }

    public string HeroPlayer { get; private set; }
    public string ArchitectPlayer { get; private set; }

    public int SeqNumber { get; private set; }

    public string GameState { get; private set; }

    private SocketIOComponent socket;

    private Dictionary<int, JSONObject> queuedPackets;

    public Dictionary<string, MapObject> MapObjects = new Dictionary<string, MapObject>();

	// Use this for initialization
	void Start () {
	    if (GameManager.instance != null && GameManager.instance.MatchData != null)
	    {
            if (instance != null) {
                Debug.LogError("Something went wrong in match singleton creation");
                Destroy(this);
                return;
            } else {
                MatchManager.instance = this;
            }
            queuedPackets = new Dictionary<int, JSONObject>();
	        socket = GameManager.instance.getSocket();
	        MatchIdentifier = GameManager.instance.MatchData.GetField("match_identifier").str;

	        JSONDecoder.DecodeMap(GameManager.instance.MatchData.GetField("map"), map);
            JSONDecoder.DecodeHeroes(GameManager.instance.MatchData.GetField("board_objects"), manager);
            socket.On("game_update", GameUpdate);
        }
	    else
	    {
            Debug.LogError("You got in here without a game manager or match start data! How did you do that!");
            
	    }
	
	}

    void Update()
    {
        if (queuedPackets.ContainsKey(SeqNumber + 1))
        {
            SeqNumber++;
            JSONObject data = queuedPackets[SeqNumber];
            queuedPackets.Remove(SeqNumber);
            ProcessAction(data);
        }
    }

    public void SendAction(string actionType, JSONObject payload)
    {
        payload.AddField("type", actionType);
        socket.Emit("game_action", payload, ActionCallback);
    }

    private void ActionCallback(JSONObject response)
    {
        Debug.Log(response);
    }

    private void GameUpdate(SocketIOEvent e)
    {
        JSONObject data = e.data;
        Debug.Log(data);
        if (true)
        {
            SeqNumber++;
            ProcessAction(data.GetField("action"));
        }
        else
        {
            Debug.Log("Queued packet for future processing");
            queuedPackets.Add((int)data.GetField("new_state").GetField("current_sequence").n, data);
        }
    }

    private void ProcessAction(JSONObject action)
    {
        if (action.GetField("type").str.Equals("move"))
        {
            MoveCharacter(action);
        }
    }

    private void MoveCharacter(JSONObject move)
    {
        List<JSONObject> path = move.GetField("path").list;
        string characterID = move.GetField("character").str;
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < path.Count; i++)
        {
            int xPos = (int) path[i].GetField("x").n;
            int yPos = (int) path[i].GetField("y").n;
            tiles.Add(map.GetTileAtPosition(xPos, yPos));
        }
        MapObject target = MapObjects[characterID];
        map.MoveMapObject(target, tiles);
    }
}
