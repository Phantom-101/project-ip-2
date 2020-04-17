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
    public string targetStat;
    public StructureStatModifierType statModifierType;
    public float value;

    public StructureStatModifier(string targetStat, StructureStatModifierType statModifierType, float value) {
        this.targetStat = targetStat;
        this.statModifierType = statModifierType;
        this.value = value;
    }
}

public class StructureStatModifiersPackage {
    public List<StructureStatModifier> modifiers;
    public float duration;

    public StructureStatModifiersPackage(List<StructureStatModifier> modifiers, float duration) {
        this.modifiers = modifiers;
        this.duration = duration;
    }
}

public enum StructureStatModifierType {
    ImmediateAdditive,
    Additive,
    Multiplicative,
    Percent
}