using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureStatsManager : MonoBehaviour
{
    public StructureProfile profile;
    public Dictionary<string, StructureStat> stats = new Dictionary<string, StructureStat>();
    public Dictionary<Item, int> cargoHold = new Dictionary<Item, int>();
    public string faction;

    StructuresManager sm;

    void Awake() {
        InitializeStats();
    }

    void Update() {
        if (GetStat("Hull") <= 0.0f) sm.Destroyed(this);
    }

    void InitializeStats() {
        sm = GameObject.FindObjectOfType<StructuresManager>();
        // Structure stats
        stats.Add("Hull Max", new StructureStat(profile.hull));
        stats.Add("Armor Max", new StructureStat(profile.armor));
        stats.Add("Shield Max", new StructureStat(profile.shield));
        stats.Add("Hull", new StructureStat(profile.hull));
        stats.Add("Armor", new StructureStat(profile.armor));
        stats.Add("Shield", new StructureStat(profile.shield));
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
        stats.Add("Hull Resistance", new StructureStat(0.0f));
        stats.Add("Armor Resistance", new StructureStat(0.0f));
        stats.Add("Shield Resistance", new StructureStat(0.0f));
        // Multipliers
        stats.Add("Turret Modules Damage Multiplier", new StructureStat(1.0f));
    }

    public void AddModifier(string statName, StructureStatModifier modifier) {
        stats[statName].modifiers.Add(modifier);
    }

    public void SetStat(string statName, float v) {
        List<StructureStatModifier> modifiers = stats[statName].modifiers;
        stats[statName] = new StructureStat(v);
        stats[statName].modifiers = modifiers;
    }

    public float GetStat(string statName) {
        float v = stats[statName].baseValue;
        float a = 0.0f;
        float m = 1.0f;
        foreach(StructureStatModifier mod in stats[statName].modifiers) {
            if(mod.statModifierType == StructureStatModifierType.Additive) {
                a += mod.value;
            } else {
                m *= mod.value;
            }
        }
        return v * m + a;
    }

    public void TakeDamage(float v, GameObject from)
    {
        if(GetComponent<AIController>()) GetComponent<AIController>().RespondToHelpRequest(from);
        AIController[] AIs = GameObject.FindObjectsOfType<AIController>();
        foreach(AIController AI in AIs)
        {
            if(AI.gameObject.GetComponent<StructureStatsManager>().faction != "" && AI.gameObject.GetComponent<StructureStatsManager>().faction == faction)
            {
                AI.RespondToHelpRequest(from);
            }
        }
        ApplyDamageToHull(ApplyDamageToArmor(ApplyDamageToShield(v)));
        if(GetComponent<Mineable>())
        {
            GetComponent<Mineable>().Mined(from);
        }
    }

    float ApplyDamageToShield(float v)
    {
        if(v <= GetStat("Shield"))
        {
            SetStat("Shield", GetStat("Shield") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("Shield");
            SetStat("Shield", 0.0f);
            return v;
        }
    }

    float ApplyDamageToArmor(float v)
    {
        if (v <= GetStat("Armor"))
        {
            SetStat("Armor", GetStat("Armor") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("Armor");
            SetStat("Armor", 0.0f);
            return v;
        }
    }

    float ApplyDamageToHull(float v)
    {
        if (v <= GetStat("Hull"))
        {
            SetStat("Hull", GetStat("Hull") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("Hull");
            SetStat("Hull", 0.0f);
            return v;
        }
    }

    public void ChangeItem(Item item, int amount)
    {
        if (!(cargoHold.ContainsKey(item))) cargoHold[item] = 0;
        if(cargoHold[item] + amount >= 0) cargoHold[item] += amount;
        if (GetComponent<PlayerController>()) GetComponent<PlayerController>().RefreshInventory();
    }
}
