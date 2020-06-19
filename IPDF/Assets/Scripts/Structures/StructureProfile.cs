using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure Profile", menuName = "Structures/Profile")]
public class StructureProfile : Item {
    [Header ("Appearance")]
    public Mesh mesh;
    public Material material;
    public float apparentSize;
    public GameObject explosion;
    public Sprite selectableBillboard;
    [Header ("UI")]
    public Sprite hullUI;
    [Header ("Adjustables")]
    public bool enforceHeight;
    public bool canFireAt;
    [Header ("AI")]
    public float engagementRangeMultiplier;
    public float rangeChangeAngle;
    [Header ("Lore Attributes")]
    public StructureClass structureClass;
    public Manufacturer manufacturer;
    public string description;
    [Header ("Stats")]
    public float hull;
    public float inventorySize;
    [Header ("Fitting")]
    public int maxEquipmentMeta;
    public int turretSlots;
    public Vector3[] turretPositions;
    public Vector3[] turretRotations;
    public float[] turretAngles;
    public bool electronicsCapable;
    [Header ("Docking")]
    public Vector3[] dockingLocations;
    public float[] dockingSizes;
    public Vector3[] dockingRotations;
    public float dockingRange;
    [Header ("Economy")]
    public StructureMarket market;
    public Factory[] factories;
    [Header ("Physics")]
    public Mesh collisionMesh;
    public float mass;
    public float drag;
    public float angularDrag;
    public PhysicMaterial physicMaterial;
    [Header ("Decals")]
    public GameObject decals;
    public GameObject debris;
    [Header ("Correction")]
    public Vector3 offset;
    public Vector3 rotate;
    public Vector3 colliderOffset;
    public Vector3 colliderRotate;
}

public enum StructureClass {
    Station,
    Freighter,
    Craft,
    Corvette,
    Frigate,
    Destroyer,
    Cruiser,
    Battlecruiser,
    Battleship,
    Carrier,
    Other
}

public enum Manufacturer {
    CLL,
    ADR,
    Delta,
    TEF,
    NTE,
    Lambda,
    Omega,
    Cydia,
    Unknown
}