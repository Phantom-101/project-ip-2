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