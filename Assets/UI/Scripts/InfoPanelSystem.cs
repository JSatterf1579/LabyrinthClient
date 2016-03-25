using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanelSystem : MonoBehaviour {

	public Text name;

	public Text stats;

	public Text description;

	public Text equippedWeapon;

	public InfoDismissButton dismissButton;



	public void updateSelectedInfo(Unit unit) {
		name.text = "names aren't important";

		// TODO: when we get health info use that instead of maxHealth
		string stats = "H: " + unit.maxHealth + "/" + unit.maxHealth + " | A: " + unit.attack + 
			" | D: " + unit.defense + " | V: " + unit.vision + " | M: " + unit.movement;

		stats.text = stats;

		equippedWeapon.text = "it might have a weapon. ask us tomorrow.";
		//infoPanel.equippedWeapon.text = "Weapon: " + unit.weapon.name;

		description.text = "we have no idea what this is.";
	}
}
