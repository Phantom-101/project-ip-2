using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEquipmentManager : MonoBehaviour {
    [Header("Equipment")]
    public List<Equipment> equipment = new List<Equipment>();
    public List<GameObject> equipmentGOs = new List<GameObject>();

    StructureInitializer structureInitializer;
    StructureStatsManager structureStatsManager;

    // Has this component been initialized?
    bool initialized = false;

    public void Initialize(StructureInitializer initializer) {
        structureInitializer = initializer;
        structureStatsManager = initializer.structureStatsManager;
        InitializeEquipment();
        initialized = true;
    }

    void InitializeEquipment() {
        GameObject e = new GameObject("Equipment");
        e.transform.parent = transform;
        e.transform.localPosition = Vector3.zero;
        e.transform.localRotation = Quaternion.identity;
        int allowedEquipmentCount = structureStatsManager.profile.equipmentLocations.Length;
        if (equipment.Count != allowedEquipmentCount) {
            equipment = new List<Equipment>();
            for(int i = 0; i < allowedEquipmentCount; i++) equipment.Add(null);
        }
        for(int i = 0; i < allowedEquipmentCount; i++) {
            GameObject point = new GameObject("EAP");
            point.transform.parent = e.transform;
            point.AddComponent<EquipmentAttachmentPoint>();
            point.transform.localPosition = structureStatsManager.profile.equipmentLocations[i];
        }
        equipmentGOs = new List<GameObject>();
        for (int i = 0; i < allowedEquipmentCount; i++) {
            GameObject equipmentGO = e.transform.GetChild(i).gameObject;
            equipmentGOs.Add(equipmentGO);
            if(equipment[i] != null) {
                if(equipment[i].meta > structureStatsManager.profile.equipmentMaxMeta) equipment[i] = null;
                else {
                    EquipmentAttachmentPoint equipmentScript = equipmentGO.GetComponent<EquipmentAttachmentPoint>();
                    equipmentScript.equipment = equipment[i];
                    if(equipment[i].accepted.Length > 0) equipmentScript.LoadCharge(equipment[i].accepted[0], 100);
                    equipmentScript.Initialize();
                }
            }
        }
    }

    public void TryActivateAllEquipment(GameObject to) {
        if(!initialized) return;
        for(int i = 0; i < equipmentGOs.Count; i++) {
            EquipmentAttachmentPoint equipmentScript = equipmentGOs[i].GetComponent<EquipmentAttachmentPoint>();
            equipmentScript.target = to;
            equipmentScript.SetEquipmentActive(true);
        }
    }

    public void TryToggleAllEquipment(GameObject to) {
        if(!initialized) return;
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
        if(!initialized) return;
        EquipmentAttachmentPoint equipmentScript = equipmentGOs[index].GetComponent<EquipmentAttachmentPoint>();
        equipmentScript.target = to;
        equipmentScript.SetEquipmentActive(true);
    }

    public void ToggleEquipment(int index, GameObject to, bool b) {
        if(!initialized) return;
        EquipmentAttachmentPoint equipmentScript = equipmentGOs[index].GetComponent<EquipmentAttachmentPoint>();
        equipmentScript.target = to;
        equipmentScript.SetEquipmentActive(b);
    }   
}
