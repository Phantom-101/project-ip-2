using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Faction {
    public string name;
    public int id;
    public string abbreviated;
    public long wealth;
    public float warThreshold;
    public float allyThreshold;

    public Faction (string name, string abbreviated, long wealth, float warThreshold = -1000, float allyThreshold = 1000) {
        this.name = name;
        this.abbreviated = abbreviated;
        this.wealth = wealth;
        this.warThreshold = warThreshold;
        this.allyThreshold = allyThreshold;
    }
}

[Serializable]
public struct FactionPair {
    public Faction a;
    public Faction b;

    public FactionPair (Faction a, Faction b) {
        if (a.id < b.id) {
            this.a = a;
            this.b = b;
        } else {
            this.b = a;
            this.a = b;
        }
    }

    public FactionPair (int a, int b, FactionsManager factionsManager) {
        this.a = factionsManager.GetFaction (a);
        this.b = factionsManager.GetFaction (b);
    }

    public override int GetHashCode () {
        return a.id + b.id;
    }
}