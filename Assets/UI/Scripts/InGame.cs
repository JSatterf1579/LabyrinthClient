﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class InGame : MonoBehaviour {

	public Text GameInfo;

	public Button ConfirmAction;
	public GameObject EndTurn;

	public GameObject jumpButtonYouPrefab;
	public GameObject jumpButtonThemPrefab;

	[System.NonSerialized]
	public MapObject selectedUnit;

	public Selector selector;

	public InfoPanelSystem infoPanel;
	public GameObject actionButtons;

	private MatchManager match;

	private bool alreadyDismissed;

	public Unit tempSelectedUnit;

	private Canvas canvas;

	// Use this for initialization
	void Start () {
		match = MatchManager.instance;
//		infoPanel.gameObject.SetActive(false);
		alreadyDismissed = false;
		tempSelectedUnit = selector.SelectedUnit;
		canvas = gameObject.GetComponent<Canvas>();
		if (match) displayJumpButtons();
	}
	
	// Update is called once per frame
	void Update () {
		if (!tempSelectedUnit) tempSelectedUnit = selector.SelectedUnit;
		DebugHUD.setValue("Is dismissed", infoPanel.dismissButton.Dismissed);
		if (match) {
			updateGameInfo();
//			if (selector.SelectedUnit) {
//				if (!selector.SelectedUnit.Equals(selectedUnit) || infoPanel.dismissButton.Dismissed) {
					//selectedUnit = selector.SelectedUnit;
					infoPanel.updateSelectedInfo(selectedUnit);
//					infoPanel.gameObject.SetActive(true);
//					infoPanel.dismissButton.Dismissed = false;
//					alreadyDismissed = false;
//				}
//			} else {
////				infoPanel.gameObject.SetActive(false);
//				if (!alreadyDismissed) {
//					infoPanel.dismissButton.Dismissed = true;
//					alreadyDismissed = true;
//				}
//			}
			EndTurn.SetActive(match.MyTurn);
		} else {
			match = MatchManager.instance;
			displayJumpButtons();
		}
	}

	public void displayJumpButtons() {
		int count = 0;
		int monsterCount = 0;
		foreach (Unit unit in match.MapObjects.Values.Where(x => x is Unit)) {
			GameObject jumpButton;
			if (unit.ownerID == GameManager.instance.Username) {
				jumpButton = Instantiate(jumpButtonYouPrefab);
			} else {
				bool isVisible = unit.Tile.VisionState.Equals(VisionState.VISIBLE);
				if (isVisible) {
					jumpButton = Instantiate(jumpButtonThemPrefab);
				} else {
					continue;
				}
			}
			jumpButton.transform.SetParent(canvas.transform);
			JumpButton jb = jumpButton.GetComponent<JumpButton>();
			RectTransform rt = (RectTransform)jumpButton.transform;
			jb.selector = selector;
			jb.unit = unit;
			float height = jb.anchorMax.y - jb.anchorMin.y;
			rt.anchorMin = new Vector2(jb.anchorMin.x, jb.anchorMin.y - (height + jb.offset) * count);
			rt.anchorMax = new Vector2(jb.anchorMax.x, jb.anchorMax.y - (height + jb.offset) * count);
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			count++;
			string title;
			switch(unit.MOName.ToLower()) {
			case "warrior":
				title = "W";
				break;
			case "mage":
				title = "M";
				break;
			case "rogue":
				title = "R";
				break;
			case "warriorrogue":
				title = "W/R";
				break;
			case "warriormage":
				title = "W/M";
				break;
			case "roguemage":
				title = "R/M";
				break;
			default:
				monsterCount++;
				title = "M" + monsterCount;
				break;
			}
			jb.title.text = title;
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
		// TODO: split bot's username on underscore
		GameInfo.text = "vs " + opponent + " // turn #" + turn + " // " + (myTurn ? "your turn" : "their turn");
	}
}
