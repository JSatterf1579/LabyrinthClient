using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSONDecoder{

    public static void DecodeMap(JSONObject serializedMap, Map mapRenderer)
    {
        if (mapRenderer == null)
        {
            Debug.LogError("No MapRenderer was given, cannot decode map!");
            return;
        }

        int x = (int)serializedMap.GetField("size").GetField("x").n;
        int y = (int)serializedMap.GetField("size").GetField("y").n;

        mapRenderer.DestroyMap();
        mapRenderer.InitalizeMap(x, y);

        JSONObject serializedTiles = serializedMap.GetField("tiles");
        for (int i = 0; i < serializedTiles.list.Count; i++)
        {
            int tileX = (int)serializedTiles.list[i].GetField("x").n;
            int tileY = (int)serializedTiles.list[i].GetField("y").n;
            int rotation = (int)serializedTiles.list[i].GetField("rotation").n;
            string type = serializedTiles.list[i].GetField("terrain").str;
            bool isObstacle = serializedTiles.list[i].GetField("is_obstacle").b;
            mapRenderer.InstantiateTile(tileX, tileY, type, rotation, isObstacle);
        }
    }

    public static void DecodeHeroes(JSONObject serializedHeroes, HeroManager Manager)
    {
        List<JSONObject> heroList = serializedHeroes.list;
        GameObject[] heroes = new GameObject[heroList.Count];
        foreach (JSONObject hero in heroList)
        {
            if (hero.GetField("type").str == "hero")
            {


                string UUID = hero.GetField("id").str;
                string owner = hero.GetField("owner_id").str;
                string controller = hero.GetField("controller_id").str;
                int xPos = (int) hero.GetField("x").n;
                int yPos = (int) hero.GetField("y").n;


                int level = (int) hero.GetField("level").n;
                int health = (int) hero.GetField("health").n;
                int attack = (int) hero.GetField("attack").n;
                int defense = (int) hero.GetField("defense").n;
                int vision = (int) hero.GetField("vision").n;
                int movement = (int) hero.GetField("movement").n;
                bool blocksMovement = hero["blocks_movement"].b;

                string heroType = hero.GetField("hero_type").str;

                Weapon weapon = null;

                if (hero.GetField("weapon") != null)
                {
                    weapon = DecodeWeapon(hero.GetField("weapon"));
                }

                Manager.InstantiateHero(heroType, owner, controller, UUID, xPos, yPos, level, health, attack, defense,
                    vision, movement, weapon, blocksMovement);
            }

        }
        
    }

    public static Weapon DecodeWeapon(JSONObject serializedWeapon)
    {
        string name = serializedWeapon.GetField("name").str;
        string image = serializedWeapon.GetField("image").str;
        string description = serializedWeapon.GetField("description").str;
        int damageMod = (int) serializedWeapon.GetField("damage_mod").n;
        int range = (int) serializedWeapon.GetField("range").n;
        List<TargetGridTile> pattern = DecodePatternTiles(serializedWeapon.GetField("attack_pattern"));
        bool rotatable = serializedWeapon.GetField("attack_pattern").GetField("rotatable").b;
        int numTargets = (int) serializedWeapon.GetField("attack_pattern").GetField("count").n;
        return new Weapon(name, image, damageMod, range, description, pattern, rotatable, numTargets);
    }

    public static List<TargetGridTile> DecodePatternTiles(JSONObject serializedPatterns)
    {
        List<TargetGridTile> tiles = new List<TargetGridTile>();
        List<JSONObject> serializedTiles = serializedPatterns.GetField("effect_map").list;
        foreach (JSONObject tile in serializedTiles)
        {
            int damagePercent = (int) tile.GetField("effect_percent").n;
            int x = (int) tile.GetField("x").n;
            int y = (int) tile.GetField("y").n;
            tiles.Add(new TargetGridTile(x, y, damagePercent));
        }
        return tiles;
    }


}
