using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DurationType {
    Finite,
    Infinite
}

[CreateAssetMenu(fileName = "New ESMP", menuName = "ScriptableObjects/EquipmentStatsModificationProfile")]
public class EquipmentStatsModificationProfile : ScriptableObject {
    [Header("Effects")]
    public string[] effects;
    public StatModifierType[] modifierTypes;
    public float[] values;
    public string[] valueStats;
    public bool[] grantToTarget;
    public DurationType durationType;
    public float duration;
    public string[] durationStats;
}
