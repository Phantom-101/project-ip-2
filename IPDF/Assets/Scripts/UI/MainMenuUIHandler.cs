using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIHandler : MonoBehaviour {
    public SettingsHandler settingsHandler;
    public Canvas canvas;

    void Awake () {
        settingsHandler = FindObjectOfType<SettingsHandler> ();
        canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        if (canvas != null) canvas.GetComponent<CanvasScaler> ().scaleFactor = settingsHandler.settings.UIScale;
    }

    void Update () {
        if (canvas != null) canvas.GetComponent<CanvasScaler> ().scaleFactor = settingsHandler.settings.UIScale;
    }
}
