using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardButton : MonoBehaviour {

	public MonsterCardData cardInfo;
	public Text name;
	public Text costQty;

	public void UpdateView() {
		name.text = cardInfo.Name;
		costQty.text = 1 + " | x" + cardInfo.count;
	}

    public bool IsMonster()
    {
        return true;
    }
}
