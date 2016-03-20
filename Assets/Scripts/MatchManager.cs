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

	public string OpponentName { get; private set; }

	public PlayerType OpponentType { get; private set; }

    public int SeqNumber { get; private set; }

	public int TurnNumber { get; private set; }

	public PlayerType PlayerTurn { get; private set; }

	public bool MyTurn {
		get { 
			return PlayerTurn.Equals(MyPlayerType); 
		}
	}

	public PlayerType MyPlayerType { get; private set; }

	private string _GameState;

	public string GameState { 
		get {
			return _GameState;
		} 
		private set {
			_GameState = value;
			if ("hero_turn".Equals(value)) {
				PlayerTurn = PlayerType.Heroes;
			} else if ("architect_turn".Equals(value)) {
				PlayerTurn = PlayerType.Architect;
			}
		} 
	}

    private SocketIOComponent socket;

    private Dictionary<int, JSONObject> queuedPackets; 

	// Use this for initialization
	void Start () {
	    if (GameManager.instance != null && GameManager.instance.MatchData != null)
	    {
            queuedPackets = new Dictionary<int, JSONObject>();
	        socket = GameManager.instance.getSocket();
			JSONObject data = GameManager.instance.MatchData;
	        MatchIdentifier = data.GetField("match_identifier").str;

			Debug.Log("match manager data:");
			Debug.Log(data);

			UpdatePlayers(data);

			UpdateTurn(data);
			UpdateGameState(data);

	        JSONDecoder.DecodeMap(data.GetField("map"), map);
            JSONDecoder.DecodeHeroes(data.GetField("board_objects"), manager);
            socket.On("game_update", GameUpdate);
            if (instance != null)
            {
                Debug.LogError("Something went wrong in match singleton creation");
                Destroy(this);
                return;
            }
            else {
                MatchManager.instance = this;
            }
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
			if (data.HasField("game_state")) {
				UpdateGameState(data);
			}
			if (data.HasField("turn_number")) {
				UpdateTurn(data);
			}
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
        for (int i = 0; i < path.Count; i++)
        {
            int xPos = (int) path[i].GetField("x").n;
            int yPos = (int) path[i].GetField("y").n;
            Unit target = manager.heroList[characterID];
            map.MoveMapObject(target, xPos, yPos);
        }
    }

	private void UpdateGameState(JSONObject data) {
		GameState = data.GetField("game_state").str;
	}

	private void UpdateTurn(JSONObject data) {
		TurnNumber = (int)data.GetField("turn_number").n;
	}

	private void UpdatePlayers(JSONObject data) {
		if (data.GetField("players").GetField("heroes").str.Equals(GameManager.instance.Username)) {
			MyPlayerType = PlayerType.Heroes;
			OpponentName = data.GetField("players").GetField("architect").str;
			OpponentType = PlayerType.Architect;
		} else {
			MyPlayerType = PlayerType.Architect;
			OpponentName = data.GetField("players").GetField("heroes").str;
			OpponentType = PlayerType.Heroes;
		}
		Debug.Log("Updated Players");
	}
}

public enum PlayerType {Heroes, Architect, None};
