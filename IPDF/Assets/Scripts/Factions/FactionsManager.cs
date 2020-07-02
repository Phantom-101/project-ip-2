using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class FactionsManager : MonoBehaviour {
    public List<Faction> factions = new List<Faction> ();

    public void AddFaction (Faction faction) {
        if (faction == null) return;
        factions.Add (faction);
        if (faction.id == 0) {
            while (true) {
                int randomizedID = (int) Random.Range (int.MinValue, int.MaxValue);
                if (randomizedID != 0) {
                    bool idValid = true;
                    foreach (Faction f in factions)
                        if (f.id == randomizedID)
                            idValid = false;
                    if (idValid) {
                        faction.id = randomizedID;
                        break;
                    }
                }
            }
        }
    }

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

    public bool TransferWealth (int a, int b, long value) {
        if (a == b) return true;
        if (value < 0) return false;
        if (GetFaction (a) == null || GetFaction (b) == null) return false;
        if (GetFaction (a).wealth - value < 0) return false;
        GetFaction (a).wealth -= value;
        GetFaction (b).wealth += value;
        return true;
    }

    public float GetRelations (int a, int b) {
        if (a == b) return GetAllyThreshold (a);
        if (GetFaction (a) == null || GetFaction (b) == null) return 0;
        foreach (FactionRelation relation in GetFaction (a).relations)
            if (relation.id == b)
                return relation.relation;
        return 0;
    }

    public void SetRelations (int a, int b, float value) {
        if (a == b) return;
        if (GetFaction (a) == null || GetFaction (b) == null) return;
        foreach (FactionRelation relation in GetFaction (a).relations)
            if (relation.id == b) {
                relation.relation = value;
                return;
            }
        GetFaction (a).relations.Add (new FactionRelation (b, value));
    }

    public void ChangeRelations (int a, int b, float change) {
        if (a == b) return;
        if (GetFaction (a) == null || GetFaction (b) == null) return;
        foreach (FactionRelation relation in GetFaction (a).relations)
            if (relation.id == b) {
                relation.relation += change;
                return;
            }
        GetFaction (a).relations.Add (new FactionRelation (b, change));
    }

    public float GetHostileThreshold (int id) {
        Faction faction = GetFaction (id);
        if (faction == null) return 0;
        return faction.warThreshold;
    }

    public void SetHostileThreshold (int id, float value) {
        Faction faction = GetFaction (id);
        if (faction == null) return;
        faction.warThreshold = value;
    }

    public void ChangeHostileThreshold (int id, float change) {
        Faction faction = GetFaction (id);
        if (faction == null) return;
        faction.warThreshold += change;
    }

    public float GetAllyThreshold (int id) {
        Faction faction = GetFaction (id);
        if (faction == null) return 0;
        return faction.allyThreshold;
    }

    public void SetAllyThreshold (int id, float value) {
        Faction faction = GetFaction (id);
        if (faction == null) return;
        faction.allyThreshold = value;
    }

    public void ChangeAllyThreshold (int id, float change) {
        Faction faction = GetFaction (id);
        if (faction == null) return;
        faction.allyThreshold += change;
    }

    public bool Hostile (int a, int b) {
        if (a == b) return false;
        if (GetFaction (a) == null || GetFaction (b) == null) return false;
        return GetRelations (a, b) <= GetHostileThreshold (a);
    }

    public bool Ally (int a, int b) {
        if (a == b) return true;
        if (GetFaction (a) == null || GetFaction (b) == null) return false;
        return GetRelations (a, b) >= GetAllyThreshold (a);
    }

    public void ChangeRelationsWithAcquiredModification (int a, int b, float change) {
        foreach (Faction faction in factions) {
            if (faction.id == a) ChangeRelations (a, b, change);
            else if (faction.id != b) {
                if (GetRelations (faction.id, a) > 0) ChangeRelations (faction.id, b, change / 2);
                else if (GetRelations (faction.id, a) < 0) ChangeRelations (faction.id, b, -change / 2);
            }
        }
    }
}
