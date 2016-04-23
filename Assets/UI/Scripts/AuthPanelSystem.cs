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
	public Text errorMessage;
	public Color fieldErrorColor;
	public Color fieldSuccessColor;

	private Color defaultFieldColor;
	private Image passImage;
	private Image userImage;

	private SocketIOComponent socket;

	// Use this for initialization
	void Start () { 
		socket = GameManager.instance.getSocket();
		defaultFieldColor =  usernameField.GetComponent<Image>().color;
		passImage = passField.GetComponent<Image>();
		userImage = usernameField.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {

		if(usernameField.isFocused && Input.GetKey(KeyCode.Tab) && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
			passField.ActivateInputField();
		}

		if(Input.GetKeyDown(KeyCode.Return)) {
			login();
		}

		if(passField.isFocused && Input.GetKey(KeyCode.Tab) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
			usernameField.ActivateInputField();
		}
	}

	public void updateUsername(InputField arg0){
		this.username = arg0.text;
	}

	public void updatePassword(InputField arg0){
		this.password = arg0.text;
	}

	public void register() {
		sendPacket (true);
	}

	public void login() {
		sendPacket (false);
	}

    private void toMenu() {
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

	private void ProcessRegister(JSONObject response) {
		clearError();
        Debug.Log(response.ToString());
		float status = response.list[0].GetField("status").n;
        if (status == 200)
        {
            GameManager.instance.Username = this.username;
			string message = "Successfully registered " + this.username + ".";
			success(message);
		} else {
			string message = response.list[0].GetField("message").str;
			error(message, usernameField);
		}
	}

    private void processLogin(JSONObject response) {
		clearError();
		Debug.Log (response.ToString ());
		float status = response.list[0].GetField("status").n;
	    if (status == 200) {
	        GameManager.instance.Username = this.username;
			toMenu();
		} else {
			string message = response.list[0].GetField("message").str;
			error(message, passField);
		}
	}

	private void error(string message, InputField field) {
		errorMessage.color = fieldErrorColor;
		errorMessage.text = message;
		field.text = "";
		field.ActivateInputField();
		field.GetComponent<Image>().color = fieldErrorColor;
	}

	private void success(string message) {
		errorMessage.color = fieldSuccessColor;
		errorMessage.text = message;
	}

	private void clearError() {
		errorMessage.text = "";
		userImage.color = defaultFieldColor;
		passImage.color = defaultFieldColor;
	}
}
