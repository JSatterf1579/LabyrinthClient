using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MyDeck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}
}
