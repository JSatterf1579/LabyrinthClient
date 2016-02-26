using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardButton : MonoBehaviour {

	public TempCard cardInfo;
	public Text name;
	public Text costQty;

	// Use this for initialization
	void Start () {
	
	}

	public void UpdateView() {
		name.text = cardInfo.Name;
		costQty.text = cardInfo.Cost + " | " + cardInfo.Quantity;
	}
}
