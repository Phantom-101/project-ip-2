using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class StructureBehaviours : MonoBehaviour {
    [Header ("Profile")]
    public StructureProfile profile;
    [Header ("Stats")]
    public float hull;
    public float hullTimeSinceLastDamaged;
    public bool cloaked;
    public string faction;
    [Header ("Saved Data")]
    public bool initializeAccordingToSaveData;
    public InventoryHandler savedInventory;
    public List<TurretHandler> savedTurrets = new List<TurretHandler> ();
    public ShieldHandler savedShield;
    public CapacitorHandler savedCapacitor;
    public GeneratorHandler savedGenerator;
    public EngineHandler savedEngine;
    public ElectronicsHandler savedElectronics;
    public TractorBeamHandler savedTractorBeam;
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
    [Header ("Physics")]
    public new Rigidbody rigidbody;
    public new ConstantForce constantForce;
    [Header ("AI")]
    public bool AIActivated;
    [Header ("Misc")]
    public StructureBehaviours targetted;
    public bool initialized;

    StructuresManager structuresManager;
    DiplomacyManager diplomacyManager;

    public void Initialize () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        diplomacyManager = FindObjectOfType<DiplomacyManager> ();
        structuresManager.AddStructure (this);
        GameObject meshGameObject = new GameObject ();
        meshGameObject.name = "Mesh and Collider";
        meshGameObject.transform.parent = transform;
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter> ();
        MeshCollider meshCollider = meshGameObject.AddComponent<MeshCollider> ();
        Renderer renderer = meshGameObject.AddComponent<MeshRenderer> ();
        meshCollider.sharedMesh = profile.mesh;
        meshCollider.convex = true;
        meshCollider.material = profile.physicMaterial;
        meshFilter.mesh = profile.mesh;
        renderer.material = profile.material;
        meshGameObject.transform.localPosition = profile.offset;
        meshGameObject.transform.localEulerAngles = profile.rotate;
        hull = profile.hull;
        if (initializeAccordingToSaveData) {
            inventory = new InventoryHandler (savedInventory, this);
            for (int i = 0; i < profile.turretSlots; i++) {
                turrets.Add (i < savedTurrets.Count ? new TurretHandler (savedTurrets[i], profile.turretPositions[i], profile.turretAlignments[i], this) :
                    new TurretHandler (null, profile.turretPositions[i], profile.turretAlignments[i], this));
                // Temporary
                if (turrets[i].turret.requireAmmunition) {
                    inventory.AddItem (turrets[i].turret.acceptedAmmunitions[0], 25);
                    turrets[i].UseAmmunition (turrets[i].turret.acceptedAmmunitions[0]);
                }
            }
            shield = new ShieldHandler (savedShield, this);
            capacitor = new CapacitorHandler (savedCapacitor, this);
            generator = new GeneratorHandler (savedGenerator, this);
            engine = new EngineHandler (savedEngine, this);
            electronics = profile.electronicsCapable ? new ElectronicsHandler (savedElectronics, this) : new ElectronicsHandler (this);
            tractorBeam = new TractorBeamHandler (savedTractorBeam, this);
        } else {
            inventory = new InventoryHandler (this, null, profile.inventorySize);
            for (int i = 0; i < profile.turretSlots; i++) turrets.Add (new TurretHandler (this, profile.turretPositions[i], profile.turretAlignments[i]));
            shield = new ShieldHandler (this);
            capacitor = new CapacitorHandler (this);
            generator = new GeneratorHandler (this);
            engine = new EngineHandler (this);
            electronics = new ElectronicsHandler (this);
            tractorBeam = new TractorBeamHandler (this);
        }
        rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.mass = profile.mass;
        rigidbody.drag = profile.drag;
        rigidbody.angularDrag = profile.angularDrag;
        // rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX;
        constantForce = GetComponent<ConstantForce> ();
        if (constantForce == null) constantForce = gameObject.AddComponent<ConstantForce> ();
        initialized = true;
    }

    void Update () {
        if (!initialized) return;
        // Check if should be destroyed
        if (hull == 0.0f) {
            structuresManager.RemoveStructure (this);
            Destroy (gameObject);
        }
        // Position and physics stuff
        transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
        rigidbody.velocity = new Vector3 (rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
        transform.localEulerAngles = new Vector3 (0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
        rigidbody.angularVelocity = new Vector3 (0.0f, rigidbody.angularVelocity.y, rigidbody.angularVelocity.z);
        // Hull damage timer
        if (hullTimeSinceLastDamaged > 1.5f) hullTimeSinceLastDamaged = 0.0f;
        else if (hullTimeSinceLastDamaged >= 0.3f) hullTimeSinceLastDamaged += Time.deltaTime;
        // Equipment
        if (turrets.Count != profile.turretSlots) turrets = new List<TurretHandler> (profile.turretSlots);
        shield.Process ();
        generator.GenerateEnergy (capacitor);
        capacitor.DistributeEnergy (turrets, shield, electronics, tractorBeam);
        engine.ApplySettings (GetComponent<ConstantForce> ());
        electronics.Process (gameObject);
        tractorBeam.Process (gameObject);
        // AI stuff
        if (AIActivated) {
            StructureBehaviours closest = null;
            float leastWeight = float.MaxValue;
            foreach (StructureBehaviours structure in structuresManager.structures) {
                float sizeDif = Mathf.Abs (profile.apparentSize - structure.profile.apparentSize);
                sizeDif = MathUtils.Clamp (sizeDif - 0.2f, 0.01f, 100.0f);
                float distance = Vector3.Distance (transform.position, structure.transform.position);
                float weight = distance + distance * sizeDif / 5.0f;
                if (structure != this && structure.faction != faction &&
                    diplomacyManager.GetRelations (faction, structure.faction) <= -0.5f && !structure.cloaked && weight < leastWeight &&
                    structure.transform.parent == transform.parent) {
                    leastWeight = weight;
                    closest = structure;
                }
            }
            if (closest != null) {
                targetted = closest;
                Debug.DrawRay (transform.position, targetted.transform.position - transform.position, Color.red);
                float totalRange = 0.0f;
                int effectiveTurrets = 0;
                foreach (TurretHandler turretHandler in turrets) {
                    turretHandler.Activate (targetted.gameObject);
                    Turret turret = turretHandler.turret;
                    if (turret != null) {
                        totalRange += turret.range;
                        effectiveTurrets ++;
                    }
                }
                float optimalRange = totalRange / effectiveTurrets * 0.75f;
                engine.forwardSetting = 1.0f;
                Vector3 heading = targetted.transform.position - transform.position;
                Vector3 perp = Vector3.Cross (transform.forward, heading);
                float leftRight = Vector3.Dot (perp, transform.up);
                float angle = targetted.transform.position - transform.position == Vector3.zero ?
                        0.0f :
                        Quaternion.Angle (transform.rotation, Quaternion.LookRotation (targetted.transform.position
                    - transform.position)
                );
                float lrMult = leftRight >= 0.0f ? 1.0f : -1.0f;
                angle *= lrMult;
                float approachAngle = 90.0f * lrMult;
                approachAngle -= (targetted.transform.position - transform.position).sqrMagnitude > optimalRange * optimalRange ? 60.0f * lrMult : 0.0f;
                approachAngle += (targetted.transform.position - transform.position).sqrMagnitude < optimalRange * optimalRange * 0.9f ? 60.0f * lrMult : 0.0f;
                Debug.DrawRay (transform.position, transform.rotation * Quaternion.Euler (0.0f, angle - approachAngle, 0.0f) * Vector3.forward * 10.0f * profile.apparentSize, Color.yellow);
                if (angle > approachAngle) engine.turnSetting = 1.0f;
                else if (angle > 0.0f && angle < approachAngle * 0.9) engine.turnSetting = -1.0f;
                else if (angle < -approachAngle) engine.turnSetting = -1.0f;
                else if (angle < 0.0f && angle > -approachAngle * 0.9) engine.turnSetting = 1.0f;
                else engine.turnSetting = 0.0f;
            } else {
                engine.forwardSetting = 0.0f;
                engine.turnSetting = 0.0f;
            }
        }
    }

    public void InstantiateProjectiles (TurretHandler turretHandler, GameObject target, Vector3 offset) {
        StartCoroutine (Fire (turretHandler, target, offset));
    }

    IEnumerator Fire (TurretHandler turretHandler, GameObject target, Vector3 offset) {
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
                        projectile.GetComponent<Projectile> ().Initialize (turret, turretHandler.usingAmmunition, gameObject, target, turretHandler.storedEnergy / turret.maxStoredEnergy, faction);
                    }
                    turretHandler.storedEnergy = 0.0f;
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
}