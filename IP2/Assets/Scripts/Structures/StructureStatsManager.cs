using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureStatsManager : MonoBehaviour {
    public StructureProfile profile;
    public Dictionary<string, Stat> stats = new Dictionary<string, Stat>();
    public List<StatModifiersPackage> modifiersPackages = new List<StatModifiersPackage>();
    public List<DamageProfileStruct> damageStack = new List<DamageProfileStruct>();
    public string faction;

    StructuresManager structuresManager;
    StructureInitializer structureInitializer;
    StructureEquipmentManager structureEquipmentManager;

    // Has this component been initialized?
    bool initialized = false;

    public void Initialize(StructureInitializer initializer) {
        structureInitializer = initializer;
        structureEquipmentManager = initializer.structureEquipmentManager;
        // Setup mesh and collision
        GetComponent<MeshFilter>().mesh = profile.mesh;
        GetComponent<MeshCollider>().sharedMesh = profile.mesh;
        // Register structure
        structuresManager = FindObjectOfType<StructuresManager>();
        structuresManager.AddStructure(this);
        // Initialize stats dictionary
        InitializeStats();
        initialized = true;
    }

    void Update() {
        if(!initialized) return;
        IterateModifiersPackages();
        AddModifier(new StatModifier("Capacitance", StatModifierType.ImmediateAdditive, GetStat("Generation") * Time.deltaTime));
        CheckStats();
    }

    void InitializeStats() {
        // Structure stats
        stats.Add("Hull Max", new Stat(profile.hull));
        stats.Add("Hull", new Stat(profile.hull));
        stats.Add("Armor Max", new Stat(profile.armor));
        stats.Add("Armor", new Stat(profile.armor));
        stats.Add("Shield Max", new Stat(profile.shield));
        stats.Add("Shield", new Stat(profile.shield));
        stats.Add("Equipment Damage", new Stat(0.0f));
        stats.Add("Capacitance Max", new Stat(profile.capacitance));
        stats.Add("Capacitance", new Stat(profile.capacitance));
        stats.Add("Generation", new Stat(profile.generation));
        stats.Add("Speed", new Stat(profile.speed));
        stats.Add("Turn Speed", new Stat(profile.turnSpeed));
        stats.Add("Warp Speed", new Stat(profile.warpSpeed));
        stats.Add("Warp Accuracy", new Stat(profile.warpAccuracy));
        stats.Add("Warp Field Strength", new Stat(profile.warpFieldStrength));
        stats.Add("Scanners Range", new Stat(profile.scannersRange));
        stats.Add("Scanners Strength", new Stat(profile.scannersStrength));
        stats.Add("Signature Strength", new Stat(profile.signatureStrength));
        stats.Add("Cargo Hold Size", new Stat(profile.cargoHoldSize));
        // Resistances
        stats.Add("Hull Resistance", new Stat(profile.hullResistance));
        stats.Add("Armor Resistance", new Stat(profile.armorResistance));
        stats.Add("Shield Resistance", new Stat(profile.shieldResistance));
        // Multipliers
        stats.Add("Turrets Damage", new Stat(1.0f));
    }

    public void AddModifiersPackage(StatModifiersPackage package) {
        if(!initialized) return;
        modifiersPackages.Add(package);
        foreach(StatModifier modifier in package.modifiers) AddModifier(modifier);
    }

    public void IterateModifiersPackages() {
        if(!initialized) return;
        foreach(StatModifiersPackage package in modifiersPackages.ToArray()) {
            float d = package.duration - Time.deltaTime;
            if(d > 0.0f || package.durationType == DurationType.Infinite) {
                StatModifiersPackage newPackage = new StatModifiersPackage(package.modifiers, package.durationType, d);
                modifiersPackages.Remove(package);
                modifiersPackages.Add(newPackage);
            } else {
                foreach(StatModifier modifier in package.modifiers) RemoveModifier(modifier);
            }
        }
    }

    public void AddModifier(StatModifier modifier) {
        if(!initialized) return;
        if(modifier.statModifierType == StatModifierType.ImmediateAdditive) {
            Stat newStat = new Stat(stats[modifier.targetStat].baseValue + modifier.value);
            stats[modifier.targetStat] = newStat;
        }
        else stats[modifier.targetStat].modifiers.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier) {
        if(!initialized) return;
        stats[modifier.targetStat].modifiers.Remove(modifier);
    }

    public void SetStat(string statName, float v) {
        if(!initialized) return;
        Stat newStat = new Stat(v);
        stats[statName] = newStat;
    }

    public float GetStat(string statName) {
        if(!initialized) return;
        float v = stats[statName].baseValue;
        float additive = 0.0f;
        float multiplicative = 1.0f;
        float percent = 0.0f;
        foreach(StatModifier mod in stats[statName].modifiers) {
            if(mod.statModifierType == StatModifierType.Additive) {
                additive += mod.value;
            } else if (mod.statModifierType == StatModifierType.Multiplicative) {
                multiplicative *= mod.value;
            } else if(mod.statModifierType == StatModifierType.Percent) {
                percent += mod.value;
            }
        }
        return v + additive + (multiplicative * v - v) + (percent / 100.0f) * v;
    }

    void CheckStats() {
        ApplyDamage();
        if (GetStat("Hull") <= 0.0f) structuresManager.Destroyed(this);
        if (GetStat("Hull") > GetStat("Hull Max")) SetStat("Hull", GetStat("Hull Max"));
        if (GetStat("Armor") < 0.0f) SetStat("Armor", 0.0f);
        if (GetStat("Armor") > GetStat("Armor Max")) SetStat("Armor", GetStat("Armor Max"));
        if (GetStat("Shield") < 0.0f) SetStat("Shield", 0.0f);
        if (GetStat("Shield") > GetStat("Shield Max")) SetStat("Shield", GetStat("Shield Max"));
        if (GetStat("Capacitance") > GetStat("Capacitance Max")) SetStat("Capacitance", GetStat("Capacitance Max"));
        bool hasEquipment = false;
        foreach(Equipment e in structureEquipmentManager.equipment) {
            if(e != null) {
                hasEquipment = true;
                break;
            }
        }
        while (GetStat("Equipment Damage") > 0) {
            if(!hasEquipment) {
                SetStat("Equipment Damage", 0.0f);
                break;
            }
            int randIndex = Random.Range(0, structureEquipmentManager.equipment.Count - 1);
            structureEquipmentManager.equipmentGOs[randIndex].GetComponent<EquipmentAttachmentPoint>().ChangeHitpoints(-GetStat("Equipment Damage"));
        }
    }

    public void AddDamage(DamageProfileStruct damageProfileStruct) {
        if(!initialized) return;
        damageStack.Add(damageProfileStruct);
    }

    void ApplyDamage() {
        foreach(DamageProfileStruct damageProfileStruct in damageStack.ToArray()) {
            DamageProfileStruct final = ApplyDamageToHull(ApplyDamageToArmor(ApplyDamageToShield(damageProfileStruct)));
            if(final.value == 0.0f) damageStack.Remove(damageProfileStruct);
        }
    }

    DamageProfileStruct ApplyDamageToShield(DamageProfileStruct damageProfileStruct) {
        if (damageProfileStruct.value == 0.0f) return damageProfileStruct;
        if (damageProfileStruct.bypassShield) return damageProfileStruct;
        float shield = GetStat("Shield");
        float resist = GetStat("Shield Resistance");
        float damage = damageProfileStruct.value * (1 - resist) * damageProfileStruct.againstShield;
        if(damage <= shield) {
            AddModifier(new StatModifier("Shield", StatModifierType.ImmediateAdditive, -damage));
            return new DamageProfileStruct(0.0f);
        } else {
            SetStat("Shield", 0.0f);
            return new DamageProfileStruct(damageProfileStruct, -shield / (damageProfileStruct.againstShield * (1 - resist)));
        }
    }

    DamageProfileStruct ApplyDamageToArmor(DamageProfileStruct damageProfileStruct) {
        if (damageProfileStruct.value == 0.0f) return damageProfileStruct;
        if (damageProfileStruct.bypassArmor) return damageProfileStruct;
        float armor = GetStat("Armor");
        float resist = GetStat("Armor Resistance");
        float damage = damageProfileStruct.value * (1 - resist) * damageProfileStruct.againstArmor;
        if(damage <= armor) {
            AddModifier(new StatModifier("Armor", StatModifierType.ImmediateAdditive, -damage));
            return new DamageProfileStruct(0.0f);
        } else {
            SetStat("Armor", 0.0f);
            return new DamageProfileStruct(damageProfileStruct, -armor / (damageProfileStruct.againstArmor * (1 - resist)));
        }
    }

    DamageProfileStruct ApplyDamageToHull(DamageProfileStruct damageProfileStruct) {
        if (damageProfileStruct.value == 0.0f) return damageProfileStruct;
        if (damageProfileStruct.bypassHull) return damageProfileStruct;
        float hull = GetStat("Hull");
        float resist = GetStat("Hull Resistance");
        float damage = damageProfileStruct.value * (1 - resist) * damageProfileStruct.againstHull;
        if(damage <= hull) {
            AddModifier(new StatModifier("Hull", StatModifierType.ImmediateAdditive, -damage));
            return new DamageProfileStruct(0.0f);
        } else {
            SetStat("Hull", 0.0f);
            return new DamageProfileStruct(damageProfileStruct, -hull / (damageProfileStruct.againstHull * (1 - resist)));
        }
    }
}
