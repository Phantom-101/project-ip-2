using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEquipmentManager : MonoBehaviour {
    public List<Equipment> equipment = new List<Equipment>();
    public List<GameObject> equipmentGOs = new List<GameObject>();
    public List<GameObject> dockingBays = new List<GameObject>();

    StructureStatsManager ssm;

    void Start() {
        ssm = GetComponent<StructureStatsManager>();
        InitializeEquipment();
        InitializeDockingBays();
    }

    void InitializeEquipment() {
        GameObject e = new GameObject("Equipment");
        e.transform.parent = transform;
        e.transform.localPosition = Vector3.zero;
        e.transform.localRotation = Quaternion.identity;
        int allowedEquipmentCount = ssm.profile.equipmentLocations.Length;
        if (equipment.Count != allowedEquipmentCount) {
            equipment = new List<Equipment>();
            for(int i = 0; i < allowedEquipmentCount; i++) equipment.Add(null);
        }
        for(int i = 0; i < allowedEquipmentCount; i++) {
            GameObject point = new GameObject("EAP");
            point.transform.parent = e.transform;
            point.AddComponent<EquipmentAttachmentPoint>();
            point.transform.localPosition = ssm.profile.equipmentLocations[i];
        }
        equipmentGOs = new List<GameObject>();
        for (int i = 0; i < allowedEquipmentCount; i++) {
            GameObject equipmentGO = e.transform.GetChild(i).gameObject;
            equipmentGOs.Add(equipmentGO);
            if(equipment[i] != null) {
                if(equipment[i].meta > ssm.profile.equipmentMaxMeta) equipment[i] = null;
                else {
                    EquipmentAttachmentPoint equipmentScript = equipmentGO.GetComponent<EquipmentAttachmentPoint>();
                    equipmentScript.equipment = equipment[i];
                    if(equipment[i].accepted.Length > 0) equipmentScript.LoadCharge(equipment[i].accepted[0], 100);
                    equipmentScript.Initialize();
                }
            }
        }
    }

    void InitializeDockingBays() {
        GameObject db = new GameObject("Docking Bays");
        db.transform.parent = transform;
        db.transform.localPosition = Vector3.zero;
        db.transform.localRotation = Quaternion.identity;
        int bays = ssm.profile.dockingBayLocations.Length;
        dockingBays = new List<GameObject>();
        for(int i = 0; i < bays; i++) {
            GameObject dockingBay = new GameObject("DB");
            dockingBay.transform.parent = db.transform;
            dockingBay.transform.localPosition = ssm.profile.dockingBayLocations[i];
            dockingBay.transform.localRotation = Quaternion.identity;
            dockingBays.Add(dockingBay);
        }
    }

    public void RequestToDock(StructureStatsManager ssm) {
        if((transform.position - ssm.transform.position).sqrMagnitude <= 100.0f) {
            GameObject accepted = null;
            foreach(GameObject dockingBay in dockingBays) {
                if(dockingBay.transform.childCount == 0) {
                    accepted = dockingBay;
                    break;
                }
            }
            if(accepted != null) {
                ssm.transform.parent = accepted.transform;
                ssm.transform.localPosition = Vector3.zero;
                ssm.transform.localRotation = Quaternion.identity;
                ssm.GetComponent<MeshCollider>().enabled = false;
                ssm.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public void TryActivateAllEquipment(GameObject to) {
        for(int i = 0; i < equipmentGOs.Count; i++) {
            EquipmentAttachmentPoint equipmentScript = equipmentGOs[i].GetComponent<EquipmentAttachmentPoint>();
            equipmentScript.target = to;
            equipmentScript.SetEquipmentActive(true);
        }
    }

    public void TryToggleAllEquipment(GameObject to) {
        bool hasActive = false;
        foreach(GameObject equipmentGO in equipmentGOs)
            if(equipmentGO.GetComponent<EquipmentAttachmentPoint>().equipmentActive) {
                hasActive = true;
                break;
            }
        
        for(int i = 0; i < equipmentGOs.Count; i++) {
            EquipmentAttachmentPoint equipmentScript = equipmentGOs[i].GetComponent<EquipmentAttachmentPoint>();
            equipmentScript.target = to;
            equipmentScript.SetEquipmentActive(!hasActive);
        }
    }

    public void TryActivateEquipment(int index, GameObject to) {
        EquipmentAttachmentPoint equipmentScript = equipmentGOs[index].GetComponent<EquipmentAttachmentPoint>();
        equipmentScript.target = to;
        equipmentScript.SetEquipmentActive(true);
    }

    public void ToggleEquipment(int index, GameObject to, bool b) {
        EquipmentAttachmentPoint equipmentScript = equipmentGOs[index].GetComponent<EquipmentAttachmentPoint>();
        equipmentScript.target = to;
        equipmentScript.SetEquipmentActive(b);
    }   
}
