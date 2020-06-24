using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class SettingsSaveData {
    public int qualityLevel;
}

public class SettingsHandler : MonoBehaviour {
    [Header ("Settings")]
    public SettingsSaveData settings;
    public RenderPipelineAsset[] graphicsLevels;

    void Awake () {
        DontDestroyOnLoad (gameObject);
        if (!File.Exists (Application.persistentDataPath + "/settings.txt")) File.Create (Application.persistentDataPath + "/settings.txt");
        settings = JsonUtility.FromJson<SettingsSaveData> (File.ReadAllText (Application.persistentDataPath + "/settings.txt"));
        if (settings == null) settings = new SettingsSaveData ();
    }

    void Update () {
        GraphicsSettings.renderPipelineAsset = graphicsLevels[settings.qualityLevel];
        File.WriteAllText (Application.persistentDataPath + "/settings.txt", JsonUtility.ToJson (settings, true));
    }
}