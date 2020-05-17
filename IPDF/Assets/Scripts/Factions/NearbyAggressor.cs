using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyAggressor : MonoBehaviour {
    public string currentFaction;
    public float range;
    
    StructuresManager structuresManager;
    DiplomacyManager diplomacyManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        diplomacyManager = FindObjectOfType<DiplomacyManager> ();
        foreach (StructureBehaviours structure in structuresManager.structures)
            if ((transform.position - structure.transform.position).sqrMagnitude <= range * range)
                diplomacyManager.SetRelations (currentFaction, structure.faction, -1.0f);
    }
}
