using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class FactionsManager : MonoBehaviour {
    public List<Faction> factions = new List<Faction> ();

    public Faction GetFaction (int id) {
        foreach (Faction faction in factions)
            if (faction.id == id)
                return faction;
        return null;
    }

    public long GetWealth (int id) {
        return GetFaction (id) == null ? -1 : GetFaction (id).wealth;
    }

    public bool SetWealth (int id, long value) {
        if (value < 0) return false;
        if (GetFaction (id) == null) return false;
        GetFaction (id).wealth = value;
        return true;
    }

    public bool ChangeWealth (int id, long value) {
        if (GetFaction (id) == null) return false;
        if (GetFaction (id).wealth + value < 0) return false;
        GetFaction (id).wealth += value;
        return true;
    }

    public float GetRelations (int a, int b) {
        if (GetFaction (a) == null || GetFaction (b) == null) return 0;
        foreach (FactionRelation relation in GetFaction (a).relations)
            if (relation.id == b)
                return relation.relation;
        return 0;
    }

    public void SetRelations (int a, int b, float value) {
        if (GetFaction (a) == null || GetFaction (b) == null) return;
        foreach (FactionRelation relation in GetFaction (a).relations)
            if (relation.id == b) {
                relation.relation = value;
                return;
            }
        GetFaction (a).relations.Add (new FactionRelation (b, value));
    }

    public void ChangeRelations (int a, int b, float change) {
        if (GetFaction (a) == null || GetFaction (b) == null) return;
        foreach (FactionRelation relation in GetFaction (a).relations)
            if (relation.id == b) {
                relation.relation += change;
                return;
            }
        GetFaction (a).relations.Add (new FactionRelation (b, change));
    }
}
