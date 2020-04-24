using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEquipmentManager : MonoBehaviour {
    public StructureManagers structureManagers;
    public List<Equipment> equipment = new List<Equipment>();
    public List<GameObject> equipmentGOs = new List<GameObject>();

    StructureStatsManager ssm;

    void Start() {
        InitializeModules();
    }

    void InitializeModules() {
        ssm = structureManagers.structureStatsManager;
        GameObject e = new GameObject("Equipment");
        e.transform.parent = transform;
        e.transform.localPosition = Vector3.zero;
        e.transform.localRotation = Quaternion.identity;
        int allowedEquipmentCount = ssm.profile.maxEquipment;
        for(int i = 0; i < allowedEquipmentCount; i++) {
            GameObject point = new GameObject("EAP");
            point.transform.parent = e.transform;
            point.AddComponent<EquipmentAttachmentPoint>();
            point.transform.localPosition = ssm.profile.equipmentLocations[i];
        }
        if (equipment.Count != allowedEquipmentCount) equipment = new List<Equipment>(allowedEquipmentCount);
        equipmentGOs = new List<GameObject>(allowedEquipmentCount);
        for (int i = 0; i < allowedEquipmentCount; i++) {
            GameObject equipmentGO = e.transform.GetChild(i).gameObject;
            equipmentGOs.Add(equipmentGO);
            if (equipment[i] != null) {
                EquipmentAttachmentPoint equipmentScript = equipmentGO.GetComponent<EquipmentAttachmentPoint>();
                equipmentScript.equipment = equipment[i];
                if(equipment[i].accepted.Length > 0) equipmentScript.LoadCharge(equipment[i].accepted[0], 100);
                equipmentScript.Initialize();
            }
        }
    }

    public void TryActivateAllEquipment(GameObject to) {
        for(int i = 0; i < equipmentGOs.Count; i++) {
            EquipmentAttachmentPoint equipmentScript = equipmentGOs[i].GetComponent<EquipmentAttachmentPoint>();
            equipmentScript.target = to;
            equipmentScript.SetModuleActive(true);
        }
    }

    public void TryActivateEquipment(int index, GameObject to) {
        EquipmentAttachmentPoint equipmentScript = equipmentGOs[index].GetComponent<EquipmentAttachmentPoint>();
        equipmentScript.target = to;
        equipmentScript.SetModuleActive(true);
    }

    public void ToggleEquipment(int index, GameObject to, bool b) {
        EquipmentAttachmentPoint equipmentScript = equipmentGOs[index].GetComponent<EquipmentAttachmentPoint>();
        equipmentScript.target = to;
        equipmentScript.SetModuleActive(b);
    }   
}
