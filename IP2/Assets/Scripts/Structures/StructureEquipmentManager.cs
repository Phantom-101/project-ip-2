using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEquipmentManager : MonoBehaviour {
    public List<Equipment> equipment = new List<Equipment>();
    public List<GameObject> equipmentGOs = new List<GameObject>();

    StructureStatsManager ssm;

    void Start() {
        InitializeEquipment();
    }

    void InitializeEquipment() {
        ssm = GetComponent<StructureStatsManager>();
        GameObject e = new GameObject("Equipment");
        e.transform.parent = transform;
        e.transform.localPosition = Vector3.zero;
        e.transform.localRotation = Quaternion.identity;
        int allowedEquipmentCount = ssm.profile.maxEquipment;
        if (equipment.Count != allowedEquipmentCount) equipment = new List<Equipment>(allowedEquipmentCount);
        for(int i = 0; i < allowedEquipmentCount; i++) {
            GameObject point = new GameObject("EAP");
            point.transform.parent = e.transform;
            point.AddComponent<EquipmentAttachmentPoint>();
            point.transform.localPosition = ssm.profile.equipmentLocations[i];
        }
        equipmentGOs = new List<GameObject>(allowedEquipmentCount);
        for (int i = 0; i < allowedEquipmentCount; i++) {
            if(equipment[i].meta > ssm.profile.equipmentMaxMeta) equipment[i] = null;
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
