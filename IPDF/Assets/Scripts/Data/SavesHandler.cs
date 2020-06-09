using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class StructureSaveData {
    public string name;
    public int sectorID;
    public float[] position;
    public float[] rotation;
    public StructureProfile profile;
    public float hull;
    public int factionID;
    public InventoryHandler inventory;
    public List<TurretHandler> turrets;
    public ShieldHandler shield;
    public CapacitorHandler capacitor;
    public GeneratorHandler generator;
    public EngineHandler engine;
    public ElectronicsHandler electronics;
    public TractorBeamHandler tractorBeam;
    public List<FactoryHandler> factories;
    public StructureAI AI;
    public bool isPlayer;
}

public class SectorSaveData {
    public string name;
    public float[] position;
    public float[] rotation;
    public SectorData sectorData;
}

public class SavesHandler : MonoBehaviour {
    public string saveName = "default";
    public GameObject basicSector;
    public GameObject basicStructure;

    public void Save () {
        if (!Directory.Exists (GetSavePath (saveName))) Directory.CreateDirectory (GetSavePath (saveName));
        string saveString = "";
        // Setup needed components
        ItemsHandler itemsHandler = FindObjectOfType<ItemsHandler> ();
        PlayerController playerController = FindObjectOfType<PlayerController> ();
        // Save sectors
        Sector[] sectors = FindObjectsOfType<Sector> ();
        foreach (Sector sector in sectors) {
            SectorSaveData sectorData = new SectorSaveData ();
            sectorData.name = sector.name;
            sectorData.position = new float[] {sector.transform.position.x, sector.transform.position.y, sector.transform.position.z};
            sectorData.rotation = new float[] {sector.transform.eulerAngles.x, sector.transform.eulerAngles.y, sector.transform.eulerAngles.z};
            sectorData.sectorData = sector.sectorData;
            saveString += JsonUtility.ToJson (sectorData, true) + "\nNext Sector\n";
        }
        File.WriteAllText (GetSectorSavePath (saveName), saveString);
        saveString = "";
        // Save data of each structure
        StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
        foreach (StructureBehaviours structure in structures) {
            StructureSaveData data = new StructureSaveData();
            data.name = structure.gameObject.name;
            Transform parent = structure.transform.parent;
            while (!parent.GetComponent<Sector> ())
                parent = parent.parent;
            data.sectorID = parent.GetComponent<Sector> ().sectorData.id;
            data.position = new float[] {structure.transform.localPosition.x, structure.transform.localPosition.y, structure.transform.localPosition.z};
            data.rotation = new float[] {structure.transform.localEulerAngles.x, structure.transform.localEulerAngles.y, structure.transform.localEulerAngles.z};
            data.profile = structure.profile;
            data.hull = structure.hull;
            data.factionID = structure.factionID;
            data.inventory = structure.inventory;
            data.turrets = new List<TurretHandler> ();
            for (int i = 0; i < structure.profile.turretSlots; i++) data.turrets.Add (structure.turrets[i]);
            data.shield = structure.shield;
            data.capacitor = structure.capacitor;
            data.generator = structure.generator;
            data.engine = structure.engine;
            data.electronics = structure.electronics;
            data.tractorBeam = structure.tractorBeam;
            data.factories = structure.factories;
            data.AI = structure.AI;
            data.isPlayer = (playerController.structureBehaviours == structure);
            saveString += JsonUtility.ToJson (data, true) + "\nNext Structure\n";
        }
        File.WriteAllText (GetStructureSavePath (saveName), saveString);
    }

