using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour {

	public Text title;

	public Vector2 anchorMin = new Vector2(0.01f, 0.65f);
	public Vector2 anchorMax = new Vector2(0.0525f, 0.719f);
	public float offset = .02f;

	public Unit unit;

	public Selector selector;

	private MapViewCameraController cameraController;

	// Use this for initialization
	void Start () {
		cameraController = FindObjectOfType<MapViewCameraController>();
	}

	public void JumpToUnit() {
		cameraController.AnimateMoveToTile(unit.Tile);
		selector.SelectUnit(unit);
	}
}
