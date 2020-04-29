using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StructureProfile", menuName = "ScriptableObjects/StructureProfile")]
public class StructureProfile : ScriptableObject {
    [Header("Rendering")]
    public Mesh mesh;
    [Header("Hitpoints")]
    public float hull;
    public float hullResistance;
    public float armor;
    public float armorResistance;
    public float shield;
    public float shieldResistance;
    [Header("Stats")]
    public float capacitance;
    public float generation;
    public float mass;
    public float drag;
    public float angularDrag;
    public float speed;
    public float turnSpeed;
    public float warpSpeed;
    public float warpAccuracy;
    public float warpFieldStrength;
    public float scannersRange;
    public float scannersStrength;
    public float signatureStrength;
    public float cargoHoldSize;
    public float inventorySize;
    [Header("Fitting")]
    public int equipmentMaxMeta;
    public Vector3[] equipmentLocations;
    [Header("Docking")]
    public Vector3[] dockingBayLocations;
    [Header("Overlay")]
    public Texture signatureTex;
}
