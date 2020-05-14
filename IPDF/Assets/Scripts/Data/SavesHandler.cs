using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class StructureSaveData {
    public string name;
    public float[] position;
    public float[] rotation;
    public StructureProfile profile;
    public string faction;
    public InventoryHandler inventory;
    public List<TurretHandler> turrets;
    public ShieldHandler shield;
    public CapacitorHandler capacitor;
    public GeneratorHandler generator;
    public EngineHandler engine;
    public ElectronicsHandler electronics;
    public TractorBeamHandler tractorBeam;
    public bool AIActivated;
    public bool isPlayer;
}

public class SavesHandler : MonoBehaviour {
    public GameObject basicStructure;

    public void Save () {
        string saveString = "";
        // Setup needed components
        ItemsHandler itemsHandler = FindObjectOfType<ItemsHandler> ();
        PlayerController playerController = FindObjectOfType<PlayerController> ();
        // Save data of each structure
        StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
        foreach (StructureBehaviours structure in structures) {
            StructureSaveData data = new StructureSaveData();
            data.name = structure.gameObject.name;
            data.position = new float[] {structure.transform.position.x, structure.transform.position.y, structure.transform.position.z};
            data.rotation = new float[] {structure.transform.rotation.eulerAngles.x, structure.transform.rotation.eulerAngles.y, structure.transform.rotation.eulerAngles.z};
            data.profile = structure.profile;
            // TODO Add inventory saves later
            data.faction = structure.faction;
            data.inventory = structure.inventory;
            data.turrets = new List<TurretHandler> ();
            for (int i = 0; i < structure.profile.turretSlots; i++) data.turrets.Add (structure.turrets[i]);
            data.shield = structure.shield;
            data.capacitor = structure.capacitor;
            data.generator = structure.generator;
            data.engine = structure.engine;
            data.electronics = structure.electronics;
            data.tractorBeam = structure.tractorBeam;
            data.AIActivated = structure.AIActivated;
            data.isPlayer = (playerController.structureBehaviours == structure);
            saveString += JsonUtility.ToJson (data, true) + "\nNext Structure\n";
        }
        File.WriteAllText (GetSavePath (), saveString);
    }

    public void Load () {
        if (File.Exists (GetSavePath ())) {
            // Setup needed components
            ItemsHandler itemsHandler = FindObjectOfType<ItemsHandler> ();
            PlayerController playerController = FindObjectOfType<PlayerController> ();
            // Get all structures and destroy them
            StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
            foreach (StructureBehaviours structure in structures) Destroy (structure.gameObject);
            // Load structures
            string saveString = File.ReadAllText (GetSavePath ());
            string[] structuresData = saveString.Split (new string[] {"\nNext Structure\n"}, System.StringSplitOptions.None);
            foreach (string structureData in structuresData) {
                StructureSaveData data = JsonUtility.FromJson<StructureSaveData> (structureData);
                if (data != null) {
                    GameObject instantiated = Instantiate (
                        basicStructure,
                        new Vector3 (data.position[0], data.position[1], data.position[2]),
                        Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2])
                    ) as GameObject;
                    instantiated.name = data.name;
                    StructureBehaviours structureBehaviours = instantiated.GetComponent<StructureBehaviours> ();
                    if (structureBehaviours == null) structureBehaviours = instantiated.AddComponent<StructureBehaviours> ();
                    structureBehaviours.initializeAccordingToSaveData = true;
                    structureBehaviours.profile = data.profile;
                    structureBehaviours.faction = data.faction;
                    structureBehaviours.savedInventory = data.inventory;
                    structureBehaviours.savedTurrets = data.turrets;
                    structureBehaviours.savedShield = data.shield;
                    structureBehaviours.savedCapacitor = data.capacitor;
                    structureBehaviours.savedGenerator = data.generator;
                    structureBehaviours.savedEngine = data.engine;
                    structureBehaviours.savedElectronics = data.electronics;
                    structureBehaviours.savedTractorBeam = data.tractorBeam;
                    structureBehaviours.Initialize ();
                    structureBehaviours.AIActivated = data.AIActivated;
                    if (data.isPlayer) playerController.structureBehaviours = structureBehaviours;
                }
            }
            // Reset camera position
            CameraFollowPlayer cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
            cameraFollowPlayer.ResetPosition ();
        }
    }

    public string GetSavePath () {
        return Application.persistentDataPath + "/save.txt";
    }

    public void LogSavePath () {
        Debug.Log (GetSavePath ());
    }
}

[CustomEditor (typeof (SavesHandler))]
class SavesHandlerEditor : Editor {
    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        SavesHandler savesHandler = (SavesHandler) target;
        if (GUILayout.Button ("Get Saves Path")) {
            savesHandler.LogSavePath ();
        }
    }
}