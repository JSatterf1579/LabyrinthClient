using UnityEngine;
using UnityEngine.UI;

public class InfoPanelSystem : MonoBehaviour {

    [UnityEngine.Serialization.FormerlySerializedAs("name")]
	public Text Name;

	public Text stats;

	public Text description;

	public Text equippedWeapon;

	public InfoDismissButton dismissButton;

	private Unit currentUnit;

	public void updateSelectedInfo(Unit unit) {

		if (!unit) {
			Name.text = "";
			this.stats.text = "";
			equippedWeapon.text = "";
			return;
		}

		currentUnit = unit;

		Name.text = "names aren't important";

		
		string stats = "AP: " + unit.CurrentActionPoints + "/" + unit.MaxActionPoints + " | H: " + unit.currentHealth + "/" + unit.maxHealth + " | A: " + unit.attack + 
			" | D: " + unit.defense + " | V: " + unit.vision + " | M: " + unit.movement;

		this.stats.text = stats;

//		equippedWeapon.text = "it might have a weapon. ask us tomorrow."
		equippedWeapon.text = "Weapon: " + unit.weapon.Name;
	}
}
