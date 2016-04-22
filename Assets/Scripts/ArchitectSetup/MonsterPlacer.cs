using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class MonsterPlacer : MonoBehaviour {

    public MonsterSelector selector;
    public HeroManager manager;
    public MonsterEditor editor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null) {
            var tile = Map.Current.GetTileAtMouse();
            if (tile != null && !tile.IsObstacle) {
                if (tile.IsEmpty) {
                    var card = selector.GetCurrentlySelectedMonster();
                    if (card != null) {
                        var monsterInfo = card.cardInfo;
                        if (monsterInfo != null) {
                            manager.InstantiateMonster(monsterInfo.Name, GameManager.instance.Username, GameManager.instance.Username, System.Guid.NewGuid().ToString(), tile.XPos, tile.YPos, monsterInfo.health, monsterInfo.attack, monsterInfo.defense, monsterInfo.vision, monsterInfo.movement, 0, monsterInfo.weapon);
                            selector.MonsterPlaced(card);
                        }
                    }
                } else {
                    Monster m = (Monster)tile.MapObjects.First();
                    editor.EditMonster(m);
                }
            } else {
                editor.EditMonster(null);
            }
        }
	}
}
