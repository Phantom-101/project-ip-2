using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

public class StructureBehaviours : MonoBehaviour {
    [Header ("Profile")]
    public StructureProfile profile;
    [Header ("Stats")]
    public float hull;
    public float hullTimeSinceLastDamaged;
    public bool cloaked;
    public int factionID;
    [Header ("Navigation")]
    public Route route;
    public StructureBehaviours target;
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
    public GameObject[] docked;
    [Header ("Production")]
    public List<FactoryHandler> factories;
    [Header ("Physics")]
    public new Rigidbody rigidbody;
    public float dampening = 1;
    [Header ("AI")]
    public StructureAI AI;
    [Header ("Misc")]
    public StructureBehaviours targeted;
    public GameObject billboard;
    public bool initialized;
    [Header ("Component Cache")]
    public StructuresManager structuresManager;
    public FactionsManager factionsManager;
    public PlayerController playerController;
    public NavigationManager navigationManager;

    public void Initialize () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        factionsManager = FindObjectOfType<FactionsManager> ();
        playerController = FindObjectOfType<PlayerController> ();
        navigationManager = FindObjectOfType<NavigationManager> ();
        structuresManager.AddStructure (this);
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
        if (profile.structureClass != StructureClass.Station) meshCollider.convex = true;
        meshCollider.material = profile.physicMaterial;
        meshFilter.mesh = profile.mesh;
        renderer.material = profile.material;
        meshGameObject.transform.localPosition = profile.offset;
        meshGameObject.transform.localEulerAngles = profile.rotate;
        colliderGameObject.transform.localPosition = (profile.collisionMesh == null ? profile.offset : profile.colliderOffset);
        colliderGameObject.transform.localEulerAngles = (profile.collisionMesh == null ? profile.rotate : profile.colliderRotate);
        if (hull == 0) hull = profile.hull;
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
        if (factories.Count != profile.factories.Length) {
            factories = new List<FactoryHandler> ();
            for (int i = 0; i < profile.factories.Length; i++)
                factories.Add (new FactoryHandler (profile.factories[i]));
        }
        for (int i = 0; i < profile.factories.Length; i++)
            if (factories[i] == null)
                factories.Add (new FactoryHandler (profile.factories[i]));
        if (docked == null) docked = new GameObject[profile.dockingPoints];
        if (docked.Length != profile.dockingPoints) docked = new GameObject[profile.dockingPoints];
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
        GameObject billboardSprite = new GameObject ("Billboard");
        billboardSprite.transform.parent = transform;
        billboardSprite.AddComponent<Billboard> ();
        billboardSprite.transform.localPosition = Vector3.zero;
        billboard = new GameObject ("Sprite");
        billboard.transform.parent = billboardSprite.transform;
        billboard.AddComponent<SpriteRenderer> ();
        billboard.transform.localPosition = Vector3.zero;
        if (target != null) route = navigationManager.GetRoute (transform.position, target.transform.position);
        initialized = true;
    }

    void Update () {
        if (!initialized) return;
        // Check if should be destroyed
        if (hull == 0.0f) {
            if (profile.explosion != null) {
                GameObject explosion = Instantiate (profile.explosion, transform.position, RandomQuaternion (180)) as GameObject;
                explosion.transform.localScale = Vector3.one * profile.apparentSize * 5;
            }
            structuresManager.RemoveStructure (this);
        }
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
        else if (hullTimeSinceLastDamaged >= 0.3f) hullTimeSinceLastDamaged += Time.deltaTime;
        // Equipment
        if (turrets.Count != profile.turretSlots) turrets = new List<TurretHandler> (profile.turretSlots);
        for (int i = 0; i < profile.turretSlots; i++) {
            turrets[i].equipper = this;
            turrets[i].position = profile.turretPositions[i];
            turrets[i].turretAlignment = profile.turretAlignments[i];
            turrets[i].Process ();
        }
        shield.equipper = this;
        shield.Process ();
        generator.equipper = this;
        generator.GenerateEnergy (capacitor);
        capacitor.equipper = this;
        capacitor.DistributeEnergy (turrets, shield, electronics, tractorBeam);
        if (transform.parent.GetComponent<StructureBehaviours> ()) {
            engine.forwardSetting = 0.0f;
            engine.turnSetting = 0.0f;
        }
        engine.equipper = this;
        engine.ApplySettings (rigidbody);
        electronics.equipper = this;
        electronics.Process (gameObject);
        tractorBeam.equipper = this;
        tractorBeam.Process (gameObject);
        // Production
        inventory.storage = this;
        if (factories.Count != profile.factories.Length) factories = new List<FactoryHandler> (profile.factories.Length);
        foreach (FactoryHandler factory in factories) {
            factory.structure = this;
            factory.Process ();
        }
        // AI stuff
        if (AI != null) AI.Process (this);
    }

    public void InstantiateProjectiles (TurretHandler turretHandler, GameObject target, Vector3 offset) {
        StartCoroutine (Fire (turretHandler, target, offset));
    }

    IEnumerator Fire (TurretHandler turretHandler, GameObject target, Vector3 offset) {
        float cachedRatio = turretHandler.storedEnergy / turretHandler.turret.maxStoredEnergy;
        turretHandler.storedEnergy = 0.0f;
        Turret turret = turretHandler.turret;
        for (int i = 0; i < turret.activations; i++) {
            if (target != null) {
                if (!turret.requireAmmunition || inventory.HasItemCount (turretHandler.usingAmmunition, 1)) {
                    if (turret.requireAmmunition) inventory.RemoveItem (turretHandler.usingAmmunition, 1);
                    if (turret.projectile != null || turretHandler.usingAmmunition.projectile != null) {
                        GameObject projectile = MonoBehaviour.Instantiate (
                            turretHandler.usingAmmunition == null ? turret.projectile : turretHandler.usingAmmunition.projectile,
                            transform.position + transform.rotation * offset,
                            (turret.projectileInitializeRotation || (turretHandler.usingAmmunition == null ? false : turretHandler.usingAmmunition.projectileInitializeRotation) ? Quaternion.LookRotation (
                                CalculateLeadPosition (
                                    transform.position + transform.rotation * offset,
                                    target.transform.position,
                                    target.GetComponent<Rigidbody> ().velocity,
                                    turret.projectileVelocity,
                                    turret.leadProjectile
                                )
                            ) : transform.rotation) * RandomQuaternion (turret.projectileInaccuracy)
                        ) as GameObject;
                        projectile.GetComponent<Projectile> ().Initialize (turret, turretHandler.usingAmmunition, gameObject, target, cachedRatio, factionID);
                    }
                }
                yield return new WaitForSeconds (turret.activationDelay);
            }
        }
    }

    public Vector3 CalculateLeadPosition (Vector3 currentPosition, Vector3 targetPosition, Vector3 targetVelocity, float projectileVelocity, bool lead) {
        if (!lead) return targetPosition - currentPosition;
        float distance = Vector3.Distance(currentPosition, targetPosition);
        float travelTime = distance / projectileVelocity;
        return targetPosition + targetVelocity * travelTime - currentPosition;
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
    }

    public int GetSector (Vector3 to) {
        float angle = to - (transform.position + transform.rotation * profile.offset) == Vector3.zero ? 0.0f : Quaternion.Angle (transform.rotation, Quaternion.LookRotation (to - (transform.position + transform.rotation * profile.offset)));
        Vector3 perp = Vector3.Cross(transform.forward, to - (transform.position + profile.offset));
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
        if ((docker.transform.position - transform.position).sqrMagnitude > profile.dockingRange * profile.dockingRange) return;
        if (docker.profile.apparentSize > profile.maxDockingSize) return;
        for (int i = 0; i < profile.dockingPoints; i++) {
            if (docked[i] == null) {
                docker.transform.parent = transform;
                docked[i] = docker.gameObject;
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
        for (int i = 0; i < profile.dockingPoints; i++) {
            if (docked[i] == undocker.gameObject) {
                undocker.transform.parent = transform.parent;
                docked[i] = null;
                return;
            }
        }
    }
}