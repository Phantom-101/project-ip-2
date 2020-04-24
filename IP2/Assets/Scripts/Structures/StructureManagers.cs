using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManagers : MonoBehaviour {
    public StructureStatsManager structureStatsManager;
    public StructureMovementManager structureMovementManager;
    public StructureEquipmentManager structureEquipmentManager;

    void Awake() {
        structureStatsManager = GetComponent<StructureStatsManager>();
        if(structureStatsManager == null) structureStatsManager = AddComponent<StructureStatsManager>();
        structureStatsManager.structureManagers = this;
        structureMovementManager = GetComponent<StructureMovementManager>();
        if(structureMovementManager == null) structureMovementManager = AddComponent<StructureMovementManager>();
        structureMovementManager.structureManagers = this;
        structureEquipmentManager = GetComponent<StructureEquipmentManager>();
        if(structureEquipmentManager == null) structureEquipmentManager = AddComponent<StructureEquipmentManager>();
        structureEquipmentManager.structureManagers = this;
    }
}
