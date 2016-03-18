using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SocketIO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AuthPanelSystem : MonoBehaviour {

	private string username;
	private string password;

	public InputField usernameField;
	public InputField passField;
	public Button registerButton;
	public Button loginButton;
	public Button roomButton;

    public GameObject loginSuccessModal;
    public Button toMenuButton;

	private SocketIOComponent socket;

	private bool room = false;

	// Use this for initialization
	void Start ()
    { 
		socket = GameManager.instance.getSocket();
		usernameField.onValueChange.AddListener(updateUsername);
		passField.onValueChange.AddListener(updatePassword);
		registerButton.onClick.AddListener(register);
		loginButton.onClick.AddListener (login);
		roomButton.onClick.AddListener (toggleRoom);
        toMenuButton.onClick.AddListener(toMenu);
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

    private void toMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

	private bool sendPacket(bool isRegister){
		Dictionary<string,string> data = new Dictionary<string,string>();
		data["username"] = this.username;
		data ["password"] = this.password;
		if (isRegister) {
			data ["password_confirm"] = this.password;
			socket.Emit ("register", new JSONObject (data), ProcessRegister);
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

    private void ProcessRegister(JSONObject response)
    {
        Debug.Log(response.ToString());
        if (response.list[0].GetField("status").n == 200)
        {
            GameManager.instance.Username = this.username;
            //loginSuccessModal.SetActive(true);
        }
    }

    private void processLogin(JSONObject response){
		Debug.Log (response.ToString ());
	    if (response.list[0].GetField("status").n == 200)
	    {
	        GameManager.instance.Username = this.username;
	        loginSuccessModal.SetActive(true);
	    }
	}
}
