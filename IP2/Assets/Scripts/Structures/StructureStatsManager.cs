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

    void Start() {
        InitializeStats();
    }

    void Update() {
        if (GetStat("structure current hull") <= 0.0f) sm.Destroyed(this);
    }

    void InitializeStats() {
        sm = GameObject.FindObjectOfType<StructuresManager>();
        // Max structure stats
        stats.Add("structure max hull", new StructureStat(profile.hull));
        stats.Add("structure max armor", new StructureStat(profile.armor));
        stats.Add("structure max shield", new StructureStat(profile.shield));
        stats.Add("structure max capacitance", new StructureStat(profile.capacitance));
        stats.Add("structure max generation", new StructureStat(profile.generation));
        stats.Add("structure max speed", new StructureStat(profile.speed));
        stats.Add("structure max turnSpeed", new StructureStat(profile.turnSpeed));
        stats.Add("structure max sensorRange", new StructureStat(profile.sensorRange));
        // Current structure stats
        stats.Add("structure current hull", new StructureStat(profile.hull));
        stats.Add("structure current armor", new StructureStat(profile.armor));
        stats.Add("structure current shield", new StructureStat(profile.shield));
        stats.Add("structure current capacitance", new StructureStat(profile.capacitance));
        stats.Add("structure current generation", new StructureStat(profile.generation));
        stats.Add("structure current speed", new StructureStat(profile.speed));
        stats.Add("structure current turnSpeed", new StructureStat(profile.turnSpeed));
        stats.Add("structure current sensorRange", new StructureStat(profile.sensorRange));
        // Resistance
        stats.Add("structure current hull resistance", new StructureStat(0.0f));
        stats.Add("structure current armor resistance", new StructureStat(0.0f));
        stats.Add("structure current shield resistance", new StructureStat(0.0f));
        // Multipliers
        stats.Add("structure turret damage multiplier", new StructureStat(1.0f));
        stats.Add("structure projectile speed multiplier", new StructureStat(1.0f));
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
        if(v <= GetStat("structure current shield"))
        {
            SetStat("structure current shield", GetStat("structure current shield") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("structure current shield");
            SetStat("structure current shield", 0.0f);
            return v;
        }
    }

    float ApplyDamageToArmor(float v)
    {
        if (v <= GetStat("structure current armor"))
        {
            SetStat("structure current armor", GetStat("structure current armor") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("structure current armor");
            SetStat("structure current armor", 0.0f);
            return v;
        }
    }

    float ApplyDamageToHull(float v)
    {
        if (v <= GetStat("structure current hull"))
        {
            SetStat("structure current hull", GetStat("structure current hull") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("structure current hull");
            SetStat("structure current hull", 0.0f);
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
