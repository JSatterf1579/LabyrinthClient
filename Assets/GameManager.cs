using UnityEngine;
using System.Collections;
using SocketIO;

public class GameManager : MonoBehaviour {

	private string username;

	public static GameManager instance;

	// Use this for initialization
	void Awake () {
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
