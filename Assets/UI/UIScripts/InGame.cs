﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGame : MonoBehaviour {

	public Text GameInfo;

	public Button ConfirmAction;

	[System.NonSerialized]
	public Unit selectedUnit;

	public Selector selector;

	public InfoPanelSystem infoPanel;

	private MatchManager match;

	// Use this for initialization
	void Start () {
		match = MatchManager.instance;
		infoPanel.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		DebugHUD.setValue("Is dismissed", infoPanel.dismissButton.Dismissed);
		if (match) {
			updateGameInfo();
			if (selector.SelectedUnit) {
				if (!selector.SelectedUnit.Equals(selectedUnit) || infoPanel.dismissButton.Dismissed) {
					selectedUnit = selector.SelectedUnit;
					updateSelectedInfo(selectedUnit);
					infoPanel.gameObject.SetActive(true);
					infoPanel.dismissButton.Dismissed = false;
				}
			} else {
//				infoPanel.gameObject.SetActive(false);
				infoPanel.dismissButton.Dismissed = true;
			}
		} else {
			match = MatchManager.instance;
		}
	}

	private void updateSelectedInfo(Unit unit) {
		infoPanel.name.text = "names aren't important";

		// TODO: when we get health info use that instead of maxHealth
		string stats = "H: " + unit.maxHealth + "/" + unit.maxHealth + " | A: " + unit.attack + 
			" | D: " + unit.defense + " | V: " + unit.vision + " | M: " + unit.movement;

		infoPanel.stats.text = stats;

		infoPanel.equippedWeapon.text = "it might have a weapon. ask us tomorrow.";
//		infoPanel.equippedWeapon.text = "Weapon: " + unit.weapon.name;

		infoPanel.description.text = "we have no idea what this is.";
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
