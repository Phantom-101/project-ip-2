using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure Profile", menuName = "Structures/Profile")]
public class StructureProfile : Item {
    [Header ("Appearance")]
    public Mesh mesh;
    public Material material;
    [Header ("Stats")]
    public float hull;
    public int turretSlots;
    [Header ("Physics")]
    public float drag;
    public float angularDrag;
    [Header ("Correction")]
    public float yOffset;
}
