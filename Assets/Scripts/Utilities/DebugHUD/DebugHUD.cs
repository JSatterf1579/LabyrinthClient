using UnityEngine;
using System.Collections.Generic;

//this version of DebugHUD was last updated 3/22/2016
[RequireComponent(typeof(GUIText))]
public class DebugHUD : MonoBehaviour {
    private static Dictionary<string, object> list;

    private GUIText text;

    public KeyCode ToggleKey = KeyCode.BackQuote;

    static DebugHUD() {
        list = new Dictionary<string, object>();
    }

    public static void setValue(string key, object value) {
        if (value is bool) {
            setValue(key, (bool)(value) ? "<color=green>True</color>" : "<color=red>False</color>");
        } else if (value == null) {
            setValue(key, "<color=red>null</color>");
        } else {
            list[key] = value;
        }
    }

    public static void RemoveKey(string key) {
        list.Remove(key);
    }

    void Start() {
        transform.position = Vector3.up;
        text = GetComponent<GUIText>();
        text.alignment = TextAlignment.Left;
        text.anchor = TextAnchor.UpperLeft;
        text.richText = true;
        text.enabled = Application.isEditor || Debug.isDebugBuild;
        DontDestroyOnLoad(this);
    }

    void LateUpdate() {
        if (Input.GetKeyDown(ToggleKey)) {
            text.enabled = !text.enabled;
        }
        string s = "";
        foreach (string key in list.Keys) {
            s += "<color=blue>" + key + "</color>: ";
            s += list[key] + "\n";
        }
        text.text = s;
    }
}
