using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIO;
using UnityEngine.SceneManagement;

public class MyCollection : MonoBehaviour {

	
    private List<MonsterCardData> collection;
	private CardButton[] buttons;
	private List<CardButton> displayed = new List<CardButton>();
	public GameObject buttonPrefab;
	public GameObject contentParent;
	public InputField searchBox;
	public enum SortingOptions {CostIncreasing = 0, CostDecreasing, Alphabetical};
	private SortingOptions curSort = SortingOptions.CostIncreasing;
    private SocketIOComponent socket;

	// Use this for initialization
	void Start () {
		makeCollection();
	}

    void Awake()
    {
        socket = GameManager.instance.getSocket();
    }
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}

	public void OnAll(bool on) {
		if (!on) return;
		searchBox.text = "";
		displayed.Clear();
		displayed = buttons.ToList();
		RefreshDisplay(displayed);
	}

	public void OnMonsters(bool on) {
		if (!on) return;
		searchBox.text = "";
		displayed.Clear();
		displayed = buttons.Where(o => o.IsMonster()).ToList();
		RefreshDisplay(displayed);
	}

	public void OnTraps(bool on) {
		if (!on) return;
		searchBox.text = "";
		displayed.Clear();
		displayed = buttons.Where(o => !o.IsMonster()).ToList();
		RefreshDisplay(displayed);
	}

	public void Search(string term) {
		List<CardButton> results = displayed.Where(o => o.cardInfo.Name.ToLower().Contains(term.ToLower())).ToList();
		RefreshDisplay(results);
	}

	public void Dropdown(int value) {
		curSort = (SortingOptions)value;
		RefreshDisplay(displayed);
	}

	private void RefreshDisplay(List<CardButton> toDisplay) {
		ClearDisplay();
		addListToDisplay(Sort(toDisplay));
	}

	private List<CardButton> Sort(List<CardButton> toSort) {
		List<CardButton> sorted;
		switch (curSort) {
		case SortingOptions.CostIncreasing:
			sorted = CostIncreasing(toSort);
			break;
		case SortingOptions.CostDecreasing:
			sorted = CostDecreasing(toSort);
			break;
		case SortingOptions.Alphabetical:
			sorted = Alphabetical(toSort);
			break;
		default:
			sorted = CostIncreasing(toSort);
			break;
		}
		return sorted;
	}

	private List<CardButton> CostIncreasing(List<CardButton> toSort) {
		List<CardButton> sorted = toSort.OrderBy(o=>o.cardInfo.cost).ToList();
		return sorted;
	}

	public List<CardButton> CostDecreasing(List<CardButton> toSort) {
		List<CardButton> sorted = CostIncreasing(toSort);
		sorted.Reverse();
		return sorted;
	}

	private List<CardButton> Alphabetical(List<CardButton> toSort) {
		List<CardButton> sorted = toSort.OrderBy(o=>o.cardInfo.Name).ToList();
		return sorted;
	}

	private void addContentToDisplay(GameObject b, int index) {
		b.transform.SetParent(contentParent.transform, false);
		RectTransform rt = (RectTransform)(b.transform);
		calcRect(ref rt, index);
		b.SetActive(true);
	}

	private void addListToDisplay(List<CardButton> list) {
		int count = 0;
		foreach (CardButton b in list) {
			addContentToDisplay(b.gameObject, count);
			count++;
		}
	}
		
	public void ClearDisplay() {
		foreach (Transform b in contentParent.transform) {
			removeContentFromDisplay(b.gameObject);
		}
	}

	private void removeContentFromDisplay(GameObject b) {
		b.SetActive(false);
	}

	private void calcRect(ref RectTransform rt, int i) {
		rt.anchorMin = new Vector2(0.04f + 0.24f * (i % 4), 0.56f - 0.44f * (int)(i / 4));
		rt.anchorMax = new Vector2(rt.anchorMin.x + 0.2f, rt.anchorMin.y + 0.4f);
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
		rt.localScale = Vector3.one;
	}

	private void makeButtons() {
		buttons = new CardButton[collection.Count];
		for (int i = 0; i < collection.Count; i++)
        {
			GameObject b = Instantiate(buttonPrefab);
			buttons[i] = b.GetComponent<CardButton>();
			buttons[i].cardInfo = collection[i];
			buttons[i].UpdateView();
			buttons[i].gameObject.SetActive(false);
		}
	}
		
	private void makeCollection() {
        socket.Emit("get_monsters", new JSONObject(), MonstersCallback);
		//collection = new TempCard[32];
		//for (int i = 0; i < collection.Length; i++) {
		//	bool ismonster = i % 3 != 0;
		//	string name = ismonster ? "Monster " + (i + 1) : "Trap " + (i + 1);
		//	collection[i] = new TempCard(name, (int)Random.Range(100, 500), ismonster, (int)Random.Range(1, 10));
		//}
	}

    private void MonstersCallback(JSONObject response)
    {
        Debug.Log(response);
        if (response.list[0].GetField("status").n == 200)
        {
            collection = JSONDecoder.DecodeMonsterCards(response.list[0].GetField("monsters"));
            makeButtons();
            OnAll(true);
        }
    }
}
