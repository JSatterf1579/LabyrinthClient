using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using System;
using System.Linq;

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

	private GameState _gameState;

	public GameState gameState { 
		get {
			return _gameState;
		} 
		private set {
			_gameState = value;
			if (GameState.HeroTurn == value) {
				PlayerTurn = PlayerType.Heroes;
			} else if (GameState.ArchitectTurn == value) {
				PlayerTurn = PlayerType.Architect;
			}
		} 
	}

    private SocketIOComponent socket;

    private Dictionary<int, JSONObject> queuedPackets;

    public Dictionary<string, MapObject> MapObjects = new Dictionary<string, MapObject>();

    public IEnumerable<MapObject> AlliedMapObjects {
        get {
            return MapObjects.Where(x =>
                x.Value.controllerID == GameManager.instance.Username
            ).Select(x => x.Value);
        }
    }

    private Dictionary<string, List<Action<JSONChangeInfo>>> JSONChangeActions = new Dictionary<string, List<Action<JSONChangeInfo>>>();

    public static JSONObject MatchState {
        get; private set;
    }

    public static void SetInitialMatchState(JSONObject obj) {
        if(instance != null) {
            Debug.LogError("Error: attempted to SetInitialMatchState but there is already an initalized MatchManager!");
            return;
        }

        MatchState = obj;
    }

	void Start () {
	    if (GameManager.instance != null && MatchState != null) {
            if (instance != null) {
                Debug.LogError("Something went wrong in match singleton creation");
                Destroy(this);
                return;
            } else {
                MatchManager.instance = this;
            }
            queuedPackets = new Dictionary<int, JSONObject>();
	        socket = GameManager.instance.getSocket();
	        MatchIdentifier = MatchState.GetField("match_identifier").str;

			UpdatePlayers(MatchState);

			string gstate = MatchState["game_state"].str;
			if ("hero_turn".Equals(gstate)) {
				gameState = GameState.HeroTurn;
			} else if ("architect_turn".Equals(gstate)) {
				gameState = GameState.ArchitectTurn;
			}

			TurnNumber = (int)MatchState["turn_number"].n;
			RegisterJSONChangeAction("/turn_number", UpdateTurn);
			RegisterJSONChangeAction("/game_state", UpdateGameState);

	        JSONDecoder.DecodeMap(MatchState.GetField("map"), Map.Current);
            JSONDecoder.DecodeHeroes(MatchState.GetField("board_objects"), manager);
            socket.On("game_update", GameUpdate);
        }
	    else
	    {
            Debug.LogError("You got in here without a game manager or match start data! How did you do that!");
            Destroy(this.gameObject);
            
	    }
	
	}

    public void EndTurn()
    {
        foreach (MapObject o in MapObjects.Values)
        {
            if (o.ownerID == GameManager.instance.Username)
            {
                JSONObject payload = new JSONObject();
                payload.AddField("character_id", o.UUID);
                SendAction("pass", payload);
            }
        }
    }

    void Update() {        
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

#pragma warning disable 162
    private void GameUpdate(SocketIOEvent e)
    {
        JSONObject data = e.data;
        Debug.Log(data);
        if (true)
        {
            SeqNumber++;
            ProcessAction(data.GetField("action"));
            ApplyDiff(data["new_state"]);
        }
        else
        {
            Debug.Log("Queued packet for future processing");
            queuedPackets.Add((int)data.GetField("new_state").GetField("current_sequence").n, data);
        }
    }
#pragma warning restore 162
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
            tiles.Add(Map.Current.GetTileAtPosition(xPos, yPos));
        }
        MapObject target = MapObjects[characterID];
        Map.Current.MoveMapObject(target, tiles);
    }

    #region state diff handling
    public void RegisterJSONChangeAction(string watchedElement, Action<JSONChangeInfo> del) {
        if (!JSONChangeActions.ContainsKey(watchedElement)) {
            JSONChangeActions.Add(watchedElement, new List<Action<JSONChangeInfo>>());
        }

        JSONChangeActions[watchedElement].Add(del);
        Debug.Log("JSON change event registered for item " + watchedElement);
    }

    public void RemoveJSONChangeAction(string watchedElement, Action<JSONChangeInfo> del) {
        if (!JSONChangeActions.ContainsKey(watchedElement)) {
            Debug.LogError("Attempted to remove a JSON change action that does not exist");
            return;
        }
        var list = JSONChangeActions[watchedElement];
        if (!list.Contains(del)) {
            Debug.LogError("Attempted to remove a JSON change action that does not exist");
            return;
        }
        list.Remove(del);
        if (list.Count == 0) {
            JSONChangeActions.Remove(watchedElement);
        }
    }

    private void FireJSONChangeEvent(JSONChangeInfo change) {
        Debug.Log("Firing JSON change event for element " + change.Path);
        if (JSONChangeActions.ContainsKey(change.Path)) {
            var list = JSONChangeActions[change.Path];
            foreach (var action in list) {
                action(change);
            }
        }
    }

    private void ApplyDiff(JSONObject diff) {
        //Debug.Log("applying diff...");
        ApplyRemovals(diff["removed"], MatchState, "");
        ApplyAdditions(diff["added"], MatchState, "");
        ApplyChanges(diff["changed"], MatchState, "");
        //Debug.Log("New match state: " + MatchState);
    }

    private void ApplyRemovals(JSONObject diff, JSONObject master, string path) {
        if (diff.IsNull) return;
        if (diff.IsObject) {
            foreach (string key in diff.keys) {
                JSONObject cur = diff[key];
                if (cur.isContainer) {
                    ApplyRemovals(cur, master[key], path+"/"+key);
                } else if (cur.IsString && cur.str == "DELETED") {
                    JSONObject oldVal = master[key];
                    var change = new JSONChangeInfo(JSONChangeInfo.ChangeType.DELETED, path + "/" + key, oldVal, null);
                    master.RemoveField(key);
                    FireJSONChangeEvent(change);
                } else {
                    Debug.LogError("Bad data encountered in JSON diff while applying removals");
                }
            }
        } else if (diff.IsArray) {
            Debug.LogError("Bad data encountered in JSON diff while applying removals; arrays are not allowed in diffs");
        } else {
            Debug.LogError("Bad data encountered in JSON diff while applying removals");
        }
    }

    private void ApplyAdditions(JSONObject diff, JSONObject master, string path) {
        if (diff.IsNull) return;
        if (diff.IsObject) {
            foreach (string key in diff.keys) {
                JSONObject cur = diff[key];
                if (cur.IsObject) {
                    if (!master.HasField(key)) {
                        var nobj = new JSONObject();
                        master.AddField(key, nobj);
                        FireJSONChangeEvent(new JSONChangeInfo(JSONChangeInfo.ChangeType.ADDED, path + "/" + key, null, nobj));
                    }
                    ApplyAdditions(cur, master[key], path + "/" + key);
                } else if (!master.HasField(key)) {
                    master.AddField(key, cur);
                    FireJSONChangeEvent(new JSONChangeInfo(JSONChangeInfo.ChangeType.ADDED, path + "/" + key, null, cur));
                } else {
                    Debug.LogError("Bad data encountered in JSON diff while applying additions");
                }
            }
        } else if (diff.IsArray) {
            Debug.LogError("Bad data encountered in JSON diff while applying additions; arrays are not allowed in diffs");
        } else {
            Debug.LogError("Bad data encountered in JSON diff while applying additions");
        }
    }

    private void ApplyChanges(JSONObject diff, JSONObject master, string path) {
        if (diff.IsNull) return;
        if (diff.IsObject) {
            foreach (string key in diff.keys) {
                JSONObject cur = diff[key];
                if (cur.IsObject) {
                    if (!master.HasField(key)) {
                        var nobj = new JSONObject();
                        master.AddField(key, nobj);
                        FireJSONChangeEvent(new JSONChangeInfo(JSONChangeInfo.ChangeType.ADDED, path + "/" + key, null, nobj));
                    }
                    ApplyChanges(cur, master[key], path + "/" + key);
                } else if (master.HasField(key)) {
                    var oldVal = master[key];
                    master.SetField(key, cur);
                    FireJSONChangeEvent(new JSONChangeInfo(JSONChangeInfo.ChangeType.CHANGED, path + "/" + key, oldVal, cur));
                } else {
                    Debug.LogError("Bad data encountered in JSON diff while applying changes");
                }
            }
        } else if (diff.IsArray) {
            Debug.LogError("Bad data encountered in JSON diff while applying changes; arrays are not allowed in diffs");
        } else {
            Debug.LogError("Bad data encountered in JSON diff while applying changes");
        }
    }
