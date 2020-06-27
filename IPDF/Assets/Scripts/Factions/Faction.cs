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
    public List<FactionRelation> relations = new List<FactionRelation> ();
    public float warThreshold;
    public float allyThreshold;

    public Faction (string name, string abbreviated, long wealth, List<FactionRelation> relations, float warThreshold = -1000, float allyThreshold = 1000) {
        this.name = name;
        this.abbreviated = abbreviated;
        this.wealth = wealth;
        this.relations = relations;
        this.warThreshold = warThreshold;
        this.allyThreshold = allyThreshold;
    }
}

[Serializable]
public class FactionRelation {
    public int id;
    public float relation;

    public FactionRelation (int id, float relation) {
        this.id = id;
        this.relation = relation;
    }
}