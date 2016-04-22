using UnityEngine;
using UnityEngine.UI;

public class MonsterEditor : MonoBehaviour {

    private Monster currentMonster;
    public InfoPanelSystem infoPanelSystem;
    public MonsterSelector selector;

    public Button DeleteButton;

    public void EditMonster(Monster monster) {
        if (currentMonster) {
            StopEditingMonster();
        }
        if (monster) {
            currentMonster = monster;
            infoPanelSystem.updateSelectedInfo(monster);
            DeleteButton.gameObject.SetActive(true);
            monster.Tile.HighlightColor = Color.green;
            monster.Tile.Highlighted = true;
        }
    }

    public void StopEditingMonster() {
        if (currentMonster != null) {
            currentMonster.Tile.Highlighted = false;
        }
        currentMonster = null;
        infoPanelSystem.updateSelectedInfo(null);
        DeleteButton.gameObject.SetActive(false);
    }

    public void DeleteButtonPressed() {
        Debug.Log("Delete button pressed");
        if(currentMonster != null) {
            Debug.Log("Deleting " + currentMonster.MOName);
            var cur = currentMonster;
            StopEditingMonster();//so that the tile will stop being hightlighted
            selector.MonsterRemoved(selector.GetMonsterCardFromName(cur.MOName));
            Map.Current.RemoveMapObject(cur);
            Destroy(cur.gameObject);
        }
    }
}
