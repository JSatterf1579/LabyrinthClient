using System;
using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.SceneManagement;

public class FindGame : MonoBehaviour
{

    public GameObject QueueingModal;

    private bool queued = false;
    private SocketIOComponent socket;

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        socket = GameManager.instance.getSocket();
    }
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}

    public void OnHeroes()
    {
        JSONObject data = new JSONObject();
        Debug.Log(data);
        data.AddField("queue_with_passbot", false);
        socket.Emit("queue_up_heroes", new JSONObject(), HeroesQueue);
    }

    private void HeroesQueue(JSONObject response)
    {
        Debug.Log(response);
        if (response.list[0].GetField("status").n == 200)
        {
            queued = true;
            QueueingModal.SetActive(true);
        }
    }

    public void OnArchitect()
    {
        throw new NotImplementedException();
    }

    public void OnDequeue()
    {
        QueueingModal.SetActive(false);
        queued = false;
    }

    private void OnMatch(SocketIOEvent e)
    {
        
    }
}
