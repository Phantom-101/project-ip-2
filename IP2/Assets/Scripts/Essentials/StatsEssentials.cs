using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat {
    public float baseValue;
    public List<StatModifier> modifiers;

    public Stat(float baseValue) {
        this.baseValue = baseValue;
        modifiers = new List<StatModifier>();
    }
}

public struct StatModifier {
    public string targetStat;
    public StatModifierType statModifierType;
    public float value;

    public StatModifier(string targetStat, StatModifierType statModifierType, float value) {
        this.targetStat = targetStat;
        this.statModifierType = statModifierType;
        this.value = value;
    }
}

public struct StatModifiersPackage {
    public List<StatModifier> modifiers;
    public DurationType durationType;
    public float duration;

    public StatModifiersPackage(List<StatModifier> modifiers, DurationType durationType, float duration) {
        this.modifiers = modifiers;
        this.durationType = durationType;
        this.duration = duration;
    }
}

public enum StatModifierType {
    ImmediateAdditive,
    Additive,
    Multiplicative,
    Percent
}