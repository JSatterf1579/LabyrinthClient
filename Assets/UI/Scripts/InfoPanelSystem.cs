using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanelSystem : MonoBehaviour {

	public Text name;

	public Text stats;

	public Text description;

	public Text equippedWeapon;

	public InfoDismissButton dismissButton;

	private Unit currentUnit;

	void Update() {
		updateSelectedInfo(currentUnit);
	}

	public void updateSelectedInfo(Unit unit) {

		currentUnit = unit;

		name.text = "names aren't important";

		
		string stats = "AP: " + unit.CurrentActionPoints + "/" + unit.MaxActionPoints + " | H: " + unit.currentHealth + "/" + unit.maxHealth + " | A: " + unit.attack + 
			" | D: " + unit.defense + " | V: " + unit.vision + " | M: " + unit.movement;

		this.stats.text = stats;

//		equippedWeapon.text = "it might have a weapon. ask us tomorrow."
		equippedWeapon.text = "Weapon: " + unit.weapon.Name;

		description.text = "we have no idea what this is.";
	}
}
