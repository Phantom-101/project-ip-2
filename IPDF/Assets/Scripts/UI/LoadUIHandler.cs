using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadUIHandler : MonoBehaviour {
    [Header ("Settings")]
    public string needToLoad = "";
    public SettingsHandler settingsHandler;
    [Header ("Prefabs")]
    public GameObject saveItem;
    [Header ("UI Elements")]
    public Canvas canvas;
    public GameObject savesPanel;

    void Awake () {
        DontDestroyOnLoad (gameObject);
        settingsHandler = FindObjectOfType<SettingsHandler> ();
        if (!Directory.Exists (GetSavePath ())) Directory.CreateDirectory (GetSavePath ());
        canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
        savesPanel = canvas.transform.Find ("Saves Selection/Outline/Panel/Viewport/Content").gameObject;
        FileInfo[] saves = new DirectoryInfo (Application.persistentDataPath + "/saves/").GetFiles ("*.txt").OrderBy (f => f.LastWriteTime).Reverse ().ToArray ();
        for (int i = 0; i < saves.Length; i++) {
            FileInfo save = saves[i];
            GameObject instantiated = Instantiate (saveItem, savesPanel.transform) as GameObject;
            RectTransform rectTransform = instantiated.GetComponent<RectTransform> ();
            rectTransform.anchoredPosition = new Vector2 (0, -i * 100);
            UniverseSaveData universe = JsonUtility.FromJson<UniverseSaveData> (File.ReadAllText (GetSavePath () + save.Name));
            instantiated.transform.GetChild (0).GetComponent<Text> ().text = universe.saveName;
            instantiated.transform.GetChild (1).GetComponent<Text> ().text = save.LastWriteTime.ToString ();
            int playerFactionID = 0;
            foreach (StructureSaveData structure in universe.structures)
                if (structure.isPlayer)
                    playerFactionID = structure.factionID;
            foreach (Faction faction in universe.factions)
                if (faction.id == playerFactionID) {
                    instantiated.transform.GetChild (2).GetComponent<Text> ().text = faction.name;
                    instantiated.transform.GetChild (3).GetComponent<Text> ().text = faction.wealth.ToString () + " Credits";
                }
            ButtonFunction (() => SaveSelected (save.Name), instantiated.GetComponent<Button> ());
        }
        savesPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, saves.Length * 100);
    }

    void Update () {
        if (canvas != null) canvas.GetComponent<CanvasScaler> ().scaleFactor = settingsHandler.settings.UIScale;
        if (needToLoad != "") {
            SavesHandler savesHandler = FindObjectOfType<SavesHandler> ();
            if (savesHandler != null) {
                savesHandler.Load (GetSavePath () + needToLoad);
                Destroy (gameObject);
            }
        }
    }

    void ButtonFunction (UnityEngine.Events.UnityAction action, Button button) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void SaveSelected (string saveName) {
        FindObjectOfType<ScenesManager> ().SetLoadedScene ("Game");
        needToLoad = saveName;
    }

    public string GetSavePath () {
        return Application.persistentDataPath + "/saves/";
    }

    public string GetSavePath (string saveName) {
        return Application.persistentDataPath + "/saves/" + saveName + ".txt";
    }
}
