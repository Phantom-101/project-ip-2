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
    public static SettingsHandler current;

    [Header ("Settings")]
    public SettingsSaveData settings;
    public RenderPipelineAsset[] graphicsLevels;

    void Awake () {
        if (SettingsHandler.GetInstance () != null) Destroy (gameObject);
        current = this;
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

    public static SettingsHandler GetInstance () {
        return current;
    }

    void Update () {
        if (settings.qualityLevel < 0 || settings.qualityLevel > 2) settings.qualityLevel = 1;
        if (settings.UIScale < 0.5f || settings.UIScale > 2) settings.UIScale = 1;
        GraphicsSettings.renderPipelineAsset = graphicsLevels[settings.qualityLevel];
        File.WriteAllText (Application.persistentDataPath + "/settings.txt", JsonUtility.ToJson (settings, true));
    }
}