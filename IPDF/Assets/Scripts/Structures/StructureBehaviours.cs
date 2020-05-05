using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class StructureBehaviours : MonoBehaviour {
    [Header ("Profile")]
    public StructureProfile profile;
    [Header ("Stats")]
    public float hull;
    public bool cloaked;
    [Header ("Equipment")]
    public Item[] savedEquipment;
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
    [Header ("Misc")]
    public GameObject targetted;
    public bool initialized;

    public void Initialize () {
        hull = profile.hull;
        if (savedEquipment == null || savedEquipment.Length != profile.turretSlots + 6) {
            for (int i = 0; i < profile.turretSlots; i++) turrets.Add (new TurretHandler ());
            shield = new ShieldHandler ();
            capacitor = new CapacitorHandler ();
            generator = new GeneratorHandler ();
            engine = new EngineHandler ();
            electronics = new ElectronicsHandler ();
            tractorBeam = new TractorBeamHandler ();
        } else {
            for (int i = 0; i < profile.turretSlots; i++) turrets.Add (new TurretHandler (savedEquipment[i] as Turret));
            shield = new ShieldHandler (savedEquipment[profile.turretSlots] as Shield);
            capacitor = new CapacitorHandler (savedEquipment[profile.turretSlots + 1] as Capacitor);
            generator = new GeneratorHandler (savedEquipment[profile.turretSlots + 2] as Generator);
            engine = new EngineHandler (savedEquipment[profile.turretSlots + 3] as Engine);
            electronics = new ElectronicsHandler (savedEquipment[profile.turretSlots + 4] as Electronics);
            tractorBeam = new TractorBeamHandler (savedEquipment[profile.turretSlots + 5] as TractorBeam);
        }
        rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.drag = profile.drag;
        rigidbody.angularDrag = profile.angularDrag;
        constantForce = GetComponent<ConstantForce> ();
        if (constantForce == null) constantForce = gameObject.AddComponent<ConstantForce> ();
        initialized = true;
    }

    public void InitializePlayerController () {
        PlayerController playerController = FindObjectOfType<PlayerController> ();
        if (playerController != null) playerController.Initialize (this);
    }

    void Update () {
        if (!initialized) return;
        if (hull == 0.0f) Destroy (gameObject);
        if (turrets.Count != profile.turretSlots) turrets = new List<TurretHandler> (profile.turretSlots);
        generator.GenerateEnergy (capacitor);
        capacitor.DistributeEnergy (turrets, shield, electronics, tractorBeam);
        engine.ApplySettings (GetComponent<ConstantForce> ());
        electronics.Process (gameObject);
        tractorBeam.Process (gameObject);
    }

    public void TakeDamage (float amount, Vector3 from) {
        if (!initialized) return;
        float angle = from - transform.position == Vector3.zero ? 0.0f : Quaternion.Angle (transform.rotation, Quaternion.LookRotation (from - transform.position));
        Vector3 perp = Vector3.Cross(transform.forward, from - transform.position);
        float leftRight = Vector3.Dot(perp, transform.up);
        angle *= leftRight >= 0.0f ? 1.0f : -1.0f;
        int directionalSector = 0;
        if (angle >= -150.0f && angle < -90.0f) directionalSector = 4;
        else if (angle >= -90.0f && angle < -30.0f) directionalSector = 5;
        else if (angle >= -30.0f && angle < 30.0f) directionalSector = 0;
        else if (angle >= 30.0f && angle < 90.0f) directionalSector = 1;
        else if (angle >= 90.0f && angle < 150.0f) directionalSector = 2;
        else directionalSector = 3;
        if (shield.strengths[directionalSector] > 0.0f)
            shield.strengths[directionalSector] = MathUtils.Clamp (shield.strengths[directionalSector] - amount, 0.0f, shield.shield.strength);
        else hull = MathUtils.Clamp (hull - amount, 0.0f, profile.hull);
    }
}