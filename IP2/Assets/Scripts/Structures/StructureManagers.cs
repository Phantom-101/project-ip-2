using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManagers : MonoBehaviour {
    public StructureStatsManager structureStatsManager;
    public StructureMovementManager structureMovementManager;
    public StructureEquipmentManager structureEquipmentManager;

    void Awake() {
        structureStatsManager = GetComponent<StructureStatsManager>();
        if(structureStatsManager == null) structureStatsManager = gameObject.AddComponent<StructureStatsManager>();
        structureStatsManager.structureManagers = this;
        structureMovementManager = GetComponent<StructureMovementManager>();
        if(structureMovementManager == null) structureMovementManager = gameObject.AddComponent<StructureMovementManager>();
        structureMovementManager.structureManagers = this;
        structureEquipmentManager = GetComponent<StructureEquipmentManager>();
        if(structureEquipmentManager == null) structureEquipmentManager = gameObject.AddComponent<StructureEquipmentManager>();
        structureEquipmentManager.structureManagers = this;
    }
}
