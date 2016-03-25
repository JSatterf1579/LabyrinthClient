using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Dialog : MonoBehaviour {

	private bool hidden;

	public Text message;

	public Button right;
	public Text rightTextBox;
	private UnityAction rightCallback;

	public Button left;
	public Text leftTextBox;
	private UnityAction leftCallback;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		hidden = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!hidden) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				rightCallback.Invoke();
				Hide();
			}
			if (Input.GetKeyDown(KeyCode.Escape)) {
				leftCallback.Invoke();
				Hide();
			}
		}
	}

	public void Hide() {
		hidden = true;
		gameObject.SetActive(false);
	}

	public void Show(string message, string rightOption, UnityAction rightCallback, string leftOption, UnityAction leftCallback) {
		hidden = false;
		gameObject.SetActive(true);
		this.message.text = message;
		rightTextBox.text = rightOption;
		this.rightCallback = rightCallback;
		this.leftCallback = leftCallback;
		right.onClick.AddListener(rightCallback);
		left.onClick.AddListener(leftCallback);
		leftTextBox.text = leftOption;
	}


}
