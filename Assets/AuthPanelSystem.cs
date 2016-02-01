using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SocketIO;
using System.Collections.Generic;

public class AuthPanelSystem : MonoBehaviour {

	private string username;
	private string password;

	public InputField usernameField;
	public InputField passField;
	public Button registerButton;
	public Button loginButton;
	public Button roomButton;

	private SocketIOComponent socket;

	private bool room = false;

	// Use this for initialization
	void Start () {
		socket = GameManager.instance.getSocket();
		socket.On ("room_ping", (SocketIOEvent data) => {
			Debug.Log (data.data.ToString());
		});
		usernameField.onValueChange.AddListener(updateUsername);
		passField.onValueChange.AddListener(updatePassword);
		registerButton.onClick.AddListener(register);
		loginButton.onClick.AddListener (login);
		roomButton.onClick.AddListener (toggleRoom);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void updateUsername(string arg0){
		this.username = arg0;
	}

	private void updatePassword(string arg0){
		this.password = arg0;
	}

	private void register(){
		Debug.Log ("Registering");
		sendPacket (true);
	}

	private void login(){
		sendPacket (false);
	}

	private bool sendPacket(bool isRegister){
		Dictionary<string,string> data = new Dictionary<string,string>();
		data["username"] = this.username;
		data ["password"] = this.password;
		if (isRegister) {
			data ["password_confirm"] = this.password;
			socket.Emit ("register", new JSONObject (data), processLogin);
			return true;
		} else {

			socket.Emit ("login", new JSONObject (data), processLogin);
			return true;
		}
	}

	private void toggleRoom(){
		if (room) {
			leaveRoom ();
			room = false;
		} else {
			joinRoom();
			room = true;
		}
	}

	private void joinRoom(){
		socket.Emit ("join", new JSONObject() , processLogin);
	}

	private void leaveRoom(){
		socket.Emit ("leave", new JSONObject() , processLogin);
	}

	private void processLogin(JSONObject response){
		Debug.Log (response.ToString ());
	}
}
