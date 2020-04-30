using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureStatsManager : MonoBehaviour {
    public float[] hitpoints;
    public float capacitor;
    public StructureProfile profile;
    public Dictionary<string, Stat> stats = new Dictionary<string, Stat>();
    public List<StatModifiersPackage> modifiersPackages = new List<StatModifiersPackage>();
    public List<HealthChange> healthChangesStack = new List<HealthChange>();
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
        hitpoints = new float[3];
        hitpoints[0] = profile.hitpoints[0];
        hitpoints[1] = profile.hitpoints[1];
        hitpoints[2] = profile.hitpoints[2];
        capacitor = profile.capacitance;
        // Structure stats
        stats.Add("Hitpoint 0", new Stat(profile.hitpoints[0]));
        stats.Add("Hitpoint 1", new Stat(profile.hitpoints[1]));
        stats.Add("Hitpoint 2", new Stat(profile.hitpoints[2]));
        stats.Add("Equipment Damage", new Stat(0.0f));
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
        stats.Add("Resistance 0", new Stat(profile.resistances[0]));
        stats.Add("Resistance 1", new Stat(profile.resistances[1]));
        stats.Add("Resistance 2", new Stat(profile.resistances[2]));
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
        if(!initialized) return 0.0f;
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
        ApplyHealthChanges();
        if(hitpoints[0] <= 0.0f) structuresManager.Destroyed(this);
        else if (hitpoints[0] > GetStat("Hitpoint 0")) hitpoints[0] = GetStat("Hitpoint 0");
        if(hitpoints[1] <= 0.0f) hitpoints[1] = 0.0f;
        else if (hitpoints[1] > GetStat("Hitpoint 1")) hitpoints[1] = GetStat("Hitpoint 1");
        if(hitpoints[2] <= 0.0f) hitpoints[2] = 0.0f;
        else if (hitpoints[2] > GetStat("Hitpoint 2")) hitpoints[2] = GetStat("Hitpoint 2");
        if(hitpoints[0] <= 0.0f) structuresManager.Destroyed(this);
        else if (capacitor > GetStat("Capacitance")) capacitor = GetStat("Capacitance");
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

    public void AddHealthChange(HealthChange healthChange) {
        if(!initialized) return;
        healthChangesStack.Add(healthChange);
    }

    void ApplyHealthChanges() {
        foreach(HealthChange healthChange in healthChangesStack.ToArray()) {
            ApplyHealthChange(healthChange);
            healthChangesStack.Remove(healthChange);
        }
    }

    void ApplyHealthChange(HealthChange healthChange) {
        for(int i = hitpoints.Length - 1; i >= 0; i--) {
            if(!healthChange.bypasses[i]) {
                float curHp = hitpoints[i];
                float resist = GetStat("Resistance " + i);
                float change = healthChange.value * (1 - resist) * healthChange.effectiveness[i];
                if(curHp + change >= 0.0f) {
                    hitpoints[i] += change;
                    healthChange = new HealthChange(0.0f, new float[3], new bool[3]);
                } else {
                    hitpoints[i] = 0.0f;
                    HealthChange temp = healthChange;
                    healthChange = new HealthChange(temp.value - curHp, temp.effectiveness, temp.bypasses);
                }
            }
        }
    }
}
