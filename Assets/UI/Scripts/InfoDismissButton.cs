using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//[RequireComponent(typeof(Button))]
public class InfoDismissButton : MonoBehaviour {

	public Animator animator;

	public Text buttonText;

	private bool _dismissed;

	public bool Dismissed {
		get {
			return _dismissed;
		}
		set {
			_dismissed = value;
			if (_dismissed) {
				Dismiss();
			} else {
				Show();
			}
		}
	}

	public void isClicked(Toggle toggleDismissed) {
		Dismissed = toggleDismissed.isOn;
	}

	public void Dismiss() {
		buttonText.rectTransform.localScale = new Vector3(-1, -1, 1);
		animator.Play("DismissInfoPanel");
	}

	public void Show() {
		buttonText.rectTransform.localScale = new Vector3(1, -1, 1);
		animator.Play("Reversed");
	}
}
