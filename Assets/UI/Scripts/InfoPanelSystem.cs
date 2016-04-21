using UnityEngine;
using UnityEngine.UI;

public class InfoPanelSystem : MonoBehaviour {

    [UnityEngine.Serialization.FormerlySerializedAs("name")]
	public Text Name;

	public Text stats;

	public Text description;

	public Text equippedWeapon;

	public InfoDismissButton dismissButton;

	private MapObject currentUnit;

	public void updateSelectedInfo(MapObject unit) {

		if (!unit) {
			Name.text = "";
			this.stats.text = "";
			equippedWeapon.text = "";
			return;
		}

		currentUnit = unit;

		Name.text = unit.MOName;

	    if (unit is Unit)
	    {
	        Unit u = (Unit) unit;
	        string stats = "AP: " + u.CurrentActionPoints + "/" + u.MaxActionPoints + " | H: " + u.currentHealth +
	                       "/" + u.maxHealth + " | A: " + u.attack +
	                       " | D: " + u.defense + " | V: " + u.vision + " | M: " + u.movement;

	        this.stats.text = stats;

            
	        equippedWeapon.text = "Weapon: " + u.weapon.Name;
	    }
        else if (unit is Objective)
        {
            string stats = "This is the objecive. Capture it to win";
            this.stats.text = stats;
            equippedWeapon.text = "";
        }
	}
}
