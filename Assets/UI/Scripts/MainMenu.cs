using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {

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
#if UNITY_EDITOR
        if (Application.isEditor) {
			EditorApplication.isPlaying = false;
		}
        else {
#endif
            Application.Quit();
#if UNITY_EDITOR
        }
#endif
    }
}
