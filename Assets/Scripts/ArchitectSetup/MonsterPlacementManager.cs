using UnityEngine;
using System.Collections;

public class MonsterPlacementManager : MonoBehaviour {

    public static JSONObject InitialMap;

	// Use this for initialization
	void Start () {
        Debug.Log(InitialMap);
        JSONDecoder.DecodeMap(InitialMap, Map.Current);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
