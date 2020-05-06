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
    public string[] inventoryIds;
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
            data.inventoryIds = new string[] {""};
            data.equipmentIds = new string[structure.profile.turretSlots + 6];
            for (int i = 0; i < structure.profile.turretSlots; i++) data.equipmentIds[i] = (structure.turrets[i].turret == null ? "Invalid" : structure.turrets[i].turret.name);
            data.equipmentIds[structure.profile.turretSlots] = (structure.shield.shield == null ? "Invalid" : structure.shield.shield.name);
            data.equipmentIds[structure.profile.turretSlots + 1] = (structure.capacitor.capacitor == null ? "Invalid" : structure.capacitor.capacitor.name);
            data.equipmentIds[structure.profile.turretSlots + 2] = (structure.generator.generator == null ? "Invalid" : structure.generator.generator.name);
            data.equipmentIds[structure.profile.turretSlots + 3] = (structure.engine.engine == null ? "Invalid" : structure.engine.engine.name);
            data.equipmentIds[structure.profile.turretSlots + 4] = (structure.electronics.electronics == null ? "Invalid" : structure.electronics.electronics.name);
            data.equipmentIds[structure.profile.turretSlots + 5] = (structure.tractorBeam.tractorBeam == null ? "Invalid" : structure.tractorBeam.tractorBeam.name);
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
                    int totalEquipment = profile.turretSlots + 6;
                    Item[] equipment = new Item[totalEquipment];
                    for (int i = 0; i < totalEquipment; i++) equipment[i] = itemsHandler.GetItemById (data.equipmentIds[i]);
                    StructureBehaviours structureBehaviours = instantiated.GetComponent<StructureBehaviours> ();
                    if (structureBehaviours == null) structureBehaviours = instantiated.AddComponent<StructureBehaviours> ();
                    structureBehaviours.profile = profile;
                    structureBehaviours.savedEquipment = equipment;
                    structureBehaviours.Initialize ();
                    if (data.isPlayer) playerController.structureBehaviours = structureBehaviours;
                }
            }
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