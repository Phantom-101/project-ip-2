using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIHandler : MonoBehaviour {
    public Canvas canvas;
    [Header ("Graphics Settings")]
    public Dropdown graphicsLevelDropdown;
    public Slider uiScaleSlider;
    public Text uiScale;
    [Header ("Components")]
    public SettingsHandler settingsHandler;
    public CanvasScaler canvasScaler;

    void Awake () {
        settingsHandler = FindObjectOfType<SettingsHandler> ();
        canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        canvasScaler = canvas.GetComponent<CanvasScaler> ();
        if (canvas != null) canvas.GetComponent<CanvasScaler> ().scaleFactor = settingsHandler.settings.UIScale;
        graphicsLevelDropdown = canvas.transform.Find ("Graphics Level Dropdown").GetComponent<Dropdown> ();
        uiScaleSlider = canvas.transform.Find ("UI Scaling Slider").GetComponent<Slider> ();
        uiScale = canvas.transform.Find ("Current UI Scale").GetComponent<Text> ();
        graphicsLevelDropdown.value = settingsHandler.settings.qualityLevel;
        uiScaleSlider.value = settingsHandler.settings.UIScale;
    }

    void Update () {
        canvasScaler.scaleFactor = settingsHandler.settings.UIScale;
        settingsHandler.settings.qualityLevel = graphicsLevelDropdown.value;
        settingsHandler.settings.UIScale = uiScaleSlider.value;
        uiScale.text = uiScaleSlider.value.ToString("0.00") + "x";
    }
}
