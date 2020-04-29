using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureDockingManager : MonoBehaviour {
    [Header("Docking")]
    public List<GameObject> dockingBays = new List<GameObject>();

    StructureInitializer structureInitializer;
    StructureStatsManager structureStatsManager;

    // Has this component been initialized?
    bool initialized = false;

    public void Initialize(StructureInitializer initializer) {
        structureInitializer = initializer;
        structureStatsManager = initializer.structureStatsManager;
        InitializeDockingBays();
        initialized = true;
    }

    void InitializeDockingBays() {
        GameObject db = new GameObject("Docking Bays");
        db.transform.parent = transform;
        db.transform.localPosition = Vector3.zero;
        db.transform.localRotation = Quaternion.identity;
        int bays = structureStatsManager.profile.dockingBayLocations.Length;
        dockingBays = new List<GameObject>();
        for(int i = 0; i < bays; i++) {
            GameObject dockingBay = new GameObject("DB");
            dockingBay.transform.parent = db.transform;
            dockingBay.transform.localPosition = structureStatsManager.profile.dockingBayLocations[i];
            dockingBay.transform.localRotation = Quaternion.identity;
            dockingBays.Add(dockingBay);
        }
    }

    public void Dock(StructureStatsManager requester) {
        if(!initialized) return;
        if((transform.position - requester.transform.position).sqrMagnitude <= 100.0f) {
            GameObject accepted = null;
            foreach(GameObject dockingBay in dockingBays) {
                if(dockingBay.transform.childCount == 0) {
                    accepted = dockingBay;
                    break;
                }
            }
            if(accepted != null) {
                requester.transform.parent = accepted.transform;
                requester.transform.localPosition = Vector3.zero;
                requester.transform.localRotation = Quaternion.identity;
                requester.GetComponent<MeshCollider>().enabled = false;
                requester.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public void Undock(StructureStatsmanager requester) {
        requester.transform.parent = null;
        requester.transform.Translate(Vector3.forward * 10.0f);
        requester.GetComponent<MeshCollider>().enabled = true;
        requester.GetComponent<Rigidbody>().isKinematic = false;
    }
}
