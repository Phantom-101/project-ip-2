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
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null && structure.factionID != currentFactionID && (transform.position - structure.transform.position).sqrMagnitude <= range * range) {
                factionsManager.SetRelations (currentFactionID, structure.factionID, -1.0f);
                factionsManager.SetRelations (structure.factionID, currentFactionID, -1.0f);
            }
    }
}
