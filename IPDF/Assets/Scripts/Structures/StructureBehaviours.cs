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
    [Header ("Saved Equipment States")]
    public bool initializeAccordingToSaveData;
    public List<TurretHandler> savedTurrets = new List<TurretHandler> ();
    public ShieldHandler savedShield;
    public CapacitorHandler savedCapacitor;
    public GeneratorHandler savedGenerator;
    public EngineHandler savedEngine;
    public ElectronicsHandler savedElectronics;
    public TractorBeamHandler savedTractorBeam;
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

    public void Initialize () {
        GameObject meshGameObject = new GameObject ();
        meshGameObject.name = "Mesh and Collider";
        meshGameObject.transform.parent = transform;
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter> ();
        MeshCollider meshCollider = meshGameObject.AddComponent<MeshCollider> ();
        Renderer renderer = meshGameObject.AddComponent<MeshRenderer> ();
        meshCollider.sharedMesh = profile.mesh;
        meshCollider.convex = true;
        meshFilter.mesh = profile.mesh;
        renderer.material = profile.material;
        meshGameObject.transform.localPosition = profile.offset;
        meshGameObject.transform.localEulerAngles = profile.rotate;
        hull = profile.hull;
        if (initializeAccordingToSaveData) {
            for (int i = 0; i < profile.turretSlots; i++) {
                turrets.Add (i < savedTurrets.Count ? new TurretHandler (savedTurrets[i], this) : new TurretHandler (null, this));
                turrets[i].equipper = this;
            }
            shield = new ShieldHandler (savedShield, this);
            capacitor = new CapacitorHandler (savedCapacitor, this);
            generator = new GeneratorHandler (savedGenerator, this);
            engine = new EngineHandler (savedEngine, this);
            electronics = new ElectronicsHandler (savedElectronics, this);
            tractorBeam = new TractorBeamHandler (savedTractorBeam, this);
        } else {
            for (int i = 0; i < profile.turretSlots; i++) turrets.Add (new TurretHandler (this));
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
        if (hull == 0.0f) Destroy (gameObject);
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
            StructureBehaviours[] structures = FindObjectsOfType<StructureBehaviours> ();
            StructureBehaviours closest = null;
            float closestDis = float.MaxValue;
            foreach (StructureBehaviours structure in structures)
                if (structure != this && (transform.position - structure.transform.position).sqrMagnitude < closestDis) {
                    closestDis = (transform.position - structure.transform.position).sqrMagnitude;
                    closest = structure;
                }
            if (closest != null) {
                engine.forwardSetting = 1.0f;
                Vector3 heading = closest.transform.position - transform.position;
                Vector3 perp = Vector3.Cross (transform.forward, heading);
                float leftRight = Vector3.Dot (perp, transform.up);
                float angle = (closest.transform.position + closest.transform.rotation * closest.profile.offset) - (transform.position + transform.rotation * profile.offset) == Vector3.zero ? 0.0f : Quaternion.Angle (transform.rotation, Quaternion.LookRotation ((closest.transform.position + closest.transform.rotation * closest.profile.offset) - (transform.position + transform.rotation * profile.offset)));
                angle *= leftRight >= 0.0f ? 1.0f : -1.0f;
                if (angle > 90.0f) engine.turnSetting = 1.0f;
                else if (angle < -90.0f) engine.turnSetting = -1.0f;
                else engine.turnSetting = 0.0f;
            } else {
                engine.forwardSetting = 0.0f;
                engine.turnSetting = 0.0f;
            }
        }
    }

    public void TakeDamage (float amount, Vector3 from) {
        if (!initialized) return;
        float angle = from - (transform.position + transform.rotation * profile.offset) == Vector3.zero ? 0.0f : Quaternion.Angle (transform.rotation, Quaternion.LookRotation (from - (transform.position + transform.rotation * profile.offset)));
        Vector3 perp = Vector3.Cross(transform.forward, from - (transform.position + profile.offset));
        float leftRight = Vector3.Dot(perp, transform.up);
        angle *= leftRight >= 0.0f ? 1.0f : -1.0f;
        int directionalSector = 0;
        if (angle >= -150.0f && angle < -90.0f) directionalSector = 4;
        else if (angle >= -90.0f && angle < -30.0f) directionalSector = 5;
        else if (angle >= -30.0f && angle < 30.0f) directionalSector = 0;
        else if (angle >= 30.0f && angle < 90.0f) directionalSector = 1;
        else if (angle >= 90.0f && angle < 150.0f) directionalSector = 2;
        else directionalSector = 3;
        float residual = shield.TakeDamage (directionalSector, amount);
        if (residual > 0.0f) {
            hull = MathUtils.Clamp (hull - residual, 0.0f, profile.hull);
            hullTimeSinceLastDamaged = 0.3f;
        }
    }
}