    public void Load () {
        if (!Directory.Exists (GetSavePath (saveName))) return;
        WaveSpawner[] waveSpawners = FindObjectsOfType<WaveSpawner> ();
        foreach (WaveSpawner waveSpawner in waveSpawners) Destroy (waveSpawner.gameObject);
        List<Sector> sectors = new List<Sector> ();
        if (File.Exists (GetSectorSavePath (saveName))) {
            Sector[] oldSectors = FindObjectsOfType<Sector> ();
            foreach (Sector sector in oldSectors) Destroy (sector.gameObject);
            string saveString = File.ReadAllText (GetSectorSavePath (saveName));
            string[] sectorsData = saveString.Split (new string[] {"\nNext Sector\n"}, System.StringSplitOptions.None);
            foreach (string sectorData in sectorsData) {
                SectorSaveData data = JsonUtility.FromJson<SectorSaveData> (sectorData);
                if (data != null) {
                    GameObject instantiated = Instantiate (basicSector, Vector3.zero, Quaternion.identity) as GameObject;
                    instantiated.name = data.name;
                    instantiated.transform.position = new Vector3 (data.position[0], data.position[1], data.position[2]);
                    instantiated.transform.eulerAngles = new Vector3 (data.rotation[0], data.rotation[1], data.rotation[2]);
                    instantiated.GetComponent<Sector> ().sectorData = data.sectorData;
                    sectors.Add (instantiated.GetComponent<Sector> ());
                }
            }
        }
        if (File.Exists (GetStructureSavePath (saveName))) {
            // Setup needed components
            ItemsHandler itemsHandler = FindObjectOfType<ItemsHandler> ();
            PlayerController playerController = FindObjectOfType<PlayerController> ();
            StructuresManager structuresManager = FindObjectOfType<StructuresManager> ();
            // Get all structures and destroy them
            StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
            foreach (StructureBehaviours structure in structures) structuresManager.RemoveStructure (structure);
            // Load structures
            List<StructureBehaviours> loaded = new List<StructureBehaviours> ();
            string saveString = File.ReadAllText (GetStructureSavePath (saveName));
            string[] structuresData = saveString.Split (new string[] {"\nNext Structure\n"}, System.StringSplitOptions.None);
            foreach (string structureData in structuresData) {
                StructureSaveData data = JsonUtility.FromJson<StructureSaveData> (structureData);
                if (data != null) {
                    GameObject instantiated = Instantiate (basicStructure, Vector3.zero, Quaternion.identity) as GameObject;
                    instantiated.name = data.name;
                    foreach (Sector sector in sectors)
                        if (sector.sectorData.id == data.sectorID)
                            instantiated.transform.parent = sector.transform;
                    instantiated.transform.localPosition = new Vector3 (data.position[0], data.position[1], data.position[2]);
                    instantiated.transform.localEulerAngles = new Vector3 (data.rotation[0], data.rotation[1], data.rotation[2]);
                    StructureBehaviours structureBehaviours = instantiated.GetComponent<StructureBehaviours> ();
                    structureBehaviours.initializeAccordingToSaveData = true;
                    structureBehaviours.profile = data.profile;
                    structureBehaviours.hull = data.hull;
                    structureBehaviours.factionID = data.factionID;
                    structureBehaviours.savedInventory = data.inventory;
                    structureBehaviours.savedTurrets = data.turrets;
                    structureBehaviours.savedShield = data.shield;
                    structureBehaviours.savedCapacitor = data.capacitor;
                    structureBehaviours.savedGenerator = data.generator;
                    structureBehaviours.savedEngine = data.engine;
                    structureBehaviours.savedElectronics = data.electronics;
                    structureBehaviours.savedTractorBeam = data.tractorBeam;
                    structureBehaviours.savedFactories = data.factories;
                    structureBehaviours.AI = data.AI;
                    loaded.Add (structureBehaviours);
                    if (data.isPlayer) playerController.structureBehaviours = structureBehaviours;
                }
            }
            foreach (StructureBehaviours structureBehaviours in loaded)
                structureBehaviours.Initialize ();
            // Reset camera position
            CameraFollowPlayer cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
            cameraFollowPlayer.ResetPosition ();
        }
    }

    public string GetSavePath (string saveName) {
        return Application.persistentDataPath + "/" + saveName;
    }

    public string GetSectorSavePath (string saveName) {
        return GetSavePath (saveName) + "/sectors.txt";
    }

    public string GetStructureSavePath (string saveName) {
        return GetSavePath (saveName) + "/structures.txt";
    }

    public void LogStructureSavePath (string saveName) {
        Debug.Log (GetStructureSavePath (saveName));
    }
}

[CustomEditor (typeof (SavesHandler))]
class SavesHandlerEditor : Editor {
    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        SavesHandler savesHandler = (SavesHandler) target;
        if (GUILayout.Button ("Get Saves Path")) {
            savesHandler.LogStructureSavePath (savesHandler.saveName);
        }
        if (GUILayout.Button ("Load")) {
            savesHandler.Load ();
        }
    }
}