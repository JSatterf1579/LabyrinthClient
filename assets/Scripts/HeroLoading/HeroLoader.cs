﻿using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System.Collections;
using System.Collections.Generic;

public class HeroLoader : MonoBehaviour {

    public Button MapButton;
    public Button AuthButton;
    public Button AcctButton;
    public string testUser;
    public string testPass;
    private SocketIOComponent _socket;

    public HeroManager Manager;

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
        if (response.list[0].GetField("status").n == 200)
        {
            DecodeHeroes(response.list[0].GetField("heroes"));
        }
        else
        {
            Debug.Log("Error Reciving Hero Data");
        }
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

    private GameObject[] DecodeHeroes(JSONObject serializedHeroes)
    {
        List<JSONObject> heroList = serializedHeroes.list;
        GameObject[] heroes = new GameObject[heroList.Count];
        foreach (JSONObject hero in heroList)
        {
            string UUID = hero.GetField("id").str;
            string owner = hero.GetField("owner_id").str;
            string controller = hero.GetField("controller_id").str;
            int xPos = (int)hero.GetField("x").n;
            int yPos = (int)hero.GetField("y").n;


            int level = (int)hero.GetField("level").n;
            int health = (int) hero.GetField("health").n;
            int attack = (int)hero.GetField("attack").n;
            int defense = (int)hero.GetField("defense").n;
            int vision = (int)hero.GetField("vision").n;
            int movement = (int)hero.GetField("movement").n;

            string heroType = hero.GetField("hero_type").str;

            //Manager.InstantiateHero(heroType, owner, controller, UUID, xPos, yPos, health, level, attack, defense, vision, movement);

        }
        return heroes;
    }
}
