using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureStat {
    public float baseValue;
    public List<StructureStatModifier> modifiers;

    public StructureStat(float baseValue) {
        this.baseValue = baseValue;
        modifiers = new List<StructureStatModifier>();
    }
}

public class StructureStatModifier {
    public StructureStatModifierType statModifierType;
    public float value;

    public StructureStatModifier(StructureStatModifierType statModifierType, float value) {
        this.statModifierType = statModifierType;
        this.value = value;
    }
}

public enum StructureStatModifierType {
    Additive,
    Multiplicative,
    ImmediateAdditive
}