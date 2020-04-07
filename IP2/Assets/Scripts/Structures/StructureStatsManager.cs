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
        if (GetStat("structure hull") <= 0.0f) sm.Destroyed(this);
    }

    void InitializeStats() {
        sm = GameObject.FindObjectOfType<StructuresManager>();
        // Structure stats
        stats.Add("structure hull max", new StructureStat(profile.hull));
        stats.Add("structure armor max", new StructureStat(profile.armor));
        stats.Add("structure shield max", new StructureStat(profile.shield));
        stats.Add("structure hull", new StructureStat(profile.hull));
        stats.Add("structure armor", new StructureStat(profile.armor));
        stats.Add("structure shield", new StructureStat(profile.shield));
        stats.Add("structure capacitance", new StructureStat(profile.capacitance));
        stats.Add("structure generation", new StructureStat(profile.generation));
        stats.Add("structure speed", new StructureStat(profile.speed));
        stats.Add("structure turn speed", new StructureStat(profile.turnSpeed));
        stats.Add("structure sensor range", new StructureStat(profile.sensorRange));
        stats.Add("structure sensor strength", new StructureStat(1.0f));
        stats.Add("structure signature strength", new StructureStat(1.0f));
        stats.Add("structure cargo hold size", new StructureStat(1.0f));
        // Resistances
        stats.Add("structure hull resistance", new StructureStat(0.0f));
        stats.Add("structure armor resistance", new StructureStat(0.0f));
        stats.Add("structure shield resistance", new StructureStat(0.0f));
        // Multipliers
        stats.Add("structure module turret damage", new StructureStat(1.0f));
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
        if(v <= GetStat("structure shield"))
        {
            SetStat("structure shield", GetStat("structure shield") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("structure shield");
            SetStat("structure shield", 0.0f);
            return v;
        }
    }

    float ApplyDamageToArmor(float v)
    {
        if (v <= GetStat("structure armor"))
        {
            SetStat("structure armor", GetStat("structure armor") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("structure armor");
            SetStat("structure armor", 0.0f);
            return v;
        }
    }

    float ApplyDamageToHull(float v)
    {
        if (v <= GetStat("structure hull"))
        {
            SetStat("structure hull", GetStat("structure hull") - v);
            return 0.0f;
        }
        else
        {
            v -= GetStat("structure hull");
            SetStat("structure hull", 0.0f);
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
