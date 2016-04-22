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

		int currentHealth = unit.currentHealth;
		string stats = "";
		if (currentHealth == 0) {
			stats = "<color=red>Dead<color>";
		} else {
			int currentAP = unit.CurrentActionPoints;
			string apColor = "green";
			if (currentAP == 0) {
				apColor = "red";
			} else if (currentAP == 1) {
				apColor = "yellow";
			}
			stats = "AP: <color=" + apColor + ">" + unit.CurrentActionPoints + "</color>/" + unit.MaxActionPoints + " | H: " + unit.currentHealth + "/" + unit.maxHealth + " | A: " + unit.attack + 
				" | D: " + unit.defense + " | V: " + unit.vision + " | M: " + unit.movement;
		}



		this.stats.text = stats;

//		equippedWeapon.text = "it might have a weapon. ask us tomorrow."
		equippedWeapon.text = "Weapon: " + unit.weapon.Name;
	}
}