#endregion

	private void UpdateGameState(JSONChangeInfo info) {
		if (info.Type == JSONChangeInfo.ChangeType.CHANGED) {
			string gState = info.NewValue.str;
			if ("hero_turn".Equals(gState)) {
				gameState = GameState.HeroTurn;
			} else if ("architect_turn".Equals(gState)) {
				gameState = GameState.ArchitectTurn;
			}
		}
	}

	private void UpdateTurn(JSONChangeInfo info) {
		if (info.Type == JSONChangeInfo.ChangeType.CHANGED) {
			TurnNumber = (int)info.NewValue.n;
		}
	}

	private void UpdatePlayers(JSONObject data) {
		if (data.GetField("players").GetField("heroes").str.Equals(GameManager.instance.Username)) {
			MyPlayerType = PlayerType.Heroes;
			OpponentName = data.GetField("players").GetField("architect").str.Split('_')[0];
			OpponentType = PlayerType.Architect;
		} else {
			MyPlayerType = PlayerType.Architect;
			OpponentName = data.GetField("players").GetField("heroes").str.Split('_')[0];
			OpponentType = PlayerType.Heroes;
		}
		Debug.Log("Updated Players");
	}
}

public enum PlayerType {Heroes, Architect, None};

public enum GameState {HeroTurn, ArchitectTurn, GameOver};
