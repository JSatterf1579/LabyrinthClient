﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSONDecoder{

    public static void DecodeMap(JSONObject serializedMap, Map mapRenderer) {
        List<Tile> heroes, monsters, objectives;
        DecodeMap(serializedMap, mapRenderer, out heroes, out monsters, out objectives);
    }

    public static void DecodeMap(JSONObject serializedMap, Map mapRenderer, out List<Tile> heroSpawns, out List<Tile> monsterSpawns, out List<Tile> objectiveSpawns)
    {
        heroSpawns = new List<Tile>();
        monsterSpawns = new List<Tile>();
        objectiveSpawns = new List<Tile>();
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
            bool blocksVision = serializedTiles.list[i]["is_vision_obstacle"].b;
            Tile tile = mapRenderer.InstantiateTile(tileX, tileY, type, rotation, isObstacle, blocksVision);
            if(tile != null) {
                if (serializedTiles.list[i]["is_hero_spawn_tile"].b) {
                    heroSpawns.Add(tile);
                }
                if(serializedTiles.list[i]["is_architect_spawn_tile"].b) {
                    monsterSpawns.Add(tile);
                }
                if(serializedTiles.list[i]["is_objective_spawn_tile"].b) {
                    objectiveSpawns.Add(tile);
                }
            }
        }
    }

    public static void DecodeMapObjects(JSONObject serializedHeroes, HeroManager Manager)
    {
        Debug.Log(serializedHeroes);
        List<JSONObject> heroList = serializedHeroes.list;
        GameObject[] heroes = new GameObject[heroList.Count];
        foreach (JSONObject hero in heroList)
        {
            if (hero.GetField("type").str == "hero")
            {


                string UUID = hero.GetField("id").str;
                string owner = hero.GetField("owner_id").str;
                string controller = hero.GetField("controller_id").str;
                int xPos = (int)hero.GetField("x").n;
                int yPos = (int)hero.GetField("y").n;


                int level = (int)hero.GetField("level").n;
                int health = (int)hero.GetField("max_health").n;
                int attack = (int)hero.GetField("attack").n;
                int defense = (int)hero.GetField("defense").n;
                int vision = (int)hero.GetField("vision").n;
                int movement = (int)hero.GetField("movement").n;
                int ap = (int)hero.GetField("max_action_points").n;
                bool blocksMovement = hero["blocks_movement"].b;
                string heroType = hero.GetField("hero_type").str;

                Weapon weapon = null;

                if (hero.GetField("weapon") != null)
                {
                    weapon = DecodeWeapon(hero.GetField("weapon"));
                }

                Manager.InstantiateHero(heroType, owner, controller, UUID, xPos, yPos, level, health, attack, defense,
                    vision, movement, ap, weapon, blocksMovement);
            }
            else if (hero.GetField("type").str == "monster")
            {
                string UUID = hero.GetField("id").str;
                string owner = hero.GetField("owner_id").str;
                string controller = hero.GetField("controller_id").str;
                int xPos = (int)hero.GetField("x").n;
                int yPos = (int)hero.GetField("y").n;


                int dbid = (int)hero.GetField("database_id").n;
                int health = (int)hero.GetField("health").n;
                int attack = (int)hero.GetField("attack").n;
                int defense = (int)hero.GetField("defense").n;
                int vision = (int)hero.GetField("vision").n;
                int movement = (int)hero.GetField("movement").n;
                int ap = (int)hero.GetField("max_action_points").n;

                string monsterName = hero.GetField("name").str;

                Weapon weapon = null;

                if (hero.GetField("weapon") != null)
                {
                    weapon = DecodeWeapon(hero.GetField("weapon"));
                }

                Manager.InstantiateMonster(monsterName, owner, controller, UUID, xPos, yPos, health, attack, defense, vision, movement, ap, weapon, dbid);
            }
            else if (hero.GetField("type").str == "objective")
            {
                string UUID = hero.GetField("id").str;
                int xPos = (int) hero.GetField("x").n;
                int yPos = (int) hero.GetField("y").n;
                bool blocksMovement = hero.GetField("blocks_movement").b;
                Manager.InstantiateObjective(UUID, xPos, yPos, blocksMovement);
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

    public static List<HeroCardData> DecodeHeroCards(JSONObject serializedHeroes)
    {
        List<HeroCardData> heroes = new List<HeroCardData>();
        foreach (JSONObject hero in serializedHeroes.GetField("heroes").list)
        {
            string name = hero.GetField("hero_type").str;
            string ownerID = hero.GetField("owner_id").str;
            string UUID = hero.GetField("id").str;

            int health = (int)hero.GetField("max_health").n;
            int attack = (int)hero.GetField("attack").n;
            int defense = (int)hero.GetField("defense").n;
            int vision = (int)hero.GetField("vision").n;
            int movement = (int)hero.GetField("movement").n;
            int actionPoints = (int)hero.GetField("max_action_points").n;
            int level = (int)hero.GetField("level").n;
            Weapon wep = DecodeWeapon(hero.GetField("weapon"));

            HeroCardData hcard = new HeroCardData();
            hcard.init(name, ownerID, UUID, health, attack, defense, vision, movement, actionPoints, wep, level);
            heroes.Add(hcard);
        }
        return heroes;
    }

    public static List<MonsterCardData> DecodeMonsterCards(JSONObject serializedMonsters)
    {
        List<MonsterCardData> monsters = new List<MonsterCardData>();
        foreach (JSONObject monster in serializedMonsters.list)
        {
            string name = monster.GetField("name").str;
            int id = (int)monster.GetField("id").n;
            int health = (int) monster.GetField("health").n;
            int attack = (int) monster.GetField("attack").n;
            int defense = (int) monster.GetField("defense").n;
            int movement = (int)monster.GetField("movement").n;
            int vision = (int)monster.GetField("vision").n;

            Weapon wep = DecodeWeapon(monster.GetField("weapon"));
            int count = (int)monster.GetField("quantity").n;
            monsters.Add(new MonsterCardData(name, id, health, attack, defense, movement, vision, wep, count));
        }
        return monsters;
    }

    public static List<FindGame.MapMetadata> DecodeMapMetadata(JSONObject serializedMaps)
    {
        List<FindGame.MapMetadata> maps = new List<FindGame.MapMetadata>();
        foreach (var map in serializedMaps.list)
        {
            string name = map.GetField("name").str;
            string creator = map.GetField("creator_id").str;
            int id = (int)map.GetField("id").n;
            int cap = (int)map.GetField("hero_capacity").n;
            int x = (int) map.GetField("size").GetField("x").n;
            int y = (int)map.GetField("size").GetField("y").n;
            FindGame.MapMetadata m = new FindGame.MapMetadata(id, cap, name, creator, x, y);
            maps.Add(m);
        }
        return maps;
    }  


}
