using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/StatsModificationEquipment")]
public class StatsModificationEquipment : Equipment {
    public string[] effects;
    public StructureStatModifierType[] modifierTypes;
    public float[] values;
    public string[] valueStats;
    public bool[] grantToTarget;
    public string[] requirements;
    public string requirementStats;
    public float[] minValues;
    public float[] maxValues;
    public float duration;
    public string[] durationStats;
}
