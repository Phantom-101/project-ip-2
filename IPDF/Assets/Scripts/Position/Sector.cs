using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sector : MonoBehaviour {
    public SectorData sectorData;
    public List<StructureBehaviours> inSector = new List<StructureBehaviours> ();
    public int controllerID;

    void Update () {
        inSector.RemoveAll (structure => structure == null);
        Dictionary<int, float> control = new Dictionary<int, float> ();
        foreach (StructureBehaviours structure in inSector)
            if (structure.profile != null) {
                if (!control.ContainsKey (structure.factionID)) control[structure.factionID] = 0;
                control[structure.factionID] += structure.profile.authority;
            }
        float max = 0;
        int maxID = 0;
        foreach (int faction in control.Keys.ToArray ())
            if (control[faction] > max) {
                max = control[faction];
                maxID = faction;
            }
        controllerID = maxID;
    }
}

[Serializable]
public class SectorData {
    public int id;
    public float radius;
    public float alignment;
    public int skyboxID;
}