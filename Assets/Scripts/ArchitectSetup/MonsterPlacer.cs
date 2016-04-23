using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class MonsterPlacer : MonoBehaviour {

    public MonsterSelector selector;
    public HeroManager manager;
    public MonsterEditor editor;
    public MonsterPlacementManager placementManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null) {
            var tile = Map.Current.GetTileAtMouse();
            if (placementManager.IsValidTileForPlacement(tile)) {
                if (tile.IsEmpty) {
                    var card = selector.GetCurrentlySelectedMonster();
                    if (card != null) {
                        var monsterInfo = card.cardInfo;
                        if (monsterInfo != null) {
                            var monster = manager.InstantiateMonster(monsterInfo.Name, GameManager.instance.Username, GameManager.instance.Username, System.Guid.NewGuid().ToString(), tile.XPos, tile.YPos, monsterInfo.health, monsterInfo.attack, monsterInfo.defense, monsterInfo.vision, monsterInfo.movement, 0, monsterInfo.weapon, monsterInfo.id);
                            selector.MonsterPlaced(card);
                            placementManager.AddMonster(monster);
                        }
                    }
                } else {
                    var obj = tile.MapObjects.First();
                    if (obj is Monster) {
                        Monster m = (Monster)obj;
                        editor.EditMonster(m);
                    }
                }
            } else {
                editor.EditMonster(null);
            }
        }
	}
}
