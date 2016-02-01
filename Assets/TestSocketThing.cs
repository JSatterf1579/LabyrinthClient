using UnityEngine;
using System.Collections;
using SocketIO;

public class TestSocketThing : MonoBehaviour {

	SocketIOComponent socket;

	// Use this for initialization
	void Start () {
		GameObject s = GameObject.Find ("SocketIO");
		socket = s.GetComponent<SocketIOComponent>();
	}
	
	// Update is called once per frame
	void Update () {
		socket.On("beep", (data)=>{
			Debug.Log (data);
		});
	}
}