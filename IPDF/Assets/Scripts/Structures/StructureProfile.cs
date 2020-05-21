using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure Profile", menuName = "Structures/Profile")]
public class StructureProfile : Item {
    [Header ("Appearance")]
    public Mesh mesh;
    public Material material;
    public float apparentSize;
    [Header ("Adjustables")]
    public bool isStation;
    public bool enforceHeight;
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
    public int turretSlots;
    public Vector3[] turretPositions;
    public TurretAlignment[] turretAlignments;
    public bool electronicsCapable;
    [Header ("Docking")]
    public int dockingPoints;
    public Vector3[] dockingLocations;
    public float dockingRange;
    [Header ("Physics")]
    public float mass;
    public float drag;
    public float angularDrag;
    public PhysicMaterial physicMaterial;
    [Header ("Correction")]
    public Vector3 offset;
    public Vector3 rotate;
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
    Carrier
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