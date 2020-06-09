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
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null && (transform.position - structure.transform.position).sqrMagnitude <= range * range)
                factionsManager.SetRelations (currentFactionID, structure.factionID, -1.0f);
    }
}
