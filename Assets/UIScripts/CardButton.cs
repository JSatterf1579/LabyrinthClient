using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardButton : MonoBehaviour {

	public TempCard cardInfo;
	public Text name;
	public Text costQty;

	public void UpdateView() {
		name.text = cardInfo.Name;
		costQty.text = cardInfo.Cost + " | x" + cardInfo.Quantity;
	}
}
