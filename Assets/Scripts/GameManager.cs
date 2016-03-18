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
		GameObject s = GameObject.Find ("SocketIO");
		return s.GetComponent<SocketIOComponent>();
	}
}
