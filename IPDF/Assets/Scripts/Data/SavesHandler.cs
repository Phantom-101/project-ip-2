using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

[Serializable]
public class UniverseSaveData {
    public string saveName;
    public List<StructureSaveData> structures = new List<StructureSaveData> ();
    public List<SectorSaveData> sectors = new List<SectorSaveData> ();
    public List<CelestialObjectSaveData> celestialObjects = new List<CelestialObjectSaveData> ();
    public List<Faction> factions = new List<Faction> ();
    public Dictionary<SerializableFactionPair, float> relations = new Dictionary<SerializableFactionPair, float> ();
}

[Serializable]
public class BaseSaveData {
    public string name;
    public float[] position;
    public float[] rotation;
    public float[] scale;
}

[Serializable]
public class StructureSaveData : BaseSaveData {
    public string id;
    public int sectorID;
    public string profile;
    public float hull;
    public int factionID;
    public List<StructureInventoryPair> inventory;
    public List<TurretHandler> turrets;
    public ShieldHandler shield;
    public CapacitorHandler capacitor;
    public GeneratorHandler generator;
    public EngineHandler engine;
    public ElectronicsHandler electronics;
    public TractorBeamHandler tractorBeam;
    public List<FactoryHandler> factories;
    public StructureAI AI;
    public string[] docked;
    public bool isPlayer;
    public JumpGateSaveData jumpGateSaveData;
}

[Serializable]
public class JumpGateSaveData {
    public float triggerRange;
    public float forwardDistance;
    public string otherId;
}

[Serializable]
public class StructureInventoryPair {
    public string item;
    public int amount;

    public StructureInventoryPair (string item, int amount) {
        this.item = item;
        this.amount = amount;
    }
}

[Serializable]
public class SectorSaveData : BaseSaveData {
    public SectorData sectorData;
}

[Serializable]
public class CelestialObjectSaveData : BaseSaveData {
    public int celestialObjectID;
}

[Serializable]
public struct SerializableFactionPair {
    public int a;
    public int b;

    public SerializableFactionPair (FactionPair data) {
        this.a = data.a.id;
        this.b = data.b.id;
    }
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
    public GameUIHandler gameUIHandler;

    void Awake () {
        factionsManager = FindObjectOfType<FactionsManager> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        playerController = FindObjectOfType<PlayerController> ();
        itemsHandler = FindObjectOfType<ItemsHandler> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
        gameUIHandler = FindObjectOfType<GameUIHandler> ();
        if (!Directory.Exists (Application.persistentDataPath + "/saves/")) Directory.CreateDirectory (Application.persistentDataPath + "/saves/");
    }

    public void Save () {
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
        foreach (StructureBehaviours structure in structuresManager.structures) {
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
            data.profile = itemsHandler.GetItemId (structure.profile);
            data.hull = structure.hull;
            data.factionID = structure.factionID;
            data.inventory = new List<StructureInventoryPair> ();
            foreach (Item item in structure.inventory.inventory.Keys.ToArray ())
                if (structure.inventory.GetItemCount (item) > 0)
                    data.inventory.Add (new StructureInventoryPair (itemsHandler.GetItemId (item), structure.inventory.GetItemCount (item)));
            data.turrets = new List<TurretHandler> ();
            for (int i = 0; i < structure.profile.turretSlots; i++) {
                data.turrets.Add (structure.turrets[i]);
                data.turrets[i].mountedID = itemsHandler.GetItemId (structure.turrets[i].turret);
            }
            data.shield = structure.shield;
            data.shield.mountedID = itemsHandler.GetItemId (structure.shield.shield);
            data.capacitor = structure.capacitor;
            data.capacitor.mountedID = itemsHandler.GetItemId (structure.capacitor.capacitor);
            data.generator = structure.generator;
            data.generator.mountedID = itemsHandler.GetItemId (structure.generator.generator);
            data.engine = structure.engine;
            data.engine.mountedID = itemsHandler.GetItemId (structure.engine.engine);
            data.electronics = structure.electronics;
            data.electronics.mountedID = itemsHandler.GetItemId (structure.electronics.electronics);
            data.tractorBeam = structure.tractorBeam;
            data.tractorBeam.mountedID = itemsHandler.GetItemId (structure.tractorBeam.tractorBeam);
            data.factories = new List<FactoryHandler> ();
            for (int i = 0; i < structure.profile.factories.Length; i++) {
                data.factories.Add (structure.factories[i]);
                data.factories[i].mountedID = itemsHandler.GetItemId (structure.factories[i].factory);
            }
            data.AI = structure.AI;
            data.docked = structure.docked;
            data.isPlayer = (playerController.structureBehaviours == structure);
            JumpGate jumpGate = structure.GetComponent<JumpGate> ();
            if (jumpGate != null && jumpGate.other != null) {
                JumpGateSaveData jumpGateData = new JumpGateSaveData ();
                jumpGateData.triggerRange = jumpGate.triggerRange;
                jumpGateData.forwardDistance = jumpGate.forwardDistance;
                jumpGateData.otherId = jumpGate.other.GetComponent<StructureBehaviours> ().id;
                data.jumpGateSaveData = jumpGateData;
            }
            universe.structures.Add (data);
        }

        universe.factions = factionsManager.factions;

        foreach (FactionPair pair in factionsManager.relations.Keys.ToArray ())
            universe.relations.Add (new SerializableFactionPair (pair), factionsManager.relations[pair]);

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
        File.WriteAllText (GetSavePath (universeName + "-" + nowString), JsonUtility.ToJson (universe, false));
    }

