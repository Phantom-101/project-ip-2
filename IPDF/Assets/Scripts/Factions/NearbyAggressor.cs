using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyAggressor : MonoBehaviour {
    public int factionID;
    public Faction faction;
    public float range;
    
    StructuresManager structuresManager;
    FactionsManager factionsManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        factionsManager = FindObjectOfType<FactionsManager> ();
        faction = factionsManager.GetFaction (factionID);
        StartCoroutine (Aggress ());
    }

    IEnumerator Aggress () {
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null && structure.faction != faction && (transform.position - structure.transform.position).sqrMagnitude <= range * range) {
                factionsManager.SetRelations (faction, structure.faction, -1000f);
                factionsManager.SetRelations (structure.faction, faction, -1000f);
            }
        yield return new WaitForSeconds (5);
        StartCoroutine (Aggress ());
    }
}
