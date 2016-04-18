using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.SceneManagement;

public class MyHeroes : MonoBehaviour {

    private SocketIOComponent socket;

    // Use this for initialization
    void Start () {
	
	}

    void Awake()
    {
        socket = GameManager.instance.getSocket();
        JSONObject data = new JSONObject();
        socket.Emit("get_heroes", data, GetHeroes);
    }
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}


    private void GetHeroes(JSONObject response)
    {
        Debug.Log(response);
    }
}