    public void Load (string savePath) {
        if (!File.Exists (savePath)) return;

        gameUIHandler.initialized = false;

        UniverseSaveData universe = JsonUtility.FromJson<UniverseSaveData> (File.ReadAllText (savePath));

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

        // Destroy all destroyables
        DestroyAfterT[] destroyables = FindObjectsOfType<DestroyAfterT> ();
        foreach (DestroyAfterT destroyable in destroyables) Destroy (destroyable.gameObject);

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
            structureBehaviours.profile = itemsHandler.GetItemById (data.profile) as StructureProfile;
            structureBehaviours.hull = data.hull;
            structureBehaviours.factionID = data.factionID;
            structureBehaviours.inventory = new InventoryHandler ();
            foreach (StructureInventoryPair pair in data.inventory)
                structureBehaviours.inventory.inventory.Add (itemsHandler.GetItemById (pair.item), pair.amount);
            for (int i = 0; i < data.turrets.Count; i++) {
                structureBehaviours.turrets.Add (data.turrets[i]);
                structureBehaviours.turrets[i].turret = itemsHandler.GetItemById (data.turrets[i].mountedID) as Turret;
            }
            structureBehaviours.shield = data.shield;
            structureBehaviours.shield.shield = itemsHandler.GetItemById (data.shield.mountedID) as Shield;
            structureBehaviours.capacitor = data.capacitor;
            structureBehaviours.capacitor.capacitor = itemsHandler.GetItemById (data.capacitor.mountedID) as Capacitor;
            structureBehaviours.generator = data.generator;
            structureBehaviours.generator.generator = itemsHandler.GetItemById (data.generator.mountedID) as Generator;
            structureBehaviours.engine = data.engine;
            structureBehaviours.engine.engine = itemsHandler.GetItemById (data.engine.mountedID) as Engine;
            structureBehaviours.electronics = data.electronics;
            structureBehaviours.electronics.electronics = itemsHandler.GetItemById (data.electronics.mountedID) as Electronics;
            structureBehaviours.tractorBeam = data.tractorBeam;
            structureBehaviours.tractorBeam.tractorBeam = itemsHandler.GetItemById (data.tractorBeam.mountedID) as TractorBeam;
            structureBehaviours.factories = new List<FactoryHandler> ();
            for (int i = 0; i < data.factories.Count; i++)
                structureBehaviours.factories.Add (data.factories[i]);
            structureBehaviours.AI = data.AI;
            structureBehaviours.docked = data.docked;
            if (data.jumpGateSaveData != null) {
                JumpGate jumpGate = structureBehaviours.gameObject.AddComponent<JumpGate> ();
                jumpGate.triggerRange = data.jumpGateSaveData.triggerRange;
                jumpGate.forwardDistance = data.jumpGateSaveData.forwardDistance;
                jumpGate.otherId = data.jumpGateSaveData.otherId;
            }
            loaded.Add (structureBehaviours);
            if (data.isPlayer) playerController.structureBehaviours = structureBehaviours;
        }
        foreach (StructureBehaviours structureBehaviours in loaded) {
            foreach (string dockedID in structureBehaviours.docked)
                foreach (StructureBehaviours possibleDocker in loaded)
                    if (possibleDocker.id == dockedID) {
                        possibleDocker.transform.parent = structureBehaviours.transform;
                        possibleDocker.transform.localPosition = possibleDocker.transform.position;
                    }
            JumpGate jumpGate = structureBehaviours.GetComponent<JumpGate> ();
            if (jumpGate != null)
                foreach (StructureBehaviours possibleConnection in loaded)
                    if (possibleConnection.id == jumpGate.otherId)
                        jumpGate.other = possibleConnection.GetComponent<JumpGate> ();
            structureBehaviours.Initialize ();
        }
        // Reset camera position
        cameraFollowPlayer.ResetPosition ();

        factionsManager.factions = universe.factions;

        foreach (SerializableFactionPair pair in universe.relations.Keys.ToArray ())
            factionsManager.relations.Add (new FactionPair (pair.a, pair.b, factionsManager), universe.relations[pair]);

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

        gameUIHandler.Initialize ();
    }

    public string GetSavePath (string saveName) {
        return Application.persistentDataPath + "/saves/" + saveName + ".txt";
    }

    public void LogSavePath (string saveName) {
        Debug.Log (GetSavePath (saveName));
    }
}