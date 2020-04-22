using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/StatsModificationEquipment")]
public class StatsModificationEquipment : Equipment {
    public string[] effects;
    public StatModifierType[] modifierTypes;
    public float[] values;
    public string[] valueStats;
    public bool[] grantToTarget;
    public float duration;
    public string[] durationStats;
}
