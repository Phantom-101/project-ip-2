using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class FactionsManager : MonoBehaviour {
    public List<Faction> factions = new List<Faction> ();

    public void AddFaction (Faction faction) {
        if (faction == null) return;
        factions.Add (faction);
        if (faction.id == 0) faction.id = (int) Random.Range (int.MinValue, int.MaxValue);
    }

    public Faction GetFaction (int id) {
        foreach (Faction faction in factions)
            if (faction.id == id)
                return faction;
        return null;
    }

    public long GetWealth (Faction faction) {
        return faction == null ? -1 : faction.wealth;
    }

    public bool SetWealth (Faction faction, long value) {
        if (value < 0) return false;
        if (faction == null) return false;
        faction.wealth = value;
        return true;
    }

    public bool ChangeWealth (Faction faction, long value) {
        if (faction == null) return false;
        if (faction.wealth + value < 0) return false;
        faction.wealth += value;
        return true;
    }

    public bool TransferWealth (Faction a, Faction b, long value) {
        if (a == b) return true;
        if (value < 0) return false;
        if (a == null || b == null) return false;
        if (a.wealth - value < 0) return false;
        a.wealth -= value;
        b.wealth += value;
        return true;
    }

    public float GetRelations (Faction a, Faction b) {
        if (a == b) return GetAllyThreshold (a);
        if (a == null || b == null) return 0;
        foreach (FactionRelation relation in a.relations)
            if (relation.id == b.id)
                return relation.relation;
        return 0;
    }

    public void SetRelations (Faction a, Faction b, float value) {
        if (a == b) return;
        if (a == null || b == null) return;
        foreach (FactionRelation relation in a.relations)
            if (relation.id == b.id) {
                relation.relation = value;
                return;
            }
        a.relations.Add (new FactionRelation (b.id, value));
    }

    public void ChangeRelations (Faction a, Faction b, float change) {
        if (a == b) return;
        if (a == null || b == null) return;
        foreach (FactionRelation relation in a.relations)
            if (relation.id == b.id) {
                relation.relation += change;
                return;
            }
        a.relations.Add (new FactionRelation (b.id, change));
    }

    public float GetHostileThreshold (Faction faction) {
        if (faction == null) return 0;
        return faction.warThreshold;
    }

    public void SetHostileThreshold (Faction faction, float value) {
        if (faction == null) return;
        faction.warThreshold = value;
    }

    public void ChangeHostileThreshold (Faction faction, float change) {
        if (faction == null) return;
        faction.warThreshold += change;
    }

    public float GetAllyThreshold (Faction faction) {
        if (faction == null) return 0;
        return faction.allyThreshold;
    }

    public void SetAllyThreshold (Faction faction, float value) {
        if (faction == null) return;
        faction.allyThreshold = value;
    }

    public void ChangeAllyThreshold (Faction faction, float change) {
        if (faction == null) return;
        faction.allyThreshold += change;
    }

    public bool Hostile (Faction a, Faction b) {
        if (a == b) return false;
        if (a == null || b == null) return false;
        return GetRelations (a, b) <= GetHostileThreshold (a);
    }

    public bool Ally (Faction a, Faction b) {
        if (a == b) return true;
        if (a == null || b == null) return false;
        return GetRelations (a, b) >= GetAllyThreshold (a);
    }

    public void ChangeRelationsWithAcquiredModification (Faction a, Faction b, float change) {
        foreach (Faction faction in factions) {
            if (faction.id == a.id) ChangeRelations (a, b, change);
            else if (faction.id != b.id) {
                if (GetRelations (faction, a) > 0) ChangeRelations (faction, b, change / 2);
                else if (GetRelations (faction, a) < 0) ChangeRelations (faction, b, -change / 2);
            }
        }
    }
}
