using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

public class MonsterSelector : MonoBehaviour {

    public MyCollection collection;

    private CardButton selectedMonster = null;

    public Color DefaultColor = Color.white;
    public Color SelectedColor = Color.green;

    public CardButton GetCurrentlySelectedMonster() {
        return selectedMonster;
    }

    public void MonsterPlaced(CardButton card) {
        card.Remaining--;
        if(card.Remaining <= 0) {
            card.Remaining = 0;//just in case
            //Debug.Log($"Fresh out of {type}!");//well that sucks... stupid unity using super outdated versions of C#
            Debug.Log("Fresh out of " + card.cardInfo.Name + "!");
            selectedMonster = null;
        }
        card.UpdateView();
        RefreshButtonColors();
    }

    public CardButton GetMonsterCardFromName(string name) {
        return collection.buttons.First(x => x.cardInfo.Name == name);
    }

    public void MonsterRemoved(CardButton card) {
        card.Remaining++;
        if(card.Remaining == 1) {
            Debug.Log(card.cardInfo.Name + " is back on the menu, boys!");
            //do a thing?
        }
        card.UpdateView();
        RefreshButtonColors();
    }

    public void SelectMonster(CardButton data) {
        if (selectedMonster != null) {
            var c = selectedMonster.button.colors;
            c.normalColor = DefaultColor;
            selectedMonster.button.colors = c;
        }

        selectedMonster = data;

        if (selectedMonster != null) {
            var c = selectedMonster.button.colors;
            c.normalColor = SelectedColor;
            selectedMonster.button.colors = c;
        }

        RefreshButtonColors();
    }

    public void RefreshButtonColors() {
        foreach(var b in collection.buttons) {
            if (b.Remaining == 0) {
                b.button.interactable = false;
            } else {
                b.button.interactable = true;
            }
        }
    }
}
