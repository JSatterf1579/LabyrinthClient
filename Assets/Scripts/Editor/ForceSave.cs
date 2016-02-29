using UnityEngine;
using UnityEditor;

public class ForceSave : MonoBehaviour {
    [MenuItem("File/Save All %&s")]
    public static void DoForceSave() {
        EditorApplication.SaveAssets();
    }
}
