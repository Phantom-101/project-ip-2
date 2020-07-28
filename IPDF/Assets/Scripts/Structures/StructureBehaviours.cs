using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

public class StructureBehaviours : MonoBehaviour {
    [Header ("Profile")]
    public StructureProfile profile;
    [Header ("Stats")]
    public string id;
    public float hull;
    public float hullTimeSinceLastDamaged;
    public bool destroyed;
    public bool cloaked;
    public int factionID;
    public Faction faction;
    [Header ("Navigation")]
    public Sector sector;
    public Route route;
    [Header ("Inventory")]
    public InventoryHandler inventory;
    [Header ("Equipment Handlers")]
    public List<TurretHandler> turrets = new List<TurretHandler> ();
    public ShieldHandler shield;
    public CapacitorHandler capacitor;
    public GeneratorHandler generator;
    public EngineHandler engine;
    public ElectronicsHandler electronics;
    public TractorBeamHandler tractorBeam;
    [Header ("Docking")]
    public string[] docked;
    [Header ("Production")]
    public List<FactoryHandler> factories;
    [Header ("Physics")]
    public Rigidbody rigidbody;
    public float dampening = 1;
    [Header ("AI")]
    public StructureAI AI;
    [Header ("Misc")]
    public StructureBehaviours targeted;
    public GameObject billboard;
    public bool initialized;
    [Header ("Component Cache")]
    public AudioSource audioSource;
    public StructuresManager structuresManager;
    public FactionsManager factionsManager;
    public PlayerController playerController;
    public NavigationManager navigationManager;

    public void Initialize () {
        structuresManager = StructuresManager.GetInstance ();
        factionsManager = FactionsManager.GetInstance ();
        faction = factionsManager.GetFaction (factionID);
        playerController = PlayerController.GetInstance ();
        navigationManager = NavigationManager.GetInstance ();
        structuresManager.AddStructure (this);
        AudioAsset ambience = profile.ambience;
        if (ambience != null) {
            audioSource = gameObject.AddComponent<AudioSource> ();
            audioSource.spatialBlend = ambience.spatialBlend;
            audioSource.rolloffMode = ambience.rolloffMode;
            audioSource.minDistance = ambience.minDistance;
            audioSource.maxDistance = ambience.maxDistance;
        }
        GameObject meshGameObject = new GameObject ();
        meshGameObject.name = "Mesh";
        meshGameObject.transform.parent = transform;
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter> ();
        Renderer renderer = meshGameObject.AddComponent<MeshRenderer> ();
        GameObject colliderGameObject = new GameObject ();
        colliderGameObject.name = "Collider";
        colliderGameObject.transform.parent = transform;
        MeshCollider meshCollider = colliderGameObject.AddComponent<MeshCollider> ();
        meshCollider.sharedMesh = (profile.collisionMesh == null ? profile.mesh : profile.collisionMesh);
        if (profile.structureClass != StructureClass.Station)
            meshCollider.convex = true;
        meshCollider.material = profile.physicMaterial;
        meshFilter.mesh = profile.mesh;
        renderer.material = profile.material;
        meshGameObject.transform.localPosition = profile.offset;
        meshGameObject.transform.localEulerAngles = profile.rotate;
        colliderGameObject.transform.localPosition = (profile.collisionMesh == null ? profile.offset : profile.colliderOffset);
        colliderGameObject.transform.localEulerAngles = (profile.collisionMesh == null ? profile.rotate : profile.colliderRotate);
        if (hull == 0 && !destroyed) hull = profile.hull;
        if (inventory == null) inventory = new InventoryHandler (null, profile.inventorySize);
        if (inventory.inventory == null) inventory.inventory = new Dictionary<Item, int> ();
        if (turrets.Count != profile.turretSlots) {
            turrets = new List<TurretHandler> ();
            for (int i = 0; i < profile.turretSlots; i++)
                turrets.Add (new TurretHandler ());
        }
        for (int i = 0; i < profile.turretSlots; i++)
            if (turrets[i] == null) 
                turrets.Add (new TurretHandler ());
        if (shield == null) shield = new ShieldHandler ();
        if (capacitor == null) capacitor = new CapacitorHandler ();
        if (generator == null) generator = new GeneratorHandler ();
        if (engine == null) engine = new EngineHandler ();
        if (electronics == null) electronics = new ElectronicsHandler ();
        if (tractorBeam == null) tractorBeam = new TractorBeamHandler ();
        if (docked == null) docked = new string[profile.dockingLocations.Length];
        if (docked.Length != profile.dockingLocations.Length) docked = new string[profile.dockingLocations.Length];
        rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.mass = profile.mass;
        if (profile.structureClass == StructureClass.Station) rigidbody.isKinematic = true;
        if (profile.decals != null) {
            GameObject decals = Instantiate (profile.decals) as GameObject;
            decals.transform.parent = transform;
            decals.transform.localPosition = Vector3.zero;
            decals.transform.localEulerAngles = Vector3.zero;
        }
        sector = transform.parent.GetComponent<Sector> ();
        if (sector == null) sector = transform.parent.parent.GetComponent<Sector> ();
        if (!sector.inSector.Contains (this)) sector.inSector.Add (this);
        StartCoroutine (AmbientSound ());
        initialized = true;
    }

