using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class InGame : MonoBehaviour {

	public Text GameInfo;

	public Button ConfirmAction;

	public GameObject jumpButtonPrefab;

	[System.NonSerialized]
	public Unit selectedUnit;

	public Selector selector;

	public InfoPanelSystem infoPanel;

	private MatchManager match;

	private bool alreadyDismissed;

	public Unit tempSelectedUnit;

	private Canvas canvas;

	// Use this for initialization
	void Start () {
		match = MatchManager.instance;
		infoPanel.gameObject.SetActive(false);
		alreadyDismissed = false;
		tempSelectedUnit = selector.SelectedUnit;
		canvas = gameObject.GetComponent<Canvas>();
		if (match) displayHeroes();
	}
	
	// Update is called once per frame
	void Update () {
		if (!tempSelectedUnit) tempSelectedUnit = selector.SelectedUnit;
		DebugHUD.setValue("Is dismissed", infoPanel.dismissButton.Dismissed);
		if (match) {
			updateGameInfo();
			if (selector.SelectedUnit) {
				if (!selector.SelectedUnit.Equals(selectedUnit) || infoPanel.dismissButton.Dismissed) {
					selectedUnit = selector.SelectedUnit;
					infoPanel.updateSelectedInfo(selectedUnit);
					infoPanel.gameObject.SetActive(true);
					infoPanel.dismissButton.Dismissed = false;
					alreadyDismissed = false;
				}
			} else {
//				infoPanel.gameObject.SetActive(false);
				if (!alreadyDismissed) {
					infoPanel.dismissButton.Dismissed = true;
					alreadyDismissed = true;
				}
			}
		} else {
			match = MatchManager.instance;
			displayHeroes();
		}
	}

	public void displayHeroes() {
		int count = 0;
		foreach (Hero hero in match.MapObjects.Values.Where(x => x is Hero && x.ownerID == GameManager.instance.Username)) {
			GameObject jumpButton = Instantiate(jumpButtonPrefab);
			jumpButton.transform.SetParent(canvas.transform);
			JumpButton jb = jumpButton.GetComponent<JumpButton>();
			RectTransform rt = (RectTransform)jumpButton.transform;
			jb.selector = selector;
			jb.unit = hero;
			float height = jb.anchorMax.y - jb.anchorMin.y;
			rt.anchorMin = new Vector2(jb.anchorMin.x, jb.anchorMin.y - (height + jb.offset) * count);
			rt.anchorMax = new Vector2(jb.anchorMax.x, jb.anchorMax.y - (height + jb.offset) * count);
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
			count++;
			jb.title.text = "Hero " + count;
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
