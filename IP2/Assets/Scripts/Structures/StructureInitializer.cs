using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInitializer : MonoBehaviour {
    public StructureStatsManager structureStatsManager;
    public StructureEquipmentManager structureEquipmentManager;
    public StructureMovementManager structureMovementManager;
    public StructureDockingManager structureDockingManager;
    public StructureInventoryManager structureInventoryManager;

    void Awake() {
        structureStatsManager = GetComponent<StructureStatsManager>();
        if(structureStatsManager == null) structureStatsManager = gameObject.AddComponent<StructureStatsManager>();
        structureEquipmentManager = GetComponent<StructureEquipmentManager>();
        if(structureEquipmentManager == null) structureEquipmentManager = gameObject.AddComponent<StructureEquipmentManager>();
        structureMovementManager = GetComponent<StructureMovementManager>();
        if(structureMovementManager == null) structureMovementManager = gameObject.AddComponent<StructureMovementManager>();
        structureDockingManager = GetComponent<StructureDockingManager>();
        if(structureDockingManager == null) structureDockingManager = gameObject.AddComponent<StructureDockingManager>();
        structureInventoryManager = GetComponent<StructureInventoryManager>();
        if(structureInventoryManager == null) structureInventoryManager = gameObject.AddComponent<StructureInventoryManager>();
        structureStatsManager.Initialize(this);
        structureEquipmentManager.Initialize(this);
        structureMovementManager.Initialize(this);
        structureDockingManager.Initialize(this);
        structureInventoryManager.Initialize(this);
    }
}
