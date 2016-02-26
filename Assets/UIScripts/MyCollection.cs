using UnityEngine;
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
//		RectTransform ct = (RectTransform)contentParent.transform;
//		ct.rect.height = 
		if (!on) return;
		Clear();
		for (int i = 0; i < buttons.Length; i++) {
			displayed.Add(buttons[i]);
		}
		Sort();
	}

	public void OnMonsters(bool on) {
		Clear();
		if (!on) return;
		for (int i = 0; i < buttons.Length; i++) {
			if (collection[i].IsMonster) {
				displayed.Add(buttons[i]);
			}
		}
		Sort();
	}

	public void OnTraps(bool on) {
		Clear();
		if (!on) return;
		for (int i = 0; i < buttons.Length; i++) {
			if (!collection[i].IsMonster) {
				displayed.Add(buttons[i]);
			}
		}
		Sort();
	}

	public void Search(string term) {
		GameObject[] toSearch = displayed.ToArray();
		Clear();
		term = term.ToLower();
		for (int i = 0; i < toSearch.Length; i++) {
			if (collection[i].Name.ToLower().Contains(term)) {
				displayed.Add(buttons[i]);
			}
		}
		Sort();
	}

	public void Dropdown(int value) {
		curSort = (SortingOptions)value;
		Sort();
	}

	private void Sort() {
		switch (curSort) {
		case SortingOptions.CostIncreasing:
			CostIncreasing();
			break;
		case SortingOptions.CostDecreasing:
			CostDecreasing();
			break;
		case SortingOptions.Alphabetical:
			Alphabetical();
			break;
		default:
			CostIncreasing();
			break;
		}
	}

	private void CostIncreasing() {
		List<GameObject> sorted = displayed.OrderBy(o=>o.GetComponent<CardButton>().cardInfo.Cost).ToList();
		Clear();
		addList(sorted);
	}

	public void CostDecreasing() {
		List<GameObject> sorted = displayed.OrderBy(o=>o.GetComponent<CardButton>().cardInfo.Cost).ToList();
		sorted.Reverse();
		Clear();
		addList(sorted);
	}

	private void Alphabetical() {
		List<GameObject> sorted = displayed.OrderBy(o=>o.GetComponent<CardButton>().cardInfo.Name).ToList();
		Clear();
		addList(sorted);
	}

	public void Clear() {
		foreach (GameObject b in displayed) {
			removeContent(b);
		}
		displayed.Clear();
	}

	private void addContent(GameObject b, int index) {
		b.transform.SetParent(contentParent.transform, false);
		RectTransform rt = (RectTransform)(b.transform);
		calcRect(ref rt, index);
		b.SetActive(true);
		displayed.Add(b);
	}

	private void addList(List<GameObject> list) {
		int count = 0;
		foreach (GameObject b in list) {
			addContent(b, count);
			count++;
		}
	}

	private void removeContent(GameObject b) {
		b.SetActive(false);
		b.transform.parent = null;
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
