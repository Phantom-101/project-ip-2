using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class BaseSaveData {
    public string name;
    public float[] position;
    public float[] rotation;
    public float[] scale;
}

public class StructureSaveData : BaseSaveData {
    public int id;
    public int sectorID;
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
    public int[] docked;
    public bool isPlayer;
}

public class SectorSaveData : BaseSaveData {
    public SectorData sectorData;
}

public class CelestialObjectSaveData : BaseSaveData {
    public int celestialObjectID;
}

public class SavesHandler : MonoBehaviour {
    [Header ("Save Settings")]
    public string saveName = "default";
    public string sectorsSeparator = "\nNext Sector\n";
    public string structuresSeparator = "\nNext Structure\n";
    public string factionsSeparator = "\nNext Faction\n";
    public string celestialObjectsSeparator = "\nNext Celestial Object\n";
    [Header ("Prefabs")]
    public GameObject basicSector;
    public GameObject basicStructure;
    [Header ("Components")]
    public FactionsManager factionsManager;
    public StructuresManager structuresManager;
    public PlayerController playerController;
    public ItemsHandler itemsHandler;
    public CameraFollowPlayer cameraFollowPlayer;

    public void Awake () {
        factionsManager = FindObjectOfType<FactionsManager> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        playerController = FindObjectOfType<PlayerController> ();
        itemsHandler = FindObjectOfType<ItemsHandler> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
    }

    public void Save () {
        if (!Directory.Exists (GetSavePath (saveName))) Directory.CreateDirectory (GetSavePath (saveName));
        string saveString = "";
        // Save sectors
        Sector[] sectors = FindObjectsOfType<Sector> ();
        foreach (Sector sector in sectors) {
            SectorSaveData sectorData = new SectorSaveData ();
            sectorData.name = sector.name;
            Transform transform = sector.gameObject.transform;
            sectorData.position = new float[] { transform.localPosition.x, transform.localPosition.y, transform.localPosition.z };
            sectorData.rotation = new float[] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
            sectorData.scale = new float[] { transform.localScale.x, transform.localScale.y, transform.localScale.z };
            sectorData.sectorData = sector.sectorData;
            saveString += JsonUtility.ToJson (sectorData, true) + sectorsSeparator;
        }
        File.WriteAllText (GetSectorsSavePath (saveName), saveString);

        saveString = "";
        // Save data of each structure
        StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
        foreach (StructureBehaviours structure in structures) {
            StructureSaveData data = new StructureSaveData ();
            data.name = structure.gameObject.name;
            Transform parent = structure.transform.parent;
            while (!parent.GetComponent<Sector> ())
                parent = parent.parent;
            data.id = structure.id;
            data.sectorID = parent.GetComponent<Sector> ().sectorData.id;
            data.position = new float[] { structure.transform.localPosition.x, structure.transform.localPosition.y, structure.transform.localPosition.z };
            data.rotation = new float[] { structure.transform.localEulerAngles.x, structure.transform.localEulerAngles.y, structure.transform.localEulerAngles.z };
            data.scale = new float[] { structure.transform.localScale.x, structure.transform.localScale.y, structure.transform.localScale.z };
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
            data.docked = structure.docked;
            data.isPlayer = (playerController.structureBehaviours == structure);
            saveString += JsonUtility.ToJson (data, true) + structuresSeparator;
        }
        File.WriteAllText (GetStructuresSavePath (saveName), saveString);

        saveString = "";
        foreach (Faction faction in factionsManager.factions)
            saveString += JsonUtility.ToJson (faction, true) + factionsSeparator;
        File.WriteAllText (GetFactionsSavePath (saveName), saveString);

        saveString = "";
        CelestialObject[] celestialObjects = FindObjectsOfType<CelestialObject> ();
        foreach (CelestialObject celestialObject in celestialObjects) {
            CelestialObjectSaveData data = new CelestialObjectSaveData ();
            data.name = celestialObject.gameObject.name;
            Transform transform = celestialObject.gameObject.transform;
            data.position = new float[] { transform.localPosition.x, transform.localPosition.y, transform.localPosition.z };
            data.rotation = new float[] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
            data.scale = new float[] { transform.localScale.x, transform.localScale.y, transform.localScale.z };
            data.celestialObjectID = celestialObject.celestialObjectID;
            saveString += JsonUtility.ToJson (data, true) + celestialObjectsSeparator;
        }
        File.WriteAllText (GetCelestialObjectsSavePath (saveName), saveString);
    }

