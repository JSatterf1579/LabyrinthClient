using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System.Collections;

public class HeroLoader : MonoBehaviour {

    public Button MapButton;
    public Button AuthButton;
    public Button AcctButton;
    public string testUser;
    public string testPass;
    private SocketIOComponent _socket;

    // Use this for initialization
    void Start () {
	
	}

    void Awake()
    {
        _socket = GameManager.instance.getSocket();
        MapButton.onClick.AddListener(RequestHero);
        AcctButton.onClick.AddListener(CreateAcct);
        AuthButton.onClick.AddListener(Auth);
        
    }

    private void CreateAcct()
    {
        MakeUser();
    }

    private void Auth()
    {
        
        Login();
    }

    private void RequestHero()
    {
        
        JSONObject data = new JSONObject();
        Debug.Log("Getting Hero example data");
        _socket.Emit("get_heroes", data, RecieveHero);
    }

    private void RecieveHero(JSONObject response)
    {
        Debug.Log("Recieved hero data");
        Debug.Log(response.ToString());
    }

    private void MakeUser()
    {
        JSONObject data = new JSONObject();
        data.AddField("username", testUser);
        data.AddField("password", testPass);
        data.AddField("password_confirm", testPass);
        _socket.Emit("register", data, RecieveMakeUser);
    }

    private void RecieveMakeUser(JSONObject response)
    {
        response = response.list[0];
        if (response.GetField("status").n == 200)
        {
            Debug.Log("User Created");
            Debug.Log(response.ToString());
        }
        else
        {
            Debug.Log("User Creation Failed");
            Debug.Log(response.ToString());
        }
    }

    private void Login()
    {
        JSONObject data = new JSONObject();
        data.AddField("username", testUser);
        data.AddField("password", testPass);
        _socket.Emit("login", data, RecieveLoginUser);
    }

    private void RecieveLoginUser(JSONObject response)
    {
        response = response.list[0];
        if (response.GetField("status").n == 200)
        {
            Debug.Log("User Login");
            Debug.Log(response.ToString());
        }
        else
        {
            Debug.Log("User Login Failed");
            Debug.Log(response.ToString());
        }
    }
}