    public void Tick (float deltaTime) {
        if (!initialized) return;
        if (factories.Count != profile.factories.Length) {
            factories = new List<FactoryHandler> ();
            for (int i = 0; i < profile.factories.Length; i++)
                factories.Add (new FactoryHandler (profile.factories[i]));
        }
        for (int i = 0; i < profile.factories.Length; i++)
            factories[i].factory = profile.factories[i];
        if (targeted != null && !targeted.CanBeTargeted ()) targeted = null;
        if (playerController.structureBehaviours != this)
            if (AI == null)
                AI = new StructureAI ();
        // Position and physics stuff
        if (profile.enforceHeight) {
            transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
            rigidbody.velocity = new Vector3 (rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
        }
        rigidbody.drag = profile.drag * dampening;
        rigidbody.angularDrag = profile.angularDrag * dampening;
        transform.localEulerAngles = new Vector3 (0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        rigidbody.angularVelocity = new Vector3 (0.0f, rigidbody.angularVelocity.y, rigidbody.angularVelocity.z);
        // Hull damage timer
        if (hullTimeSinceLastDamaged > 1.5f) hullTimeSinceLastDamaged = 0.0f;
        else if (hullTimeSinceLastDamaged >= 0.3f) hullTimeSinceLastDamaged += deltaTime;
        // Equipment
        if (turrets.Count != profile.turretSlots) turrets = new List<TurretHandler> (profile.turretSlots);
        for (int i = 0; i < profile.turretSlots; i++) {
            turrets[i].equipper = this;
            turrets[i].position = profile.turretPositions[i];
            turrets[i].rotation = profile.turretRotations[i];
            turrets[i].angle = profile.turretAngles[i];
            turrets[i].Process (deltaTime);
        }
        shield.equipper = this;
        shield.Process (deltaTime);
        generator.equipper = this;
        generator.GenerateEnergy (deltaTime, capacitor);
        capacitor.equipper = this;
        capacitor.DistributeEnergy (deltaTime, turrets, shield, electronics, tractorBeam);
        if (transform.parent.GetComponent<StructureBehaviours> ()) {
            engine.forwardSetting = 0.0f;
            engine.turnSetting = 0.0f;
        }
        engine.equipper = this;
        engine.ApplySettings (deltaTime, rigidbody);
        electronics.equipper = this;
        electronics.Process (deltaTime, gameObject);
        tractorBeam.equipper = this;
        tractorBeam.Process (deltaTime, gameObject);
        // Production
        inventory.storage = this;
        if (factories.Count != profile.factories.Length) factories = new List<FactoryHandler> (profile.factories.Length);
        foreach (FactoryHandler factory in factories) {
            factory.structure = this;
            factory.Process ();
        }
        // AI stuff
        if (AI != null) AI.Process (this, deltaTime);
    }

    public Quaternion RandomQuaternion (float maxRandom) {
        return Quaternion.Euler (
            UnityEngine.Random.Range(-maxRandom, maxRandom),
            UnityEngine.Random.Range(-maxRandom, maxRandom),
            UnityEngine.Random.Range(-maxRandom, maxRandom)
        );
    }

    public void TakeDamage (float amount, Vector3 from) {
        if (!initialized) return;
        int directionalSector = GetSector (from);
        float residual = shield.TakeDamage (directionalSector, amount);
        if (residual > 0.0f) {
            hull = MathUtils.Clamp (hull - residual, 0.0f, profile.hull);
            hullTimeSinceLastDamaged = 0.3f;
        }

        // Check if should be destroyed
        if (hull == 0.0f && !destroyed) {
            destroyed = true;
            structuresManager.RemoveStructure (this);
            StartCoroutine (DestructionSequence ());
        }
    }

    public int GetSector (Vector3 to) {
        float angle = to - (transform.localPosition + transform.rotation * profile.offset) == Vector3.zero ? 0.0f : Quaternion.Angle (transform.rotation, Quaternion.LookRotation (to - (transform.localPosition + transform.rotation * profile.offset)));
        Vector3 perp = Vector3.Cross(transform.forward, to - (transform.localPosition + profile.offset));
        float leftRight = Vector3.Dot(perp, transform.up);
        angle *= leftRight >= 0.0f ? 1.0f : -1.0f;
        int directionalSector = 0;
        if (angle >= -150.0f && angle < -90.0f) directionalSector = 4;
        else if (angle >= -90.0f && angle < -30.0f) directionalSector = 5;
        else if (angle >= -30.0f && angle < 30.0f) directionalSector = 0;
        else if (angle >= 30.0f && angle < 90.0f) directionalSector = 1;
        else if (angle >= 90.0f && angle < 150.0f) directionalSector = 2;
        else directionalSector = 3;
        return directionalSector;
    }

    public void Dock (StructureBehaviours docker) {
        if (!DockerCanDock (docker)) return;
        for (int i = 0; i < profile.dockingLocations.Length; i++) {
            if (System.String.IsNullOrEmpty (docked[i]) && profile.dockingSizes[i] >= docker.profile.apparentSize) {
                docker.transform.parent = transform;
                docked[i] = docker.id;
                docker.transform.localPosition = profile.dockingLocations[i];
                docker.transform.localEulerAngles = profile.dockingRotations[i];
                if (playerController.structureBehaviours == docker) {
                    playerController.Reset ();
                    FindObjectOfType<CameraFollowPlayer> ().ResetPosition ();
                }
                docker.engine.forwardSetting = 0.0f;
                docker.engine.turnSetting = 0.0f;
                docker.electronics.Deactivate ();
                docker.tractorBeam.Deactivate ();
                Rigidbody dockerRigidbody = docker.GetComponent<Rigidbody> ();
                dockerRigidbody.velocity = Vector3.zero;
                dockerRigidbody.angularVelocity = Vector3.zero;
                return;
            }
        }
    }

    public void Undock (StructureBehaviours undocker) {
        for (int i = 0; i < profile.dockingLocations.Length; i++) {
            if (docked[i] == undocker.id) {
                undocker.transform.parent = transform.parent;
                docked[i] = "";
                return;
            }
        }
    }

    public bool DockerCanDock (StructureBehaviours docker) {
        if (Vector3.Distance (docker.transform.localPosition, transform.localPosition) > profile.dockingRange) return false;
        for (int i = 0; i < profile.dockingLocations.Length; i++)
            if (System.String.IsNullOrEmpty (docked[i]) && profile.dockingSizes[i] >= docker.profile.apparentSize)
                return true;
        return false;
    }

    public bool CanShoot () {
        if (destroyed || cloaked) return false;
        return true;
    }

    public bool CanBeTargeted () {
        if (destroyed || cloaked) return false;
        return true;
    }

    IEnumerator DestructionSequence () {
        GameObject explosion;
        Vector3[] vertices = profile.mesh.vertices;
        if (profile.explosion != null) {
            for (int i = 0; i < 5; i++) {
                explosion = Instantiate (profile.explosion, GetExplosionPosition (vertices), Quaternion.identity) as GameObject;
                explosion.transform.localScale = Vector3.one * profile.apparentSize / 5;
                yield return new WaitForSeconds (Random.Range (0.5f, 1));
            }
            explosion = Instantiate (profile.explosion, transform.position, Quaternion.identity) as GameObject;
            explosion.transform.localScale = Vector3.one * profile.apparentSize;
        }
        foreach (Item item in inventory.inventory.Keys.ToArray ()) {
            if (inventory.GetItemCount (item) > 0) {
                GameObject pod = Instantiate (profile.pod, transform.position, Quaternion.identity) as GameObject;
                pod.transform.parent = transform.parent;
                pod.GetComponent<StructureBehaviours> ().inventory.AddItem (item, inventory.inventory[item]);
                pod.AddComponent<CargoPod> ();
            }
        }
        if (profile.debris != null) {
            GameObject debris = Instantiate (profile.debris,
                transform.position,
                Quaternion.Euler (new Vector3 (Random.Range (-180, 180), Random.Range (-180, 180), Random.Range (-180, 180)))
            ) as GameObject;
            debris.transform.localScale = Vector3.one * profile.apparentSize / 3;
        }
        Destroy (gameObject);
    }
    
    Vector3 GetExplosionPosition (Vector3[] candidates) {
        return transform.position + transform.rotation * (candidates[Random.Range (0, candidates.Length - 1)] / 2);
    }

    IEnumerator AmbientSound () {
        AudioAsset ambience = profile.ambience;
        if (ambience == null) yield break;
        audioSource.PlayOneShot (ambience.clip, ambience.volume);
        yield return new WaitForSeconds (ambience.clip == null ? 15 : ambience.clip.length);
        StartCoroutine (AmbientSound ());
    }
}