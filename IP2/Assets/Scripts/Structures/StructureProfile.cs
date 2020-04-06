using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StructureProfile", menuName = "ScriptableObjects/StructureProfile")]
public class StructureProfile : ScriptableObject
{
    public float hull;
    public float armor;
    public float shield;
    public float capacitance;
    public float generation;
    public float speed;
    public float turnSpeed;
    public float sensorRange;
    public float inventorySize;
    public int maxMods;
    public int maxTurrets;
    public Vector3[] turretLocations;
    public int maxBays;
    public int maxRigs;
    public Texture signature;
}
