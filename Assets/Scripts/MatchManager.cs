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

    public string MatchIdentifier { get; private set; }

    public string HeroPlayer { get; private set; }
    public string ArchitectPlayer { get; private set; }

    public int SeqNumber { get; private set; }

    public string GameState { get; private set; }

    private SocketIOComponent socket;

    private Dictionary<int, JSONObject> queuedPackets; 

	// Use this for initialization
	void Start () {
	    if (GameManager.instance != null && GameManager.instance.MatchData != null)
	    {
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

    private void GameUpdate(SocketIOEvent e)
    {
        JSONObject data = e.data;
        if ((int) data.GetField("new_state").GetField("current_sequence").n == SeqNumber + 1)
        {
            SeqNumber++;
            ProcessAction(data.GetField("action"));
        }
        else
        {
            queuedPackets.Add((int)data.GetField("new_state").GetField("current_sequence").n, data);
        }
    }

    private void ProcessAction(JSONObject action)
    {
        
    }
}
