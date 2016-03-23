using System;
using UnityEngine;
using System.Collections;
using SocketIO;

public class GameManager : MonoBehaviour
{


    public string Username;

    public bool InMatch;

    public static GameManager instance;

    private SocketIOComponent socket = null;

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);
		if (instance != null) {
			Destroy (this);
			return;
		} else {
			GameManager.instance = this;
		}
	}

	public SocketIOComponent getSocket(){
        if (socket == null) {
            GameObject s = GameObject.Find("SocketIO");
            socket = s.GetComponent<SocketIOComponent>();
        }
        return socket;
	}
}
