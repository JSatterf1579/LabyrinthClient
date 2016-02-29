using UnityEngine;
using System.Collections;

public class MatchManager : MonoBehaviour
{

    public Map map;

    public HeroManager manager;

    public string MatchIdentifier { get; private set; }


	// Use this for initialization
	void Start () {
	    if (GameManager.instance != null && GameManager.instance.MatchData != null)
	    {
	        MatchIdentifier = GameManager.instance.MatchData.GetField("match_identifier").str;
	        JSONDecoder.DecodeMap(GameManager.instance.MatchData.GetField("map"), map);
            JSONDecoder.DecodeHeroes(GameManager.instance.MatchData.GetField("board_objects"), manager);
	    }
	    else
	    {
            Debug.LogError("You got in here without a game manager or match start data! How did you do that!");
	    }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
