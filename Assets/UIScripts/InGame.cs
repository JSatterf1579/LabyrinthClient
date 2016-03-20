using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGame : MonoBehaviour {

	public Text GameInfo;

	public Button ConfirmAction;

	public Unit selectedUnit;

	private MatchManager match;

	// Use this for initialization
	void Start () {
		match = MatchManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (match) {
			updateGameInfo();
		} else {
			match = MatchManager.instance;
		}
	}

	private void updateGameInfo() {
		string opponent = match.OpponentName;
		switch (match.OpponentType) {
		case PlayerType.Architect:
			opponent = "Architect " + opponent;
			break;
		case PlayerType.Heroes:
			opponent = opponent + "'s heroes";
			break;
		default:
			break;
		}
		int turn = match.TurnNumber;
		bool myTurn = match.MyTurn;

		GameInfo.text = "vs " + opponent + " // turn #" + turn + " // " + (myTurn ? "your turn" : "their turn");
	}
}
