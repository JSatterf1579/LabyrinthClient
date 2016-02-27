using UnityEngine;
using System.Collections;
using SocketIO;

public class GameManager : MonoBehaviour {

    //Transisition to 
    public string Username { get;  set; }

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
	
	// Update is called once per frame
	void Update () {
	
	}

	public SocketIOComponent getSocket(){
		GameObject s = GameObject.Find ("SocketIO");
		return s.GetComponent<SocketIOComponent>();
	}
}
