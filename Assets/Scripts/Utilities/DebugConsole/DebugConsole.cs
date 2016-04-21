using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour {
    public Text OutputText;
    private string outputString;
    public InputField InputText;
    public Canvas ConsoleCanvas;
    public Scrollbar scrollbar;

    private MapViewCameraController cameraController;

	// Use this for initialization
	void Start () {
        cameraController = FindObjectOfType<MapViewCameraController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (ConsoleCanvas.enabled && Input.GetKeyDown(KeyCode.Return)) {
            ParseCommand(InputText.text);
            InputText.text = "";
            EventSystem.current.SetSelectedGameObject(InputText.gameObject);
        }

        if (ConsoleCanvas.enabled && Input.GetKey(KeyCode.Escape)) {
            ConsoleCanvas.enabled = false;
            if (cameraController) cameraController.enabled = true;
        }
        if (!ConsoleCanvas.enabled && Input.GetKey(KeyCode.BackQuote)) {
            ConsoleCanvas.enabled = true;
            InputText.text = "";
            if (cameraController) cameraController.enabled = false;
        }
        if (ConsoleCanvas.enabled) {
            if (EventSystem.current.currentSelectedGameObject != InputText.gameObject) {
                EventSystem.current.SetSelectedGameObject(InputText.gameObject);
            }
        }
    }

    private void ParseCommand(string command) {
        int spaceIndex = command.Trim().IndexOf(" ");
        string eventName;
        if (spaceIndex == -1) {
            eventName = command.Trim();
        } else {
            eventName = command.Substring(0, spaceIndex).Trim();
        }
        if (string.IsNullOrEmpty(eventName)) {
            return;
        }
        JSONObject obj = null;
        if (spaceIndex > 0) {
            try {
                obj = new JSONObject(command.Substring(spaceIndex).Trim());
            } catch (Exception e) {
                PrintError("Could not parse JSON data: ");
                PrintException(e);
                return;
            }
        }

        if (obj) {
            PrintLine("Executing " + eventName + " with parameter " + obj.ToString(), "green");
        } else {
            PrintLine("Executing " + eventName + " without parameters", "green");
        }
        try {
            GameManager.instance.getSocket().Emit(eventName, obj, (response) => {
                PrintLine("Response: " + response.Print(true));
            });
        } catch (Exception e) {
            PrintError("An error occured in sending the event");
            PrintException(e);
        }

    }

    public void Print(string msg) {
        outputString += msg;
        if (scrollbar) scrollbar.value = 0f;
        if(outputString.Length > 16000) {
            outputString = outputString.Substring(outputString.Length - 16000);
        }
        OutputText.text = outputString;

    }

    public void Print(string msg, string color) {
        Print("<color=" + color + ">" + msg + "</color>");
    }

    public void PrintLine(string msg) {
        Print(msg + "\n");
    }

    public void PrintLine(string msg, string color) {
        Print(msg + "\n", color);
    }

    public void PrintError(string msg) {
        PrintLine(msg, "red");
    }

    public void PrintWarning(string msg) {
        PrintLine(msg, "yellow");
    }
    public void PrintException(Exception e) {
        PrintError(e.GetType().Name + ": " + e.Message);
        PrintError("in " + e.Source);
        PrintError(e.StackTrace);
    }

    
}
