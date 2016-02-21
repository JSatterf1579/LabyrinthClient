using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}
}
