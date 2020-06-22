using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class UniverseSaveData {
    public string saveName;
    public List<StructureSaveData> structures = new List<StructureSaveData> ();
    public List<SectorSaveData> sectors = new List<SectorSaveData> ();
    public List<CelestialObjectSaveData> celestialObjects = new List<CelestialObjectSaveData> ();
    public List<Faction> factions = new List<Faction> ();
}

[System.Serializable]
public class BaseSaveData {
    public string name;
    public float[] position;
    public float[] rotation;
    public float[] scale;
}

[System.Serializable]
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

[System.Serializable]
public class SectorSaveData : BaseSaveData {
    public SectorData sectorData;
}

[System.Serializable]
public class CelestialObjectSaveData : BaseSaveData {
    public int celestialObjectID;
}

public class SavesHandler : MonoBehaviour {
    [Header ("Save Settings")]
    public string universeName = "default";
    [Header ("Components")]
    public FactionsManager factionsManager;
    public StructuresManager structuresManager;
    public PlayerController playerController;
    public ItemsHandler itemsHandler;
    public CameraFollowPlayer cameraFollowPlayer;

    void Awake () {
        DontDestroyOnLoad (gameObject);
    }

    public void Save () {
        factionsManager = FindObjectOfType<FactionsManager> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        playerController = FindObjectOfType<PlayerController> ();
        itemsHandler = FindObjectOfType<ItemsHandler> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();

        System.DateTime epochStart = new System.DateTime (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int curTime = (int) (System.DateTime.UtcNow - epochStart).TotalSeconds;
        string nowString = curTime.ToString ();
        UniverseSaveData universe = new UniverseSaveData ();

        universe.saveName = universeName;

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
            universe.sectors.Add (sectorData);
        }

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
            universe.structures.Add (data);
        }

        universe.factions = factionsManager.factions;

        CelestialObject[] celestialObjects = FindObjectsOfType<CelestialObject> ();
        foreach (CelestialObject celestialObject in celestialObjects) {
            CelestialObjectSaveData data = new CelestialObjectSaveData ();
            data.name = celestialObject.gameObject.name;
            Transform transform = celestialObject.gameObject.transform;
            data.position = new float[] { transform.localPosition.x, transform.localPosition.y, transform.localPosition.z };
            data.rotation = new float[] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
            data.scale = new float[] { transform.localScale.x, transform.localScale.y, transform.localScale.z };
            data.celestialObjectID = celestialObject.celestialObjectID;
            universe.celestialObjects.Add (data);
        }
        File.WriteAllText (GetSavePath (universeName + "-" + nowString), JsonUtility.ToJson (universe, true));
    }

    public void Load (string saveName) {
        factionsManager = FindObjectOfType<FactionsManager> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        playerController = FindObjectOfType<PlayerController> ();
        itemsHandler = FindObjectOfType<ItemsHandler> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();

        if (!File.Exists (GetSavePath (saveName))) return;

        UniverseSaveData universe = JsonUtility.FromJson<UniverseSaveData> (File.ReadAllText (GetSavePath (saveName)));

        universeName = universe.saveName;

        WaveSpawner[] waveSpawners = FindObjectsOfType<WaveSpawner> ();
        foreach (WaveSpawner waveSpawner in waveSpawners) Destroy (waveSpawner.gameObject);

        List<Sector> sectors = new List<Sector> ();
        foreach (SectorSaveData data in universe.sectors) {
            GameObject instantiated = new GameObject ();
            instantiated.name = data.name;
            instantiated.transform.localPosition = new Vector3 (data.position[0], data.position[1], data.position[2]);
            instantiated.transform.localEulerAngles = new Vector3 (data.rotation[0], data.rotation[1], data.rotation[2]);
            instantiated.transform.localScale = new Vector3 (data.scale[0], data.scale[1], data.scale[2]);
            Sector s = instantiated.AddComponent<Sector> ();
            s.sectorData = data.sectorData;
            sectors.Add (s);
        }

        // Get all structures and destroy them
        StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
        structuresManager.structures = new List<StructureBehaviours> ();
        foreach (StructureBehaviours structure in structures) Destroy (structure.gameObject);
        // Load structures
        List<StructureBehaviours> loaded = new List<StructureBehaviours> ();
        foreach (StructureSaveData data in universe.structures) {
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

        factionsManager.factions = universe.factions;

        foreach (CelestialObjectSaveData data in universe.celestialObjects) {
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

    public string GetSavePath (string saveName) {
        return Application.persistentDataPath + "/" + saveName + ".txt";
    }

    public void LogSavePath (string saveName) {
        Debug.Log (GetSavePath (saveName));
    }
}