using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void OnFindGame() {
		SceneManager.LoadScene("FindGame");
	}

	public void OnMyHeroes() {
		SceneManager.LoadScene("MyHeroes");
	}

	public void OnMyCollection() {
		SceneManager.LoadScene("MyCollection");
	}

	public void OnStore() {
		SceneManager.LoadScene("Store");
	}

	public void OnQuit() {
		if (Application.isEditor) {
			EditorApplication.isPlaying = false;
		} else {
			Application.Quit();
		}
	}
}
