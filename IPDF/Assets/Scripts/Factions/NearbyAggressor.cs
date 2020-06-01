using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyAggressor : MonoBehaviour {
    public string currentFaction;
    public float range;
    
    StructuresManager structuresManager;
    FactionsManager factionsManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        factionsManager = FindObjectOfType<FactionsManager> ();
        foreach (StructureBehaviours structure in structuresManager.structures)
            if ((transform.position - structure.transform.position).sqrMagnitude <= range * range)
                factionsManager.SetRelations (currentFaction, structure.faction, -1.0f);
    }
}
