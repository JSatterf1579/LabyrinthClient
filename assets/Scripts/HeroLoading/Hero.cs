using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class Hero : Unit
{

    //Really placeholder stuff until we finish this up.
    public Object[] equipment;

    public Object passive;

    public new void Init(string ownerID, string contrllerID, string UUID, int x, int y, int level, int health, int attack,
        int defense, int vision, int movement, Weapon weapon)
    {
        base.Init(ownerID, contrllerID, UUID, x, y, level, health, attack, defense, vision, movement, weapon);
        MatchManager.instance.RegisterJSONChangeAction("/board_objects/" + UUID + "/action_points", ActionPointsChanged);
    }

    private void ActionPointsChanged(JSONChangeInfo info) {
        if(info.Type != JSONChangeInfo.ChangeType.CHANGED) {
            Debug.LogWarning("Non change event recieved?");
            return;
        }
        Debug.Log(UUID + "'s Action Points were changed from " + info.OldValue.n + " to " + info.NewValue.n);
    }
}
