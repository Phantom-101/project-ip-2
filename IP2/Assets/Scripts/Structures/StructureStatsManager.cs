using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureStatsManager : MonoBehaviour
{
    public StructureProfile profile;
    public Dictionary<string, StructureStat> stats = new Dictionary<string, StructureStat>();
    public List<StructureStatModifiersPackage> modifiersPackages = new List<StructureStatModifiersPackage>();
    public Dictionary<Item, int> cargoHold = new Dictionary<Item, int>();
    public string faction;

    StructuresManager sm;

    void Awake() {
        GetComponent<MeshFilter>().mesh = profile.mesh;
        GetComponent<MeshCollider>().sharedMesh = profile.mesh;
        InitializeStats();
    }

    void Start() {
        StartCoroutine(DamageApplicationLoop());
    }

    void Update() {
        IterateModifiersPackages();
        CheckHitpointStats();
    }

    void InitializeStats() {
        sm = FindObjectOfType<StructuresManager>();
        // Structure stats
        stats.Add("Hull Max", new StructureStat(profile.hull));
        stats.Add("Hull", new StructureStat(profile.hull));
        stats.Add("Armor Max", new StructureStat(profile.armor));
        stats.Add("Armor", new StructureStat(profile.armor));
        stats.Add("Shield Max", new StructureStat(profile.shield));
        stats.Add("Shield", new StructureStat(profile.shield));
        stats.Add("Damage Pool Max", new StructureStat(profile.damagePool));
        stats.Add("Damage Pool", new StructureStat(0.0f));
        stats.Add("Capacitance", new StructureStat(profile.capacitance));
        stats.Add("Generation", new StructureStat(profile.generation));
        stats.Add("Speed", new StructureStat(profile.speed));
        stats.Add("Speed Interpolation", new StructureStat(profile.speedInterpolation));
        stats.Add("Turn Speed", new StructureStat(profile.turnSpeed));
        stats.Add("Turn Speed Interpolation", new StructureStat(profile.turnSpeedInterpolation));
        stats.Add("Warp Speed", new StructureStat(profile.warpSpeed));
        stats.Add("Warp Accuracy", new StructureStat(profile.warpAccuracy));
        stats.Add("Warp Field Strength", new StructureStat(profile.warpFieldStrength));
        stats.Add("Scanners Range", new StructureStat(profile.scannersRange));
        stats.Add("Scanners Strength", new StructureStat(profile.scannersStrength));
        stats.Add("Signature Strength", new StructureStat(profile.signatureStrength));
        stats.Add("Cargo Hold Size", new StructureStat(profile.cargoHoldSize));
        // Resistances
        stats.Add("Hull Resistance", new StructureStat(profile.hullResistance));
        stats.Add("Armor Resistance", new StructureStat(profile.armorResistance));
        stats.Add("Shield Resistance", new StructureStat(profile.shieldResistance));
        // Multipliers
        stats.Add("Turrets Damage", new StructureStat(1.0f));
    }

    public void AddModifiersPackage(StructureStatModifiersPackage package) {
        modifiersPackages.Add(package);
        foreach(StructureStatModifier modifier in package.modifiers) AddModifier(modifier);
    }

    public void IterateModifiersPackages() {
        foreach(StructureStatModifiersPackage package in modifiersPackages) {
            package.duration -= Time.deltaTime;
            if(package.duration <= 0.0f)
                foreach(StructureStatModifier modifier in package.modifiers) RemoveModifier(modifier);
        }
    }

    public void AddModifier(StructureStatModifier modifier) {
        if(modifier.statModifierType == StructureStatModifierType.ImmediateAdditive) stats[modifier.targetStat].baseValue += modifier.value;
        else stats[modifier.targetStat].modifiers.Add(modifier);
    }

    public void RemoveModifier(StructureStatModifier modifier) {
        stats[modifier.targetStat].modifiers.Remove(modifier);
    }

    public void SetStat(string statName, float v) {
        stats[statName].baseValue = v;
    }

    public float GetStat(string statName) {
        float v = stats[statName].baseValue;
        float additive = 0.0f;
        float multiplicative = 1.0f;
        float percent = 0.0f;
        foreach(StructureStatModifier mod in stats[statName].modifiers) {
            if(mod.statModifierType == StructureStatModifierType.Additive) {
                additive += mod.value;
            } else if (mod.statModifierType == StructureStatModifierType.Multiplicative) {
                multiplicative *= mod.value;
            } else if(mod.statModifierType == StructureStatModifierType.Percent) {
                percent += mod.value;
            }
        }
        return v + additive + (multiplicative * v - v) + (percent / 100.0f) * v;
    }

    void CheckHitpointStats() {
        if (GetStat("Shield") < 0.0f) SetStat("Shield", 0.0f);
        if (GetStat("Shield") > GetStat("Shield Max")) SetStat("Shield", GetStat("Shield Max"));
        if (GetStat("Armor") < 0.0f) SetStat("Armor", 0.0f);
        if (GetStat("Armor") > GetStat("Armor Max")) SetStat("Armor", GetStat("Armor Max"));
        if (GetStat("Hull") <= 0.0f) sm.Destroyed(this);
        if (GetStat("Hull") > GetStat("Hull Max")) SetStat("Hull", GetStat("Hull Max"));
    }

    IEnumerator DamageApplicationLoop() {
        ApplyDamage();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(DamageApplicationLoop());
    }

    void ApplyDamage() {
        if(GetStat("Damage Pool") > GetStat("Damage Pool Max")) SetStat("Damage Pool", GetStat("Damage Pool Max"));
        ApplyDamageToShield();
        ApplyDamageToArmor();
        ApplyDamageToHull();
        CheckHitpointStats();
    }

    void ApplyDamageToShield() {
        float shield = GetStat("Shield");
        float resist = GetStat("Shield Resistance");
        float damage = GetStat("Damage Pool") * (1 - resist);
        if(damage <= shield) {
            AddModifier(new StructureStatModifier("Shield", StructureStatModifierType.ImmediateAdditive, -damage));
            SetStat("Damage Pool", 0.0f);
        } else {
            AddModifier(new StructureStatModifier("Damage Pool", StructureStatModifierType.ImmediateAdditive, -shield));
            SetStat("Shield", 0.0f);
        }
    }

    void ApplyDamageToArmor() {
        float armor = GetStat("Armor");
        float resist = GetStat("Armor Resistance");
        float damage = GetStat("Damage Pool") * (1 - resist);
        if(damage <= armor) {
            AddModifier(new StructureStatModifier("Armor", StructureStatModifierType.ImmediateAdditive, -damage));
            SetStat("Damage Pool", 0.0f);
        } else {
            AddModifier(new StructureStatModifier("Damage Pool", StructureStatModifierType.ImmediateAdditive, -armor));
            SetStat("Armor", 0.0f);
        }
    }

    void ApplyDamageToHull() {
        float hull = GetStat("Hull");
        float resist = GetStat("Hull Resistance");
        float damage = GetStat("Damage Pool") * (1 - resist);
        if(damage <= hull) {
            AddModifier(new StructureStatModifier("Hull", StructureStatModifierType.ImmediateAdditive, -damage));
            SetStat("Damage Pool", 0.0f);
        } else {
            AddModifier(new StructureStatModifier("Damage Pool", StructureStatModifierType.ImmediateAdditive, -hull));
            SetStat("Hull", 0.0f);
        }
    }

    public void ChangeItem(Item item, int amount) {
        if (!(cargoHold.ContainsKey(item))) cargoHold[item] = 0;
        if(cargoHold[item] + amount >= 0) cargoHold[item] += amount;
        if (GetComponent<PlayerController>()) GetComponent<PlayerController>().RefreshInventory();
    }
}
