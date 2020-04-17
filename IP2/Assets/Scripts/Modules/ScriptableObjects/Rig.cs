using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rig", menuName = "ScriptableObjects/Rig")]
public class Rig : ActiveModule {
    public string[] targetStats;
    public StructureStatModifierType[] modifierTypes;
    public float[] values;
    public float duration;
    public GameObject effect;
}
