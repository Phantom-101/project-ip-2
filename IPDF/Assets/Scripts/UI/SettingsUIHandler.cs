using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIHandler : MonoBehaviour {
    public Canvas canvas;
    [Header ("Graphics Settings")]
    public Dropdown graphicsLevelDropdown;
    [Header ("Components")]
    public SettingsHandler settingsHandler;

    void Awake () {
        settingsHandler = FindObjectOfType<SettingsHandler> ();
        canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        graphicsLevelDropdown = canvas.transform.Find ("Graphics Level Dropdown").GetComponent<Dropdown> ();
        graphicsLevelDropdown.value = settingsHandler.settings.qualityLevel;
    }

    void Update () {
        settingsHandler.settings.qualityLevel = graphicsLevelDropdown.value;
    }
}
