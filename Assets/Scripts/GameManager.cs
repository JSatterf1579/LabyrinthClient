using System;
using UnityEngine;
using System.Collections;
using SocketIO;

public class GameManager : MonoBehaviour
{


    public string Username;

    public bool InMatch { get; private set; }

    public JSONObject MatchData { get; private set; }

    public static GameManager instance;

    private SocketIOComponent socket = null;

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);
        DebugHUD.setValue("Username", Username);
		if (instance != null) {
			Destroy (this);
			return;
		} else {
			GameManager.instance = this;
		}
	}

    public void RecieveMatchData(JSONObject data)
    {
        InMatch = true;
        MatchData = data;
    }

	public SocketIOComponent getSocket(){
        if (socket == null) {
            GameObject s = GameObject.Find("SocketIO");
            socket = s.GetComponent<SocketIOComponent>();
        }
        return socket;
	}

    public void ApplyDiff(JSONObject diff) {
        Debug.Log("applying diff...");
        ApplyRemovals(diff["removed"], MatchData);
        ApplyAdditions(diff["added"], MatchData);
        ApplyChanges(diff["changed"], MatchData);
        Debug.Log("New match state: " + MatchData);
    }

    private void ApplyRemovals(JSONObject diff, JSONObject master) {
        if (diff.IsNull) return;
        if (diff.IsObject) {
            foreach(string key in diff.keys) {
                JSONObject cur = diff[key];
                if (cur.isContainer) {
                    ApplyRemovals(cur, master[key]);
                } else if(cur.IsString && cur.str == "DELETED") {
                    master.RemoveField(key);
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

    private void ApplyAdditions(JSONObject diff, JSONObject master) {
        if (diff.IsNull) return;
        if (diff.IsObject) {
            foreach (string key in diff.keys) {
                JSONObject cur = diff[key];
                if (cur.IsObject) {
                    if (!master.HasField(key)) {
                        master.AddField(key, new JSONObject());
                    }
                    ApplyAdditions(cur, master[key]);
                } else if (!master.HasField(key)) {
                    master.AddField(key, cur);
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

    private void ApplyChanges(JSONObject diff, JSONObject master) {
        if (diff.IsNull) return;
        if (diff.IsObject) {
            foreach (string key in diff.keys) {
                JSONObject cur = diff[key];
                if (cur.IsObject) {
                    if (!master.HasField(key)) {
                        master.AddField(key, new JSONObject());
                    }
                    ApplyChanges(cur, master[key]);
                } else if (master.HasField(key)) {
                    master.SetField(key, cur);
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
}
