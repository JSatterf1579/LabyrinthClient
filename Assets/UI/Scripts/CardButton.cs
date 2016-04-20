using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardButton : MonoBehaviour {

	public MonsterCardData cardInfo;

    [UnityEngine.Serialization.FormerlySerializedAs("name")]
	public Text Name;
	public Text costQty;

	public void UpdateView() {
		Name.text = cardInfo.Name;
		costQty.text = cardInfo.cost + " | x" + cardInfo.count;
	}

    public bool IsMonster()
    {
        return true;
    }
}
