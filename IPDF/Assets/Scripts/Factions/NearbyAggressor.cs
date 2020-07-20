using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyAggressor : MonoBehaviour {
    public int currentFactionID;
    public float range;
    
    StructuresManager structuresManager;
    FactionsManager factionsManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        factionsManager = FindObjectOfType<FactionsManager> ();
    }

    void Update () {
        Faction faction = factionsManager.GetFaction (currentFactionID);
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null && structure.faction != faction && (transform.position - structure.transform.position).sqrMagnitude <= range * range) {
                factionsManager.SetRelations (faction, structure.faction, -1000f);
                factionsManager.SetRelations (structure.faction, faction, -1000f);
            }
    }
}
