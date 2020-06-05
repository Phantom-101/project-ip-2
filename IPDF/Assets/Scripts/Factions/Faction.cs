using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Faction {
    public string name;
    public string abbreviated;
    public long wealth;
    public List<FactionRelation> relations = new List<FactionRelation> ();

    public Faction (string name, string abbreviated, long wealth, List<FactionRelation> relations) {
        this.name = name;
        this.abbreviated = abbreviated;
        this.wealth = wealth;
        this.relations = relations;
    }
}

[Serializable]
public class FactionRelation {
    public int factionID;
    public float relation;

    public FactionRelation (int factionID, float relation) {
        this.factionID = factionID;
        this.relation = relation;
    }
}