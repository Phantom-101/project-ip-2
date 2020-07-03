using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class SettingsSaveData {
    public int qualityLevel;
    public float UIScale;
}

public class SettingsHandler : MonoBehaviour {
    [Header ("Settings")]
    public SettingsSaveData settings;
    public SettingsSaveData previousSettings;
    public RenderPipelineAsset[] graphicsLevels;

    void Awake () {
        DontDestroyOnLoad (gameObject);
        if (!File.Exists (Application.persistentDataPath + "/settings.txt")) {
            FileStream fileStream = File.Create (Application.persistentDataPath + "/settings.txt");
            fileStream.Close ();
        }
        settings = JsonUtility.FromJson<SettingsSaveData> (File.ReadAllText (Application.persistentDataPath + "/settings.txt"));
        if (settings == null) {
            settings = new SettingsSaveData ();
            settings.qualityLevel = 1;
            settings.UIScale = 1;
        }
    }

    void Update () {
        if (settings.qualityLevel < 0 || settings.qualityLevel > 2) settings.qualityLevel = 1;
        if (settings.UIScale < 0.5f || settings.UIScale > 2) settings.UIScale = 1;
        if (settings != previousSettings) {
            GraphicsSettings.renderPipelineAsset = graphicsLevels[settings.qualityLevel];
            File.WriteAllText (Application.persistentDataPath + "/settings.txt", JsonUtility.ToJson (settings, true));
        }
        previousSettings = settings;
    }
}