using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class StructureSaveData {
    public string name;
    public float[] position;
    public float[] rotation;
    public string profileId;
    public Dictionary<Item, int> inventory;
    public List<Turret> turrets;
    public Shield shield;
    public Capacitor capacitor;
    public Generator generator;
    public Engine engine;
    public Electronics electronics;
    public TractorBeam tractorBeam;
    public string[] equipmentIds;
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
            data.profileId = structure.profile.name;
            // TODO Add inventory saves later
            data.inventory = new Dictionary<Item, int> ();
            data.turrets = new List<Turret> ();
            for (int i = 0; i < structure.profile.turretSlots; i++) data.turrets.Add (structure.turrets[i].turret);
            data.shield = structure.shield.shield;
            data.capacitor = structure.capacitor.capacitor;
            data.generator = structure.generator.generator;
            data.engine = structure.engine.engine;
            data.electronics = structure.electronics.electronics;
            data.tractorBeam = structure.tractorBeam.tractorBeam;
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
                    StructureProfile profile = itemsHandler.GetItemById (data.profileId) as StructureProfile;
                    StructureBehaviours structureBehaviours = instantiated.GetComponent<StructureBehaviours> ();
                    if (structureBehaviours == null) structureBehaviours = instantiated.AddComponent<StructureBehaviours> ();
                    structureBehaviours.initializeAccordingToSaveData = true;
                    structureBehaviours.profile = profile;
                    structureBehaviours.savedTurrets = data.turrets;
                    structureBehaviours.savedShield = data.shield;
                    structureBehaviours.savedCapacitor = data.capacitor;
                    structureBehaviours.savedGenerator = data.generator;
                    structureBehaviours.savedEngine = data.engine;
                    structureBehaviours.savedElectronics = data.electronics;
                    structureBehaviours.savedTractorBeam = data.tractorBeam;
                    structureBehaviours.Initialize ();
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