    public void Load () {
        if (!Directory.Exists (GetSavePath (saveName))) return;
        WaveSpawner[] waveSpawners = FindObjectsOfType<WaveSpawner> ();
        foreach (WaveSpawner waveSpawner in waveSpawners) Destroy (waveSpawner.gameObject);

        List<Sector> sectors = new List<Sector> ();
        if (File.Exists (GetSectorsSavePath (saveName))) {
            Sector[] oldSectors = FindObjectsOfType<Sector> ();
            foreach (Sector sector in oldSectors) Destroy (sector.gameObject);
            string saveString = File.ReadAllText (GetSectorsSavePath (saveName));
            string[] sectorsData = saveString.Split (new string[] { sectorsSeparator }, System.StringSplitOptions.None);
            foreach (string sectorData in sectorsData) {
                SectorSaveData data = JsonUtility.FromJson<SectorSaveData> (sectorData);
                if (data != null) {
                    GameObject instantiated = new GameObject ();
                    instantiated.name = data.name;
                    instantiated.transform.localPosition = new Vector3 (data.position[0], data.position[1], data.position[2]);
                    instantiated.transform.localEulerAngles = new Vector3 (data.rotation[0], data.rotation[1], data.rotation[2]);
                    instantiated.transform.localScale = new Vector3 (data.scale[0], data.scale[1], data.scale[2]);
                    Sector s = instantiated.AddComponent<Sector> ();
                    s.sectorData = data.sectorData;
                    sectors.Add (s);
                }
            }
        }

        if (File.Exists (GetStructuresSavePath (saveName))) {
            // Get all structures and destroy them
            StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
            structuresManager.structures = new List<StructureBehaviours> ();
            foreach (StructureBehaviours structure in structures) Destroy (structure.gameObject);
            // Load structures
            List<StructureBehaviours> loaded = new List<StructureBehaviours> ();
            string saveString = File.ReadAllText (GetStructuresSavePath (saveName));
            string[] structuresData = saveString.Split (new string[] { structuresSeparator }, System.StringSplitOptions.None);
            foreach (string structureData in structuresData) {
                StructureSaveData data = JsonUtility.FromJson<StructureSaveData> (structureData);
                if (data != null) {
                    GameObject instantiated = new GameObject ();
                    StructureBehaviours structureBehaviours = instantiated.AddComponent<StructureBehaviours> ();
                    structureBehaviours.id = data.id;
                    foreach (Sector sector in sectors)
                        if (sector.sectorData.id == data.sectorID)
                            instantiated.transform.parent = sector.transform;
                    instantiated.name = data.name;
                    instantiated.transform.localPosition = new Vector3 (data.position[0], data.position[1], data.position[2]);
                    instantiated.transform.localEulerAngles = new Vector3 (data.rotation[0], data.rotation[1], data.rotation[2]);
                    instantiated.transform.localScale = new Vector3 (data.scale[0], data.scale[1], data.scale[2]);
                    structureBehaviours.profile = data.profile;
                    structureBehaviours.hull = data.hull;
                    structureBehaviours.factionID = data.factionID;
                    structureBehaviours.inventory = data.inventory;
                    structureBehaviours.turrets = data.turrets;
                    structureBehaviours.shield = data.shield;
                    structureBehaviours.capacitor = data.capacitor;
                    structureBehaviours.generator = data.generator;
                    structureBehaviours.engine = data.engine;
                    structureBehaviours.electronics = data.electronics;
                    structureBehaviours.tractorBeam = data.tractorBeam;
                    structureBehaviours.factories = data.factories;
                    structureBehaviours.AI = data.AI;
                    structureBehaviours.docked = data.docked;
                    loaded.Add (structureBehaviours);
                    if (data.isPlayer) playerController.structureBehaviours = structureBehaviours;
                }
            }
            foreach (StructureBehaviours structureBehaviours in loaded) {
                foreach (int dockedID in structureBehaviours.docked)
                    foreach (StructureBehaviours possibleDocker in loaded)
                        if (possibleDocker.id == dockedID) {
                            possibleDocker.transform.parent = structureBehaviours.transform;
                            possibleDocker.transform.localPosition = possibleDocker.transform.position;
                        }
                structureBehaviours.Initialize ();
            }
            // Reset camera position
            cameraFollowPlayer.ResetPosition ();
        }
        if (File.Exists (GetFactionsSavePath (saveName))) {
            FactionsManager factionsManager = FindObjectOfType<FactionsManager> ();
            List<Faction> factions = new List<Faction> ();
            string saveString = File.ReadAllText (GetFactionsSavePath (saveName));
            string[] factionsData = saveString.Split (new string[] { factionsSeparator }, System.StringSplitOptions.None);
            foreach (string factionData in factionsData) {
                Faction parsedFaction = JsonUtility.FromJson<Faction> (factionData);
                if (parsedFaction != null)
                    factions.Add (parsedFaction);
            }
            factionsManager.factions = factions;
        }
        if (File.Exists (GetCelestialObjectsSavePath (saveName))) {
            string saveString = File.ReadAllText (GetCelestialObjectsSavePath (saveName));
            string[] celestialObjectsData = saveString.Split (new string[] { celestialObjectsSeparator }, System.StringSplitOptions.None);
            foreach (string celestialObjectData in celestialObjectsData) {
                CelestialObjectSaveData data = JsonUtility.FromJson<CelestialObjectSaveData> (celestialObjectData);
                if (data != null) {
                    GameObject instantiated = new GameObject ("Celestial Object");
                    instantiated.name = data.name;
                    instantiated.transform.localPosition = new Vector3 (data.position[0], data.position[1], data.position[2]);
                    instantiated.transform.localEulerAngles = new Vector3 (data.rotation[0], data.rotation[1], data.rotation[2]);
                    instantiated.transform.localScale = new Vector3 (data.scale[0], data.scale[1], data.scale[2]);
                    CelestialObject co = instantiated.AddComponent<CelestialObject> ();
                    co.celestialObjectID = data.celestialObjectID;
                    co.Initialize ();
                }
            }
        }
    }

    public string GetSavePath (string saveName) {
        return Application.persistentDataPath + "/" + saveName;
    }

    public string GetSectorsSavePath (string saveName) {
        return GetSavePath (saveName) + "/sectors.txt";
    }

    public string GetStructuresSavePath (string saveName) {
        return GetSavePath (saveName) + "/structures.txt";
    }

    public string GetFactionsSavePath (string saveName) {
        return GetSavePath (saveName) + "/factions.txt";
    }

    public string GetCelestialObjectsSavePath (string saveName) {
        return GetSavePath (saveName) + "/celestialObjects.txt";
    }

    public void LogStructuresSavePath (string saveName) {
        Debug.Log (GetStructuresSavePath (saveName));
    }
}

[CustomEditor (typeof (SavesHandler))]
class SavesHandlerEditor : Editor {
    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
        SavesHandler savesHandler = (SavesHandler) target;
        if (GUILayout.Button ("Get Saves Path")) {
            savesHandler.LogStructuresSavePath (savesHandler.saveName);
        }
        if (GUILayout.Button ("Load")) {
            savesHandler.Load ();
        }
    }
}