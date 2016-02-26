using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MyCollection : MonoBehaviour {

	private TempCard[] collection;
	private GameObject[] buttons;
	private List<GameObject> displayed = new List<GameObject>();
	public GameObject buttonPrefab;
	public GameObject contentParent;
	public InputField searchBox;
	public enum SortingOptions {CostIncreasing = 0, CostDecreasing, Alphabetical};
	private SortingOptions curSort = SortingOptions.CostIncreasing;

	// Use this for initialization
	void Start () {
		makeCollection();
		makeButtons();
		OnAll(true);
	}
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}

	private void makeCollection() {
		collection = new TempCard[16];
		for (int i = 0; i < collection.Length; i++) {
			bool ismonster = i % 3 != 0;
			string name = ismonster ? "Monster " + (i + 1) : "Trap " + (i + 1);
			collection[i] = new TempCard(name, (int)Random.Range(100, 500), ismonster, (int)Random.Range(1, 10));
		}
	}

	public void OnAll(bool on) {
		if (!on) return;
		searchBox.text = "";
		displayed.Clear();
		for (int i = 0; i < buttons.Length; i++) {
			displayed.Add(buttons[i]);
		}
		RefreshDisplay();
	}

	public void OnMonsters(bool on) {
		if (!on) return;
		searchBox.text = "";
		displayed.Clear();
		for (int i = 0; i < buttons.Length; i++) {
			if (collection[i].IsMonster) {
				displayed.Add(buttons[i]);
			}
		}
		RefreshDisplay();
	}

	public void OnTraps(bool on) {
		if (!on) return;
		searchBox.text = "";
		displayed.Clear();
		for (int i = 0; i < buttons.Length; i++) {
			if (!collection[i].IsMonster) {
				displayed.Add(buttons[i]);
			}
		}
		RefreshDisplay();
	}

	public void Search(string term) {
		List<GameObject> results = displayed.Where(o => o.GetComponent<CardButton>().cardInfo.Name.ToLower().Contains(term.ToLower())).ToList();
		ClearDisplay();
		addListToDisplay(Sort(results));
	}

	public void Dropdown(int value) {
		curSort = (SortingOptions)value;
		RefreshDisplay();
	}

	private void RefreshDisplay() {
		ClearDisplay();
		addListToDisplay(Sort(displayed));
	}

	private List<GameObject> Sort(List<GameObject> toSort) {
		List<GameObject> sorted;
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

	private List<GameObject> CostIncreasing(List<GameObject> toSort) {
		List<GameObject> sorted = toSort.OrderBy(o=>o.GetComponent<CardButton>().cardInfo.Cost).ToList();
		return sorted;
	}

	public List<GameObject> CostDecreasing(List<GameObject> toSort) {
		List<GameObject> sorted = CostIncreasing(toSort);
		sorted.Reverse();
		return sorted;
	}

	private List<GameObject> Alphabetical(List<GameObject> toSort) {
		List<GameObject> sorted = toSort.OrderBy(o=>o.GetComponent<CardButton>().cardInfo.Name).ToList();
		return sorted;
	}

	private void addContentToDisplay(GameObject b, int index) {
		b.transform.SetParent(contentParent.transform, false);
		RectTransform rt = (RectTransform)(b.transform);
		calcRect(ref rt, index);
		b.SetActive(true);
	}

	private void addListToDisplay(List<GameObject> list) {
		int count = 0;
		foreach (GameObject b in list) {
			addContentToDisplay(b, count);
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
		buttons = new GameObject[collection.Length];
		for (int i = 0; i < collection.Length; i++) {
			buttons[i] = Instantiate(buttonPrefab);
			CardButton cb = buttons[i].GetComponent<CardButton>();
			cb.cardInfo = collection[i];
			cb.UpdateView();
			buttons[i].SetActive(false);
		}
	}
}
