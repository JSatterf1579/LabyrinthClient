using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Dialog : MonoBehaviour {

	public Text message;

	private bool hidden;

	public Button right;
	public Text rightTextBox;
	public KeyCode rightKey = KeyCode.Return;
	private UnityAction rightCallback;
	private bool rightHidden;

	public Button left;
	public Text leftTextBox;
	public KeyCode leftKey = KeyCode.Escape;
	private UnityAction leftCallback;
	private bool leftHidden;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		leftHidden = true;
		rightHidden = true;
        right.onClick.AddListener(this.ExecuteRight);
        left.onClick.AddListener(this.ExecuteLeft);
	}
	
	// Update is called once per frame
	void Update () {
		if (!rightHidden) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				rightCallback.Invoke();
				Hide();
			}
		}
		if (!leftHidden) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				leftCallback.Invoke();
				Hide();
			}
		}
	}

    void OnDestroy()
    {
        right.onClick.RemoveAllListeners();
        left.onClick.RemoveAllListeners();
    }

	public void Hide() {
		leftHidden = true;
		rightHidden = true;
		gameObject.SetActive(false);
	}

	public void HideRight() {
		rightHidden = true;
		right.gameObject.SetActive(!rightHidden);
	}

	public void HideLeft() {
		leftHidden = true;
		left.gameObject.SetActive(!leftHidden);
	}

	public void Show(string message = null, string rightOption = null, UnityAction rightCallback = null, string leftOption = null, UnityAction leftCallback = null) {
		ShowRight(message, rightOption, rightCallback);
		ShowLeft(message, leftOption, leftCallback);
	}

	public void ShowRight(string message = null, string rightOption = null, UnityAction rightCallback = null) {
		rightHidden = false;
		gameObject.SetActive(true);
		left.gameObject.SetActive(!leftHidden);
		right.gameObject.SetActive(!rightHidden);

		if (message != null && this.message != null) this.message.text = message;
        
		this.rightCallback = rightCallback;

		if (rightOption != null) rightTextBox.text = rightOption;
	}

	public void ShowLeft(string message = null, string leftOption = null, UnityAction leftCallback = null) {
		leftHidden = false;
		gameObject.SetActive(true);
		left.gameObject.SetActive(!leftHidden);
		right.gameObject.SetActive(!rightHidden);

		if (message != null && this.message != null) this.message.text = message;
        
		this.leftCallback = leftCallback;

		if (leftOption != null) leftTextBox.text = leftOption;
	}

    public void ExecuteRight()
    {
        if (rightCallback != null)
        {
            rightCallback.Invoke();
        }
    }

    public void ExecuteLeft()
    {
        if (leftCallback != null)
        {
            leftCallback.Invoke();
        }
    }
}
