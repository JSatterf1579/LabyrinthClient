using UnityEngine;
using System.Collections.Generic;
using System;
using SocketIO;
using UnityEngine.SceneManagement;

public class MonsterPlacementManager : MonoBehaviour {

    public HeroManager Manager;
    public Color ValidTileHightlightColor = Color.blue;
    public Dialog DialogBox;
    
    public static JSONObject InitialMap;
    public static JSONObject JSONRoot;

    [System.NonSerialized]
    public List<Tile> HeroSpawns, MonsterSpawns, ObjectiveSpawns;

    [System.NonSerialized]
    public SpawnType[,] Spawns;

    private Dictionary<string, Monster> ChosenMonsters = new Dictionary<string, Monster>();

    private bool Queueing = false;

	// Use this for initialization
	void Start () {
        Debug.Log(InitialMap);
        JSONDecoder.DecodeMap(InitialMap, Map.Current, out HeroSpawns, out MonsterSpawns, out ObjectiveSpawns);
        SetupSpawnArray();
        SpawnObjectiveObjects();
        Map.Current.LockAllTiles();
        foreach(Tile t in MonsterSpawns) {
            t.Locked = false;
            t.HighlightColor = ValidTileHightlightColor;
            t.Highlighted = true;
        }

        foreach(Tile t in ObjectiveSpawns) {
            t.HighlightOverrideLock = true;
            t.Highlighted = true;
            t.HighlightColor = Color.yellow;
        }

        foreach(Tile t in HeroSpawns) {
            t.HighlightOverrideLock = true;
            t.Highlighted = true;
            t.HighlightColor = Color.red;
        }
	}

    private void SpawnObjectiveObjects() {
        foreach(Tile t in ObjectiveSpawns) {
            Manager.InstantiateObjective("Artifact", t.XPos, t.YPos, true);
        }
    }

    public void RemoveMonster(Monster cur) {
        ChosenMonsters.Remove(cur.UUID);
    }

    public void AddMonster(Monster m) {
        ChosenMonsters.Add(m.UUID, m);
    }

    private void SetupSpawnArray() {
        Spawns = new SpawnType[Map.Current.Width, Map.Current.Height];
        foreach(Tile t in HeroSpawns) {
            Spawns[t.XPos, t.YPos] |= SpawnType.HERO;
        }
        foreach(Tile t in MonsterSpawns) {
            Spawns[t.XPos, t.YPos] |= SpawnType.MONSTER;
        }
        foreach(Tile t in ObjectiveSpawns) {
            Spawns[t.XPos, t.YPos] |= SpawnType.OBJECTIVE;
        }
    }

    public bool IsValidTileForPlacement(Tile tile) {
        if (tile == null) return false;
        if (tile.IsObstacle) return false;
        return (Spawns[tile.XPos, tile.YPos] & SpawnType.MONSTER) == SpawnType.MONSTER;
    }

    public void ConfirmAndQueue() {
        
        if (Queueing) return;
        Queueing = true;
        var socket = GameManager.instance.getSocket();
        socket.On("queue_error", FailedQueue);
        socket.On("match_found", FindGame.OnMatch);
        AddMonstersToJSON();
        Debug.Log(JSONRoot);
        socket.Emit("queue_up_architect", JSONRoot, ArchQueue);
    }

    public void ArchQueue(JSONObject response)
    {
        Debug.Log(response);
        if (response.list[0].GetField("status").n == 200)
        {
            DialogBox.Show("Queued up. Please wait for match", "Cancel", QueueCancel);
        }
        else if (response.list[0].GetField("status").n == 422)
        {
            Queueing = false;
            var socket = GameManager.instance.getSocket();
            socket.Emit("dequeue", new JSONObject());
            socket.Emit("leave_match", new JSONObject());
        }
    }

    public void QueueCancel()
    {
        var socket = GameManager.instance.getSocket();
        socket.Emit("dequeue", new JSONObject(), DequeueResponse);
    }

    public void DequeueResponse(JSONObject response)
    {
        Debug.Log(response);
        if (response.list[0].GetField("status").n == 200)
        {
            DialogBox.Hide();
            Queueing = false;
        }
    }

    public void FailedQueue(SocketIOEvent response)
    {
        Debug.Log(response.data);
        Queueing = false;
    }

    public void CancelButtonPressed() {
        SceneManager.LoadScene("FindGame");
    }

    private void AddMonstersToJSON() {
        if (JSONRoot == null) JSONRoot = new JSONObject();
        JSONObject monsterList = new JSONObject(JSONObject.Type.ARRAY);
        foreach(var monster in ChosenMonsters.Values) {
            JSONObject inner = new JSONObject();
            inner.AddField("x", monster.posX);
            inner.AddField("y", monster.posY);
            inner.AddField("id", monster.DatabaseID);
            monsterList.Add(inner);
        }
        JSONRoot.AddField("monsters", monsterList);
    }

    [Flags]
    public enum SpawnType : byte {
        NONE = 0,
        HERO = 1,
        MONSTER = 2,
        OBJECTIVE = 4
    }
}
