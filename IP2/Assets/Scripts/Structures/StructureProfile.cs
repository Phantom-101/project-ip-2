using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StructureProfile", menuName = "ScriptableObjects/StructureProfile")]
public class StructureProfile : ScriptableObject {
    public Mesh mesh;
    public float hull;
    public float hullResistance;
    public float armor;
    public float armorResistance;
    public float shield;
    public float shieldResistance;
    public float damagePool;
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
    public int maxEquipment;
    public int equipmentMaxMeta;
    public Vector3[] equipmentLocations;
    public Texture signatureTex;
}
