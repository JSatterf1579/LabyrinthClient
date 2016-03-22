using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using System;

public class MatchManager : MonoBehaviour
{
    public Map map;

    public HeroManager manager;

    public static MatchManager instance;

    public string MatchIdentifier { get; private set; }

    public string HeroPlayer { get; private set; }
    public string ArchitectPlayer { get; private set; }

    public int SeqNumber { get; private set; }

    private SocketIOComponent socket;

    private Dictionary<int, JSONObject> queuedPackets;

    public Dictionary<string, MapObject> MapObjects = new Dictionary<string, MapObject>();

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
	    if (GameManager.instance != null && MatchState != null)
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
	        MatchIdentifier = MatchState.GetField("match_identifier").str;

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
        Debug.Log("Fireing JSON change event for element " + change.Path);
        if (JSONChangeActions.ContainsKey(change.Path)) {
            var list = JSONChangeActions[change.Path];
            foreach (var action in list) {
                action(change);
            }
        }
    }

    private void ApplyDiff(JSONObject diff) {
        Debug.Log("applying diff...");
        ApplyRemovals(diff["removed"], MatchState, "");
        ApplyAdditions(diff["added"], MatchState, "");
        ApplyChanges(diff["changed"], MatchState, "");
        Debug.Log("New match state: " + MatchState);
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
}
