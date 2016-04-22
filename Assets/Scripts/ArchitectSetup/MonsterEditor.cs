using UnityEngine;
using UnityEngine.UI;

public class MonsterEditor : MonoBehaviour {

    private Monster currentMonster;
    public InfoPanelSystem infoPanelSystem;
    public MonsterSelector selector;
    public MonsterPlacementManager manager;

    public Button DeleteButton;

    public Color EditHighlightColor = Color.green;

    private Color PreviousHightlightColor;

    public void EditMonster(Monster monster) {
        if (currentMonster) {
            StopEditingMonster();
        }
        if (monster) {
            currentMonster = monster;
            infoPanelSystem.updateSelectedInfo(monster);
            DeleteButton.gameObject.SetActive(true);
            var tile = monster.Tile;
            PreviousHightlightColor = tile.HighlightColor;
            tile.HighlightColor = EditHighlightColor;
            tile.Highlighted = true;
        }
    }

    public void StopEditingMonster() {
        if (currentMonster != null) {
            //currentMonster.Tile.Highlighted = false;
            currentMonster.Tile.HighlightColor = PreviousHightlightColor;
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
            manager.RemoveMonster(cur);
            StopEditingMonster();//so that the tile will stop being hightlighted
            selector.MonsterRemoved(selector.GetMonsterCardFromName(cur.MOName));
            Map.Current.RemoveMapObject(cur);
            Destroy(cur.gameObject);
        }
    }
}
