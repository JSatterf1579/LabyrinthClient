using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[RequireComponent(typeof(UnityEngine.UI.Button))]
public class CardButton : MonoBehaviour {

	public MonsterCardData cardInfo;

    [UnityEngine.Serialization.FormerlySerializedAs("name")]
	public Text Name;
	public Text costQty;

    public bool DisplayRemaining = false;
    public int Remaining;

    [System.NonSerialized]
    public Button button;

    void Awake() {
        this.button = GetComponent<Button>();
    }

	public void UpdateView() {
		Name.text = cardInfo.Name;
        if (DisplayRemaining) {
            costQty.text = cardInfo.cost + " | x " + Remaining + "/" + cardInfo.count;
        } else {
            costQty.text = cardInfo.cost + " | x" + cardInfo.count;
        }
	}

    public bool IsMonster()
    {
        return true;
    }
}
